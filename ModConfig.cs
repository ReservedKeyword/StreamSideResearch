using System.Collections.Generic;
using System.Linq;
using MelonLoader;

namespace StreamSideResearch
{
    public class ModConfig
    {
        private const string CONFIG_PATH = "UserData/StreamSideResearch.cfg";

        // Backing fields (Twitch)
        private MelonPreferences_Entry<string> _blocklistedChatters;
        private MelonPreferences_Entry<string> _channelName;
        private MelonPreferences_Entry<string> _messageAgentCommand;
        private MelonPreferences_Entry<string> _messageCustomerCommand;
        private MelonPreferences_Entry<bool> _strictBodyPreference;
        private MelonPreferences_Entry<int> _queueSize;

        // Backing fields (UI)
        private MelonPreferences_Entry<float> _heightOffset;
        private MelonPreferences_Entry<float> _textFontSize;
        private MelonPreferences_Entry<float> _textOutlineWidth;
        private MelonPreferences_Entry<bool> _useTwitchColors;

        // Public accessors (Twitch)
        public List<string> BlocklistedChatters => ParseBlocklistChatters(_blocklistedChatters.Value);
        public string ChannelName => _channelName.Value;
        public string MessageAgentCommand => _messageAgentCommand.Value;
        public string MessageCustomerCommand => _messageCustomerCommand.Value;
        public bool StrictBodyPreference => _strictBodyPreference.Value;
        public int QueueSize => _queueSize.Value;

        // Public accessors (UI)
        public float HeighOffset => _heightOffset.Value;
        public float TextFontSize => _textFontSize.Value;
        public float TextOutlineWidth => _textOutlineWidth.Value;
        public bool UseTwitchColors => _useTwitchColors.Value;

        internal ModConfig()
        {
            InitializeTwitchSettings();
            InitializeUISettings();
        }

        private void InitializeTwitchSettings()
        {
            var twitchCategory = MelonPreferences.CreateCategory(
                identifier: "Twitch",
                display_name: "StreamSideResearch - Twitch"
            );

            twitchCategory.SetFilePath(CONFIG_PATH);

            _blocklistedChatters = twitchCategory.CreateEntry(
                identifier: "BlocklistedChatters",
                default_value: "",
                display_name: "Blocklisted Chatters",
                description: "Comma-separated list of chatters whose messages are not processed"
            );

            _channelName = twitchCategory.CreateEntry(
                identifier: "ChannelName",
                default_value: "ReservedKeyword",
                display_name: "Channel Name",
                description: "Twitch channel to listen for messages in"
            );

            _messageAgentCommand = twitchCategory.CreateEntry(
                identifier: "MessageAgentCommand",
                default_value: "!agent",
                display_name: "Agent Command",
                description: "Chat command to register chatter's intent to be an agent in game"
            );

            _messageCustomerCommand = twitchCategory.CreateEntry(
                identifier: "MessageCustomerCommand",
                default_value: "!customer",
                display_name: "Customer Command",
                description: "Chat command to register chatter's intent to be a customer in game"
            );

            _strictBodyPreference = twitchCategory.CreateEntry(
                identifier: "StrictBodyPreference",
                default_value: false,
                display_name: "Strict Body Preference",
                description: "If true, an NPC that spawns without chatters in queue with a preference toward their body type will not have a name attached"
            );

            _queueSize = twitchCategory.CreateEntry(
                identifier: "QueueSize",
                default_value: 200,
                display_name: "Queue Size",
                description: "Maximum number of unique chatters in queue"
            );

            twitchCategory.SaveToFile();
        }

        private void InitializeUISettings()
        {
            var uiCategory = MelonPreferences.CreateCategory(identifier: "UI", display_name: "StreamSideResearch - UI");

            uiCategory.SetFilePath(CONFIG_PATH);

            _heightOffset = uiCategory.CreateEntry(
                identifier: "HeightOffset",
                default_value: 0.38f,
                display_name: "Height Offset",
                description: "Vertical offset text appears above NPC's head"
            );

            _textFontSize = uiCategory.CreateEntry(
                identifier: "TextFontSize",
                default_value: 20f,
                display_name: "Text Font Size",
                description: "Font size of text shown above NPC's head"
            );

            _textOutlineWidth = uiCategory.CreateEntry(
                identifier: "TextOutlineWidth",
                default_value: 0.8f,
                display_name: "Text Outline Width",
                description: "Outline width of text shown above NPC's head"
            );

            _useTwitchColors = uiCategory.CreateEntry(
                identifier: "UseTwitchColors",
                default_value: false,
                display_name: "Use Twitch Colors",
                description: "If true, name tags will use same color as chatter in Twitch"
            );

            uiCategory.SaveToFile();
        }

        private static List<string> ParseBlocklistChatters(string value) =>
            [.. value.Split(",").Select(str => str.Trim()).Where(str => !string.IsNullOrEmpty(str))];
    }
}
