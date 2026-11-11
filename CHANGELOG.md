# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.0] - 2026-11-11

### Added

- On a road, your character moves 33% faster (`SpeedMultiplier`) and moving drains half the stamina (`StaminaDrainMultiplier`), so you can run about twice as long.
- The road boost adds to the game's own speed and stamina effects, such as an injury that slows you.
- A worn footpath, a bridge, a construction, a cave floor, and water do not count as a road. Wet roads count, and a road under snow does not.
- The boost stays on for half a second after you step off a road, so it does not flicker at the road edge.
- A green road boost buff tile shows next to your status effects while you are on a road. Its tooltip shows "On the Road" and the speed and stamina change. `ShowRoadIcon` turns the tile off.
- Only your character gets the boost. Stamina recovery and the stamina cost of actions stay as in the game.
- The mod writes nothing to the save. Made and tested for single player.
