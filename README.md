# StreamSideResearch

StreamSideResearch is a MelonLoader Unity mod for the Steam game, [Roadside Research](https://store.steampowered.com/app/3643170/Roadside_Research/).

Its primary focus is on Twitch streamers who wish to add an element of interactivity with their audience, by allowing Twitch chatters to appear in the game, either as agents or customers.

## Table of Contents

* [Prerequisites](#prerequisites)
* [Getting Started](#getting-started)
* [Configuration](#configuration)
  * [Twitch](#twitch)
    * [Blocklisted Chatters](#blocklistedchatters)
    * [Channel Name](#channelname)
    * [Message Agent Command](#messageagentcommand)
    * [Message Customer Command](#messagecustomercommand)
    * [Strict Character Appearance](#strictcharacterappearance)
    * [Queue Size](#queuesize)
  * [UI](#ui)
    * [Height Offset](#heightoffset)
    * [Text Font Size](#textfontsize)
    * [Text Outline Width](#textoutlinewidth)
    * [Use Twitch Colors](#usetwitchcolors)
* [Questions?](#questions)

## Prerequisites

* [Roadside Research](https://store.steampowered.com/app/3643170/Roadside_Research/)
* [MelonLoader](https://melon-loader.com/#download)

## Getting Started

Before installing the mod, install [MelonLoader](#prerequisites), preferably using the official installer found on MelonLoader's website. Once MelonLoader has installed, run the game once, and wait for the main menu to appear before closing the game.

> [!NOTE]
> When running the game for the first time after installing MelonLoader, it may take a few minutes, as MelonLoader decompiles IL2CPP. Progress will be shown in the console window.
> 
> Ensure you wait until MelonLoader completely finishes and loads the main menu before exiting.

Download the latest version of StreamSideResearch from our [Releases page](https://github.com/ReservedKeyword/StreamSideResearch/releases), and drag-and-drop `StreamSideResearch-x.x.x.dll` into the `Mods` directory.

For reference, if you right-click Roadside Research in Steam, click Properties, then click on Installed Files, you should see similar to the following image. When clicking on "Browse...", File Explorer will open to your game's Steam directory.

![Steam Game Location](./images/find-game-location.png)

Start Roadside Research again, allowing the game *and the mod* time to fully launch, before exiting the game (again) once reaching the main menu.

## Configuration

The configuration file can be found in your game's `UserData` directory, under the name `StreamSideResearch.cfg`.

The path will look similar to `/path/to/game/UserData/StreamSideResearch.cfg`, where `/path/to/game` is the path to the Roadside Research game directory. (See image above on how to locate where the game was downloaded.)

The following configuration options are available:

### Twitch

#### `BlocklistedChatters`

A comma-separated, trimmed list of chatter(s) who should be ignored. This option is often used for bots, such as Fossabot, StreamElements, etc.

An example of multiple users would be as follows:

```toml
BlocklistedChatters = "Fossabot,StreamElements,Streamlabs"
```

#### `ChannelName`

The Twitch channel to join and listen for commands in.

#### `MessageAgentCommand`

The command that is used in Twitch chat to express chatter intent to be an in-game agent. This command, like `MessageCustomerCommand`, can also include an additional character appearance preference, such as `f` or `female` for female and `m` or `male` for male.

Assuming that `MessageAgentCommand` is equal to `!agent` (the default), the following table can be used to better understood how the command is processed by the mod.

| Command                           | Description                                                            |
| --------------------------------- | ---------------------------------------------------------------------- |
| `!agent`                          | Chatter will be assigned to any agent, regardless of character models. |
| `!agent f` **or** `!agent female` | Chatter will be assigned to only female agents.                        |
| `!agent m` **or** `!agent male`   | Chatter will be assigned to only male agents.                          |

#### `MessageCustomerCommand`

The command that is used in Twitch chat to express chatter intent to be an in-game agent. This command, like `MessageAgentCommand`, can also include an additional character appearance preference, such as `f` or `female` for female and `m` or `male` for male.

Assuming that `MessageCustomerCommand` is equal to `!customer` (the default), the following table can be used to better understood how the command is processed by the mod.

| Command                                 | Description                                                               |
| --------------------------------------- | ------------------------------------------------------------------------- |
| `!customer`                                | Chatter will be assigned to any customer, regardless of character models. |
| `!customer f` **or** `!customer female` | Chatter will be assigned to only female customers.                        |
| `!customer m` **or** `!customer male`   | Chatter will be assigned to only male customers.                          |

#### `StrictCharacterAppearance`

A boolean value (`true`/`false`) that determines how the mod behaves when the queue is limited and an NPC spawns in the game.

If this value is set to `true` and an NPC spawns in the game, the mod will **only** fetch chatters who specified the character appearance preference that is spawning (or chatters who did not specify any body type). If no chatters with the same character appearance preference as the NPC are found, the mod will NOT generate and apply a name tag to the NPC.

In contrast, if this value is set to `false` and an NPC spawns in the game, the mod will attempt to find a chatter with the same character appearance preference as the NPC. However, unlike if this value is set to `true`, if a chatter with the specified character appearance preference cannot be found, then the mod, instead of failing and not applying a name tag, will fall back to any chatter that has expressed interest in the NPC type, regardless of their character appearance preference.

Agents and customers are in separate pools. If an agent spawns in the game and no chatters can be found to apply a name tag, the mod _will not_ attempt to fetch from the pool of chatters who have chosen to be a customer.

#### `QueueSize`

The upper limit of chatters that are allowed to be in the queue at any given time.

Queue size is expected to fluctuate as agents and customers spawn in the game and chatters are popped from the queue, so this value only defines the absolute maximum value of the queue.

### UI

#### `HeightOffset`

The vertical offset above an NPC's head that the name tag text will appear.

#### `TextFontSize`

The font size of the text that will appear above an NPC's head.

#### `TextOutlineWidth`

The outline width of the text that will appear above an NPC's head.

#### `UseTwitchColors`

If `true`, the color of the text that will appear above an NPC's head will match the color of the chatter in Twitch, if they have one specified. If the chatter does not have a specified color, or if this value is set to `false`, the color of the text will be white.

## Questions?

If you have any questions, feel free to put them in [GitHub Issues](https://github.com/ReservedKeyword/StreamSideResearch/issues) or by email at [contact@reservedkeyword.com](mailto:contact@reservedkeyword.com).