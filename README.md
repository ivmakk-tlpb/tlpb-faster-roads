![version](https://img.shields.io/badge/version-1.0.0-blue) ![coverage](https://img.shields.io/badge/coverage-100%25-brightgreen)

# Faster Roads

Faster Roads is a mod for the Steam game *The Last Plague: Blight*. The roads between the regions give no gain in the game: you move as fast and tire as quickly as on grass or in the forest. With this mod, your character moves faster on a road and loses less stamina there, so a road is worth the detour.

## Features

- **Faster on roads.** You move 33% faster on a road, walking or running (`SpeedMultiplier`).
- **Less stamina drain.** Moving on a road drains half the stamina, so you can run about twice as long (`StaminaDrainMultiplier`).
- **Road buff.** A green buff tile shows with your status effects while you are on a road. Hover over it to see the boost.
- **What counts.** The world's roads, also when wet. Worn footpaths, bridges, and roads under snow do not count.
- **Fair play.** Only your character gets the boost. Stamina regen and the stamina cost of actions, such as chopping, stay as in the game. Both boosts and the tile can be changed in the settings.

## Save safety

The mod writes nothing to the save. The road boost buff is shown only on screen and is not a real status effect. You can remove the mod at any time, and your save loads as before.

## Multiplayer

Made and tested for single player. Co-op is not tested.

## Requirements

- The Last Plague: Blight 0.12.7 or later.
- The [BepInEx Pack for The Last Plague: Blight](https://www.nexusmods.com/thelastplagueblight/mods/9), the BepInEx 6 (IL2CPP) build for the game.

## Install

1. Install the [BepInEx Pack for The Last Plague: Blight](https://www.nexusmods.com/thelastplagueblight/mods/9). If no other mods were installed before, start the game once so BepInEx finishes setup, then quit.
2. Extract this mod's zip into the game folder (the folder with the game .exe). The DLL lands in `BepInEx\plugins`. Full path example:
   - Steam: `C:\Program Files (x86)\Steam\steamapps\common\The Last Plague\BepInEx\plugins\FasterRoads.dll`

## Uninstall

Delete `FasterRoads.dll` from the `BepInEx\plugins` folder. On the next game start, roads work as in the game.

## Configuration

To change mod settings, edit `BepInEx\config\com.ivmakk.thelastplagueblight.fasterroads.cfg` while the game is closed (see [CONFIG.md](CONFIG.md) for every setting). Changes apply at the next game start.

## Troubleshooting

If a game update renames a part of the game that the mod needs for the road boost, the whole mod turns itself off with one warning in the log, and the game works as without it. If only a part for the buff tile or its tooltip is missing, the road boost still works with no tile, and the log has one warning.

Check `BepInEx\LogOutput.log` for the `Faster Roads loaded.` line, and for warnings or errors from Faster Roads.

## Build

Faster Roads is a BepInEx 6 IL2CPP plugin. Building requires the .NET 8 SDK and a game installation with BepInEx. Start the game once after installing BepInEx to generate the IL2CPP interop assemblies.

```
dotnet build src/FasterRoads.csproj -c Release
```

`Directory.Build.props` sets `GameDir` to the default Steam installation path. For another location, set the `GameDir` environment variable or pass `-p:GameDir=...` to the build command. The output DLL is `src\bin\Release\FasterRoads.dll`.

Unit tests cover all files of `src/` and run with no game and no BepInEx, because the test project compiles the patch code against fake game types.

```
dotnet test tests/FasterRoads.Tests
```

## Package

```
dotnet build src/FasterRoads.csproj -c Release -p:Package=true
```

The zip is `dist/FasterRoads-<version>.zip`, laid out as `BepInEx/plugins/FasterRoads.dll`.

## License

Licensed under the GNU General Public License v3.0. Copyright (C) 2026 ivmakk. See [LICENSE](LICENSE).

You may reuse and modify this mod, but you must keep it open under the same license and give credit. Do not reupload it without credit.
