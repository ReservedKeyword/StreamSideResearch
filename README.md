# StreamSideResearch

StreamSideResearch is a BepInEx Unity mod for the Steam game (demo), [Roadside Research](https://store.steampowered.com/app/3911640/Roadside_Research_Demo/).

Its primary focus is on Twitch streamers who wish to add an element of interactivity with their audience, by allowing Twitch chatters to appear in the game, either as agents or customers.

> [!Note]
> If you are a user that just cares about the mod and not how it was developed, feel free to skip the “How It Works” section, unless that sounds of interest to you!

## Table of Contents

* [Prerequisites](#prerequisites)
* [Getting Started](#getting-started)
* [Configuration](#configuration)
* [Questions?](#questions)

## Prerequisites

* [Roadside Research Demo](https://store.steampowered.com/app/3911640/Roadside_Research_Demo/)
* [BepInEx Stable (not Bleeding Edge)](https://docs.bepinex.dev/articles/user_guide/installation/index.html)

## Getting Started

Before installing the mod, [install BepInEx](#prerequisites) for Roadside Research and run the game. Once the game loads and the main menu appears, close the game.

Download the latest version of StreamSideResearch from our [Releases page](https://github.com/ReservedKeyword/StreamSideResearch/releases), drag-and-dropping `StreamSideResearch.dll` and _all its accompanying dependencies_ into the `BepInEx/plugins` directory.

For reference, if you right-click Roadside Research Demo in Steam, click Properties, then click on Installed Files, you should see similar to the following image. In this image, click on "Browse..." and you File Explorer will open to your game's Steam directory.

![Steam Game Location](./images/find-game-location.png)

Start Roadside Research Demo again, allowing the game *and the mod* time to fully launch, before exiting the game (again) once reaching the main menu.

Proceed to the next section in this document to learn how to configure the mod!

## Configuration

The configuration file can be found in your game's `BepInEx/config` directory, with the name `StreamSideResearch.cfg`.

The path will look similar to `/path/to/game/BepInEx/config/StreamSideResearch.cfg`, where `/path/to/game` is the path to the Roadside Research Demo game directory. (See image above on how to locate where the game was downloaded.)

![Steam Game Location](./images/find-config-location.png)

The full configuration file should look similar to the following:

```
## Settings file was created by plugin StreamSideResearch v1.0.0
## Plugin GUID: StreamSideResearch

[General]

## If true, can use hotkeys to cheat for debugging/testing mod implementation.
# Setting type: Boolean
# Default value: false
Enable Cheats = false

[Over Head UI]

## Defines the vertical offset text should appear above an NPC's head.
# Setting type: Single
# Default value: 0.4
Height Offset = 0.4

## Defines the text size above an NPC's head.
# Setting type: Single
# Default value: 24
Text Font Size = 24

## Defines the text outline width above an NPC's head.
# Setting type: Single
# Default value: 0.8
Text Outline Width = 0.8

[Twitch Integration]

## A comma-separated list of chatter usernames whose message will not process.
# Setting type: String
# Default value: 
Blocklisted Chatters = 

## The Twitch channel to listen for messages in.
# Setting type: String
# Default value: ReservedKeyword
Channel Name = ReservedKeyword

## A unique chat command, like !agent, that registers a chatter's intent to be an in-game agent.
# Setting type: String
# Default value: !agent
Message (Agent) Command = !agent

## A unique chat command, like !customer, that registers a chatter's intent to be an in-game customer.
# Setting type: String
# Default value: !customer
Message (Customer) Command = !customer

## If true, an NPC that spawns that has no chatters with a preference toward their body type will not have a name attached.
# Setting type: Boolean
# Default value: false
Strict Body Preference = true

## A weight (higher than 1.0) that makes subscribers more likely to be chosen. (For example, 1.2 means subscribers are 20% more likely to be chosen.)
# Setting type: Double
# Default value: 1.2
Subscriber Weight = 1.2

## The limit of combined, **unique** chatters to keep in the queue. Any chatters above this limit will not be added!
# Setting type: Int32
# Default value: 200
Queue Size = 200
```

## Questions?

If you have any questions, feel free to put them in [GitHub Issues](https://github.com/ReservedKeyword/StreamSideResearch/issues) or by email at [contact@reservedkeyword.com](mailto:contact@reservedkeyword.com)