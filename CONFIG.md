# Configuration

Every setting lives in `BepInEx\config\com.ivmakk.thelastplagueblight.fasterroads.cfg`, written the first time you run the game with the mod installed. Edit it with any text editor while the game is closed. Changes apply at the next game start. A value outside its range is set to the nearest end of the range.

The settings act only on a road of the world. A road under snow gives no boost, because the snow covers it.

## General

| Setting | Default | Values | What it does |
|---|---|---|---|
| `SpeedMultiplier` | `1.33` | `1.0` to `3.0` | Your movement speed on a road is the game speed times this value. `1.33` is 33% faster. With `1.0`, the speed on a road is as in the game. |
| `StaminaDrainMultiplier` | `0.5` | `0.0` to `1.0` | The stamina that moving takes on a road is the game drain times this value. `0.5` drains half, so you can run about twice as long. `0.0` takes no stamina for moving on a road. With `1.0`, the drain is as in the game. Stamina recovery and the stamina cost of actions do not change. |
| `ShowRoadIcon` | `true` | `true` / `false` | Shows the road boost buff tile next to your status effects while you are on a road. With `false`, the boost still works, with no tile. With both multipliers at `1.0`, no tile shows. |
| `Verbose` | `false` | `true` / `false` | Writes extra diagnostic lines to the BepInEx log for troubleshooting. Keep off in normal play. |
