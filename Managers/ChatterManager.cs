using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MelonLoader;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;

namespace StreamSideResearch.Managers
{
    public class ChatterManager(Mod mod, ModConfig modConfig)
    {
        public enum AppearancePreference
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

        public class Participant
        {
            public AppearancePreference AppearancePreference { get; set; }
            public UnityEngine.Color Color { get; set; }
            public string DisplayName { get; set; }
            public NPCType NPCType { get; set; }
        }

        private const string NON_ASCII_CHARACTER = @"[^\x20-\x7E]";

        private TwitchClient twitchClient;
        private readonly MelonLogger.Instance logger = mod.LoggerInstance;

        private readonly object chatterLock = new();
        private readonly List<Participant> participants = [];
        private readonly Random random = new();

        public void Connect()
        {
            ConnectionCredentials connectionCredentials = new("justinfan1234567", "");
            twitchClient = new();

            twitchClient.Initialize(connectionCredentials);
            twitchClient.OnConnected += OnConnected;
            twitchClient.OnConnectionError += OnConnectionError;
            twitchClient.OnJoinedChannel += OnJoinedChannel;
            twitchClient.OnMessageReceived += OnMessageReceived;
            twitchClient.Connect();

            logger.Msg("Attempting to connect to Twitch IRC server...");
        }

        private NPCType? GetNPCType(string messageCommand)
        {
#if IN_TEST_MODE
            var values = Enum.GetValues<NPCType>();
            return values[random.Next(values.Length - 1)];
#else
            return messageCommand switch
            {
                _ when messageCommand.Contains(modConfig.MessageAgentCommand) => NPCType.Agent,
                _ when messageCommand.Contains(modConfig.MessageCustomerCommand) => NPCType.Customer,
                _ => null,
            };
#endif
        }

        public Participant GetRandomChatter(
            NPCType npcType,
            AppearancePreference appearancePreference = AppearancePreference.Any
        )
        {
            lock (chatterLock)
            {
                if (participants.Count == 0)
                {
                    logger.Warning("No chatters found, nothing to return");
                    return null;
                }

                if (npcType == NPCType.Unknown)
                {
                    logger.Warning("Failed to fetch random chatter: Received unknown NPC type");
                    return null;
                }

                List<Participant> eligiblePool =
                [
                    .. participants
                        .Where(participant => participant.NPCType == npcType)
                        .Where(participant =>
                            appearancePreference == AppearancePreference.Any
                            || participant.AppearancePreference == AppearancePreference.Any
                            || participant.AppearancePreference == appearancePreference
                        ),
                ];

                if (eligiblePool.Count == 0)
                {
                    if (modConfig.StrictBodyPreference)
                    {
                        logger.Warning($"No chatters with preference {appearancePreference} (strict mode enabled)");
                        return null;
                    }
                    else
                    {
                        logger.Warning($"No chatters with preference {appearancePreference}, will try all of type...");
                        eligiblePool = [.. participants.Where(participant => participant.NPCType == npcType)];

                        if (eligiblePool.Count == 0)
                        {
                            logger.Warning("No chatters are available at all");
                            return null;
                        }
                    }
                }

                var participant = eligiblePool.ElementAt(random.Next(eligiblePool.Count));
                participants.Remove(participant);
                return participant;
            }
        }

        private UnityEngine.Color GetUnityColor(string colorHex)
        {
            if (string.IsNullOrEmpty(colorHex))
            {
                return UnityEngine.Color.white;
            }

            if (!colorHex.StartsWith("#"))
            {
                colorHex = $"#{colorHex}";
            }

            if (UnityEngine.ColorUtility.TryParseHtmlString(colorHex, out UnityEngine.Color unityColor))
            {
                return unityColor;
            }

            logger.Warning($"Failed to parse HEX color {colorHex}, returning white.");
            return UnityEngine.Color.white;
        }

        private void OnConnected(object sender, OnConnectedArgs e)
        {
            logger.Msg("Connected to Twitch IRC server!");
            twitchClient.JoinChannel(modConfig.ChannelName);
            logger.Msg($"Attempting to join channel {modConfig.ChannelName} as anonymous user...");
        }

        private void OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            logger.Error($"Failed to connect to Twitch IRC client!");
            logger.Error(e.Error.Message);
        }

        private void OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            logger.Msg($"Joined channel {modConfig.ChannelName} as anonymous user.");
        }

        private void OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            var chatMessage = e.ChatMessage.Message;
            var rawDisplayName = e.ChatMessage.DisplayName;
            var displayName = SanitizeDisplayName(rawDisplayName);
            var unityColor = GetUnityColor(e.ChatMessage.ColorHex);

            if (displayName == null)
            {
                logger.Msg($"Rejecting {rawDisplayName}, fewer than 3 ASCII characters remain after sanitization");
                return;
            }

            if (modConfig.BlocklistedChatters.Contains(displayName, StringComparer.CurrentCultureIgnoreCase))
            {
                logger.Msg($"Detected blocklisted chatter {displayName}, will not add to queue.");
                return;
            }

#if !IN_TEST_MODE
            if (
                !chatMessage.Contains(modConfig.MessageAgentCommand)
                && !chatMessage.Contains(modConfig.MessageCustomerCommand)
            )
            {
                return;
            }
#endif

            if (GetNPCType(chatMessage) is not NPCType npcType)
            {
                return;
            }

            var appearancePreference = chatMessage
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .ElementAtOrDefault(1)
                ?.ToLower() switch
            {
                "f" or "female" => AppearancePreference.Female,
                "m" or "male" => AppearancePreference.Male,
                _ => AppearancePreference.Any,
            };

            lock (chatterLock)
            {
                if (participants.Count > modConfig.QueueSize)
                {
                    return;
                }

                participants.RemoveAll(participant =>
                    participant.DisplayName.Equals(displayName, StringComparison.OrdinalIgnoreCase)
                );

                participants.Add(
                    new Participant
                    {
                        AppearancePreference = appearancePreference,
                        Color = unityColor,
                        DisplayName = displayName,
                        NPCType = npcType,
                    }
                );

                logger.Msg(
                    $"Chatter {displayName} ({npcType.ToString().ToLower()}, prefers {appearancePreference.ToString().ToLower()}) in queue"
                );
            }
        }

        private static string SanitizeDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
            {
                return null;
            }

            string sanitizedName = Regex.Replace(displayName, NON_ASCII_CHARACTER, "").Trim();

            if (sanitizedName.Length < 3)
            {
                return null;
            }

            return sanitizedName;
        }
    }
}
