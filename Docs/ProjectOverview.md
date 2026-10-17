# Project Overview

## Project identity
- Product name: FingerDojo
- Unity version: 6000.0.51f1
- Main package stack: URP, UGUI, Input System, Cinemachine, Recorder, Timeline
- Primary build settings: TitleScene and BattleScene are registered in the project build settings

## Verified facts from the repository
The project currently contains:
- a title scene with home canvas and difficulty selection canvas
- a battle scene with player and enemy hands, command UI, timing bar, health UI, and win/lose panels
- a round-based battle flow driven by `BattleState`
- enemy difficulty data stored as ScriptableObject assets
- commands and enemy AI logic implemented as MonoBehaviours attached to the player/enemy GameObjects

## Observed gameplay loop
The game appears to follow this flow:
1. Start at TitleScene
2. Select a difficulty
3. Load BattleScene with an enemy profile chosen for that difficulty
4. Battle starts with the player and enemy in a state machine
5. Player commands include attack, counter, break, and refresh
6. Enemy AI chooses actions based on its profile and weights
7. Health reaches zero for either side to trigger win/lose result

## Current implementation status
### Implemented
- Title-to-difficulty screen transition
- Difficulty selection that loads the battle scene
- Player health and enemy health systems
- Player and enemy state controllers for Idle, Attack, Counter, Hit, Refresh, Break, Win/Lose states
- Timing QTE bar for attack timing
- Enemy AI weighted action selection
- Damage visualization and effect instantiation
- Win/lose panels

### Not fully confirmed in runtime
- Whether the full loop is actually balanced and playable end-to-end
- Whether all Inspector references are assigned correctly in the Unity scene
- Whether any state transitions are broken during real runtime play
- Whether the UI or game rules are final, since the project has no completed design spec

## Important notes
- The project is currently best described as a playable prototype/early build, not a fully reviewed release candidate.
- The build has been verified to compile successfully in the workspace, but Unity Editor runtime verification is still pending.
- The repository contains no major product requirements document beyond this documentation folder, so the statements here are based on code and scene inspection rather than a formal design artifact.
