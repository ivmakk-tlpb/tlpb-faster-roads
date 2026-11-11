# Copilot code-review instructions

This repo is a BepInEx 6 (IL2CPP) Harmony mod for *The Last Plague: Blight*. Plugins derive from `BasePlugin` and call the game through Il2CppInterop proxy assemblies. Review with these traps in mind; a general reviewer misses them.

## IL2CPP Harmony traps

- **Getter/setter patches often never fire.** il2cpp inlines trivial accessors, so a `MethodType.Getter`/`.Setter` patch silently does nothing. Flag a new getter patch used as the only mechanism. The fix is a patch of the caller, or a change of the backing field at a load hook.
- **No `is`/`as` across the interop boundary.** Flag `is`, `as`, or a direct cast on a game type. The correct form is `x.TryCast<T>()` then a null check.
- **No `foreach` over game collections.** The interop enumerator lacks the pattern. Expect `GetEnumerator()` / `MoveNext()` / `Current`, or a count plus an indexer.
- **Never read an `Il2CppSystem.ValueTuple<...>` result of a game method.** The interop layer reads the fields wrongly and gives garbage with no error.
- **A method can compile and still throw.** Some interop methods throw `NotSupportedException: Method unstripping failed` at run time, for example `ColorUtility.ToHtmlStringRGBA`. Flag a new engine call on a hot path that no game run has confirmed.
- **Guard game lookups.** Entities, views, and interface fields can be null. Flag an unchecked dereference inside a patch.
- **A patch must not break the game.** Each patch body sits in a try/catch that logs the error once with `Plugin.WarnOnce`.

## Structure and tests

- **Folders by layer.** `src/Plugin.cs` holds only the plugin: the config, the patch groups, and `Load`. `src/Game/` holds the reads of the game (the player's views, the road check), and `src/Patches/` the Harmony patches.
- **Pure logic is separated and tested.** Logic that does not need the running game lives in its own file with no BepInEx or Il2Cpp reference (`Boost`, `DrainPulse`, `RoadState`, `RoadText`).
- **Each file of `src/` has a link in the test project.** The game-facing files compile against the fake game types of `tests/FasterRoads.Tests/Fakes/`, so the tests run with no game. Flag a new source file with no `<Compile Include>` link. A fake holds only the types and members that the mod reaches. A passing test does not show that a patch fires in the game.
- **The per-frame hooks stay cheap.** The prefixes of `EntityMovementView.Move` and `StaminaView.Update` run for every entity each frame, so they compare the view pointer with the player's first and return. The road check runs at most once in `RoadState.CheckInterval`. Flag a game lookup, an allocation, or a log line before the pointer check.
- **Only the local player gets the boost.** Animals, NPCs, and other players move as in the game. Flag a change that applies the boost to another entity.
- **The game's own sums do the work.** The speed boost adds to `movementSpeedChange` and the stamina boost to `staminaExertionRateChange`, which the game reads and resets in the same frame. Flag code that writes a speed or a stamina value directly.
- **The stamina pulse counts frame time.** The game drain of a frame is its rate times the frame time, and the game writes the stamina only when the change of a frame is above 0.005. Flag a change of `DrainPulse` that counts frames or makes the drain of each frame smaller.
- **Patches** prefer a postfix, and tie the `Harmony` instance to the plugin GUID.

## Release and config hygiene

- **Verbose ships off.** The `Verbose` config binds with default `false`. Diagnostic tracing goes on `LogDebug` behind it; `LogInfo` stays quiet apart from the load line.
- **The plugin GUID never changes.** It is `com.ivmakk.thelastplagueblight.fasterroads`, the BepInEx identity and the config file name. Flag any edit to it.
- **The version is in two places that must agree:** `<Version>` in the csproj and the `BepInPlugin` attribute.
- **No committed build output.** Flag `bin/`, `obj/`, `dist/`, or a game DLL in the diff.
- **Save safety.** The mod writes nothing to the save. The road boost tile is a display entry that goes into the status list only during the HUD update. Flag a change that adds it to the list at another time, or that writes to the save.
- **Changelog matches the change.** A player-visible change adds an `[Unreleased]` entry to `CHANGELOG.md` in player-facing wording. An internal-only refactor gets none.
