using System.Collections.Generic;
using BepInEx.Configuration;

namespace StreamSideResearch
{
    public class PluginConfig
    {
        // General
        public ConfigEntry<bool> EnableCheats { get; private set; }

        // Overhead Settings
        public ConfigEntry<float> HeightOffset { get; private set; }
        public ConfigEntry<float> TextFontSize { get; private set; }
        public ConfigEntry<float> TextOutlineWidth { get; private set; }

        // Twitch Integration
        public List<string> BlocklistedChatters { get; private set; }
        public ConfigEntry<string> ChannelName { get; private set; }
        public ConfigEntry<string> MessageAgentCommand { get; private set; }
        public ConfigEntry<string> MessageCustomerCommand { get; private set; }
        public ConfigEntry<bool> StrictBodyPreference { get; private set; }
        public ConfigEntry<double> SubscriberWeight { get; private set; }
        public ConfigEntry<int> QueueSize { get; private set; }

        internal PluginConfig(Plugin plugin)
        {
            var configFile = plugin.Config;

            // General
            EnableCheats = configFile.Bind(
                "General",
                "Enable Cheats",
                false,
                "If true, can use hotkeys to cheat for debugging/testing mod implementation."
            );

            // Over Head UI
            HeightOffset = configFile.Bind(
                "Over Head UI",
                "Height Offset",
                0.4f,
                "Defines the vertical offset text should appear above an NPC's head."
            );

            TextFontSize = configFile.Bind(
                "Over Head UI",
                "Text Font Size",
                24f,
                "Defines the text size above an NPC's head."
            );

            TextOutlineWidth = configFile.Bind(
                "Over Head UI",
                "Text Outline Width",
                0.8f,
                "Defines the text outline width above an NPC's head."
            );

            // Twitch Integration
            var blocklistedChatters = configFile.Bind(
                "Twitch Integration",
                "Blocklisted Chatters",
                "",
                "A comma-separated list of chatter usernames whose message will not process."
            );

            BlocklistedChatters = [.. blocklistedChatters.Value.Split(",")];

            ChannelName = configFile.Bind(
                "Twitch Integration",
                "Channel Name",
                "ReservedKeyword",
                "The Twitch channel to listen for messages in."
            );

            MessageAgentCommand = configFile.Bind(
                "Twitch Integration",
                "Message (Agent) Command",
                "!agent",
                "A unique chat command, like !agent, that registers a chatter's intent to be an in-game agent."
            );

            MessageCustomerCommand = configFile.Bind(
                "Twitch Integration",
                "Message (Customer) Command",
                "!customer",
                "A unique chat command, like !customer, that registers a chatter's intent to be an in-game customer."
            );

            StrictBodyPreference = configFile.Bind(
                "Twitch Integration",
                "Strict Body Preference",
                false,
                "If true, an NPC that spawns that has no chatters with a preference toward their body type will not have a name attached."
            );

            SubscriberWeight = configFile.Bind(
                "Twitch Integration",
                "Subscriber Weight",
                1.2d,
                "A weight (higher than 1.0) that makes subscribers more likely to be chosen. (For example, 1.2 means subscribers are 20% more likely to be chosen.)"
            );

            QueueSize = configFile.Bind(
                "Twitch Integration",
                "Queue Size",
                200,
                "The limit of combined, **unique** chatters to keep in the queue. Any chatters above this limit will not be added!"
            );
        }
    }
}
