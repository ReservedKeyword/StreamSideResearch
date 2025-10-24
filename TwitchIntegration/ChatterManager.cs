using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace StreamSideResearch.TwitchIntegration
{
    public class ChatterManager(PluginConfig config, ManualLogSource logger)
    {
        public enum BodyPreference
        {
            Any,
            Female,
            Male,
        }

        public enum NPCType
        {
            Agent,
            Customer,
            Unknown,
        }

        private class Participant
        {
            public BodyPreference BodyPreference { get; set; }
            public string DisplayName { get; set; }
            public bool IsSubscriber { get; set; }
            public NPCType NPCType { get; set; }
        }

        private enum TargetGroup
        {
            Subscriber,
            NonSubscriber,
            None,
        }

        private const double NON_SUBSCRIBER_WEIGHT = 1.0;

        private readonly string channelName = config.ChannelName.Value;
        private readonly List<string> blocklistedChatters = config.BlocklistedChatters;
        private readonly string messageAgentCommand = config.MessageAgentCommand.Value;
        private readonly string messageCustomerCommand = config.MessageCustomerCommand.Value;
        private readonly bool strictBodyPreference = config.StrictBodyPreference.Value;
        private readonly double subscriberWeight = config.SubscriberWeight.Value;
        private readonly int queueSize = config.QueueSize.Value;

        private TwitchClient client;
        private readonly object chattersLock = new();
        private readonly List<Participant> participants = [];
        private readonly Random random = new();

        public void Connect()
        {
            ConnectionCredentials credentials = new("justinfan1234567", "");
            client = new TwitchClient();

            client.Initialize(credentials, channelName);
            client.OnConnected += Client_OnConnected;
            client.OnConnectionError += Client_OnConnectionError;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.Connect();

            logger.LogInfo("Attempting to connect to Twitch IRC client...");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            logger.LogInfo("Connected to Twitch IRC client!");
            client.JoinChannel(channelName);
            logger.LogInfo($"Attempting to join channel {channelName} as anonymous user...");
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            logger.LogError("Failed to connect to Twitch IRC client!");
            logger.LogError(e.Error.Message);
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            logger.LogInfo($"Joined {channelName}'s Twitch channel as anonymous user!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            string displayName = e.ChatMessage.DisplayName;
            bool isSubscriber = e.ChatMessage.IsSubscriber;
            string chatMessage = e.ChatMessage.Message;

            if (blocklistedChatters.Contains(displayName, StringComparer.CurrentCultureIgnoreCase))
            {
                logger.LogInfo($"Detected blocklisted chatter {displayName}, skipping adding to queue.");
                return;
            }

            if (!chatMessage.Contains(messageAgentCommand) && !chatMessage.Contains(messageCustomerCommand))
            {
                return;
            }

            if (GetNPCTypeFromMessageCommand(chatMessage) is not NPCType npcType)
            {
                return;
            }

            // Parse if a chatter wants to be female or male in game.
            var bodyPreference = chatMessage
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ElementAtOrDefault(1)
                ?.ToLower() switch
            {
                "f" or "female" => BodyPreference.Female,
                "m" or "male" => BodyPreference.Male,
                _ => BodyPreference.Any,
            };

            lock (chattersLock)
            {
                if (participants.Count > queueSize)
                {
                    return;
                }

                participants.RemoveAll(participant =>
                    participant.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase)
                );

                participants.Add(
                    new Participant
                    {
                        BodyPreference = bodyPreference,
                        DisplayName = displayName,
                        IsSubscriber = isSubscriber,
                        NPCType = npcType,
                    }
                );

                string bodyPreferenceString =
                    bodyPreference == BodyPreference.Any ? "any" : bodyPreference.ToString().ToLower();
                string npcTypeString = npcType == NPCType.Agent ? "agent" : "customer";
                string subscriberStatus = isSubscriber ? "subscriber" : "non-subscriber";

                logger.LogInfo(
                    $"Chatter {displayName} ({npcTypeString}, {subscriberStatus}, prefers {bodyPreferenceString}) upserted in the queue!"
                );
            }
        }

        private (double totalWeight, double subscriberWeightTotal) CalculateWeights()
        {
            double subWeight = participants.Count(participant => participant.IsSubscriber) * subscriberWeight;
            double nonSubWeight = participants.Count(participant => !participant.IsSubscriber) * NON_SUBSCRIBER_WEIGHT;
            return (subWeight + nonSubWeight, subWeight);
        }

        private TargetGroup DetermineTargetGroup(double randomPick, double subscriberWeightTotal)
        {
            bool tryPickingSubscriber = randomPick < subscriberWeightTotal;
            int subscriberCount = participants.Count(participant => participant.IsSubscriber);
            int nonSubscriberCount = participants.Count(participant => !participant.IsSubscriber);

            // Pick subscriber if roll is in range & subscriber exists
            if (tryPickingSubscriber && subscriberCount > 0)
            {
                return TargetGroup.Subscriber;
            }

            // Pick a non-subscriber if roll is outside range (or a subscriber didn't exist) AND non-subscriber exists
            if (nonSubscriberCount > 0)
            {
                return TargetGroup.NonSubscriber;
            }

            // If non-subscribers didn't exist either, but subscribers did, pick a subscriber anyway.
            if (subscriberCount > 0)
            {
                return TargetGroup.Subscriber;
            }

            return TargetGroup.None;
        }

        private NPCType? GetNPCTypeFromMessageCommand(string messageCommand) =>
            messageCommand switch
            {
                var message when message.Contains(messageAgentCommand) => NPCType.Agent,
                var message when message.Contains(messageCustomerCommand) => NPCType.Customer,
                _ => null,
            };

        public string GetRandomChatter(NPCType npcType, BodyPreference bodyPreference = BodyPreference.Any)
        {
            lock (chattersLock)
            {
                if (participants.Count == 0)
                {
                    logger.LogWarning("No chatters found, nothing to return. :(");
                    return null;
                }

                if (npcType == NPCType.Unknown)
                {
                    logger.LogWarning("Failed to fetch random chatter: Received an unknown NPC type!");
                    return null;
                }

                List<Participant> eligiblePool =
                [
                    .. participants
                        .Where(participant => participant.NPCType == npcType)
                        .Where(participant =>
                            bodyPreference == BodyPreference.Any
                            || participant.BodyPreference == bodyPreference
                            || participant.BodyPreference == BodyPreference.Any
                        ),
                ];

                if (eligiblePool.Count == 0)
                {
                    if (strictBodyPreference)
                    {
                        logger.LogWarning($"No chatters found with preference {bodyPreference} (strict mode enabled).");
                        return null;
                    }
                    else
                    {
                        logger.LogWarning($"No chatters found with preference {bodyPreference}, falling back...");
                        eligiblePool = [.. participants.Where(participant => participant.NPCType == npcType)];

                        if (eligiblePool.Count == 0)
                        {
                            logger.LogWarning("No chatters available at all.");
                            return null;
                        }
                    }
                }

                (double totalWeight, double subscriberWeightTotal) = CalculateWeights();

                if (totalWeight <= 0)
                {
                    logger.LogError("Total weight is zero or negative. Cannot perform weighted pick.");
                    return null;
                }

                double randomPick = random.NextDouble() * totalWeight;
                TargetGroup target = DetermineTargetGroup(randomPick, subscriberWeightTotal);
                Participant winner = null;

                switch (target)
                {
                    case TargetGroup.Subscriber:
                        var subscribers = eligiblePool.Where(participant => participant.IsSubscriber).ToList();
                        winner = subscribers[random.Next(subscribers.Count())];
                        logger.LogInfo($"Selected winner (a subscriber): {winner.DisplayName}");
                        break;

                    case TargetGroup.NonSubscriber:
                        var nonSubscribers = eligiblePool.Where(participant => !participant.IsSubscriber).ToList();
                        winner = nonSubscribers[random.Next(nonSubscribers.Count())];
                        logger.LogInfo($"Selected winner (a non-subscriber): {winner.DisplayName}");
                        break;

                    case TargetGroup.None:
                    default:
                        logger.LogWarning("No target group determined for selection.");
                        return null;
                }

                if (winner != null)
                {
                    participants.Remove(winner);
                    logger.LogInfo($"Selected chatter {winner.DisplayName} as winner!");
                    return winner.DisplayName;
                }

                return null;
            }
        }
    }
}
