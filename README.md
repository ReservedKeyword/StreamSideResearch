# StreamSideResearch

StreamSideResearch is a MelonLoader Unity mod for the Steam game, [Roadside Research](https://store.steampowered.com/app/3643170/Roadside_Research/).

Its primary focus is on Twitch streamers who wish to add an element of interactivity with their audience, by allowing Twitch chatters to appear in the game, either as agents or customers.

## Table of Contents

* [Prerequisites](#prerequisites)
* [Getting Started](#getting-started)
* [Configuration](#configuration)
* [Questions?](#questions)

## Prerequisites

* [Roadside Research](https://store.steampowered.com/app/3643170/Roadside_Research/)
* [MelonLoader](https://melon-loader.com/#download)

## Getting Started

Before installing the mod, install [MelonLoader](#prerequisites), preferably using the official installer found on MelonLoader's website. Once MelonLoader has installed, run the game once, and wait for the main menu to appear before closing the game.

Download the latest version of StreamSideResearch from our [Releases page](https://github.com/ReservedKeyword/StreamSideResearch/releases), and drag-and-drop `StreamSideResearch-x.x.x.dll` into the `Mods` directory.

For reference, if you right-click Roadside Research in Steam, click Properties, then click on Installed Files, you should see similar to the following image. In this image, click on "Browse..." and you File Explorer will open to your game's Steam directory.

![Steam Game Location](./images/find-game-location.png)

Start Roadside Research Demo again, allowing the game *and the mod* time to fully launch, before exiting the game (again) once reaching the main menu.

Proceed to the next section in this document to learn how to configure the mod!

## Configuration

The configuration file can be found in your game's `UserData` directory, with the name `StreamSideResearch.cfg`.

The path will look similar to `/path/to/game/UserData/StreamSideResearch.cfg`, where `/path/to/game` is the path to the Roadside Research game directory. (See image above on how to locate where the game was downloaded.)

The full configuration file should look similar to the following:

```toml
[Twitch]
# Comma-separated list of chatters whose messages are not processed
BlocklistedChatters = "Fossabot,StreamElements"
# Twitch channel to listen for messages in
ChannelName = "ReservedKeyword"
# Chat command to register chatter's intent to be an agent in game
MessageAgentCommand = "!agent"
# Chat command to register chatter's intent to be a customer in game
MessageCustomerCommand = "!customer"
# If true, an NPC that spawns without chatters in queue with a preference toward their body type will not have a name attached
StrictBodyPreference = false
# Maximum number of unique chatters in queue
QueueSize = 200

[UI]
# Vertical offset text appears above NPC's head
HeightOffset = 0.38
# Font size of text shown above NPC's head
TextFontSize = 20.0
# Outline width of text shown above NPC's head
TextOutlineWidth = 0.8
# If true, name tags will use same color as chatter in Twitch
UseTwitchColors = false
```

## Questions?

If you have any questions, feel free to put them in [GitHub Issues](https://github.com/ReservedKeyword/StreamSideResearch/issues) or by email at [contact@reservedkeyword.com](mailto:contact@reservedkeyword.com).