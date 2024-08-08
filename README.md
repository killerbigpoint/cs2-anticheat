# CS2 Anti-Cheat / TB Anti-Cheat
Anticheat plugin for Counter-Strike 2 based on CounterStrikeSharp. It is heavily inspired by SourceMod Anti-Cheat. It is currently in heavy development.

## Features / Roadmap
### Core
- [x] Config
- [ ] Ban System

### Modules
- [x] Aimbot Detection
- [x] Untrusted Angles
- [x] Rapid-Fire
- [ ] Bunnyhop (WIP)

### Integration
- [ ] IKS Admin System

## Installation
- Download [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) if you haven't already and follow their install instructions
- Download the latest release from [Releases](https://github.com/killerbigpoint/cs2-anticheat/releases)
- Drag and drop the folder **TBAntiCheat** into your plugins folder which should be in inside of `game/csgo/addons/counterstrikesharp/plugins`

## Configuration
### TODO

## Commands
| Command         | Arguments                         | Description                                                          | Permissions |
|-----------------|-----------------------------------|----------------------------------------------------------------------|-------------|
| !tbac_enable     | <1 / 0>                          | Enables/Disables the anticheat on a global level                     | @css/admin  |

## Contributing
If you know how to code and want to contribute to the project then follow the instructions down below
- Download the correct version of [DotNet SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0). As of now we use .NET 8
- Fork the project and open it up in Visual Studio, no dependencies needed as they are all in NuGet
- Make your appropiate change, test it on a server and create a pull request
- Please don't create huge pull requests if you can. Rather create smaller ones and more of them to make it more readable.

## Credits
[Original Creators of SMAC](https://forums.alliedmods.net/forumdisplay.php?f=133) - AlliedModders<br />
[Fork of SMAC](https://github.com/Silenci0/SMAC) - Silenci0<br />
[CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) - Michael Wilson<br />
