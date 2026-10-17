# Project Overview

## Project identity
- Product name: FingerDojo
- Unity version: 6000.0.51f1
- Main package stack: URP, UGUI, Input System, Cinemachine, Recorder, Timeline
- Primary build settings: TitleScene and BattleScene are registered in the project build settings

## Development environment check
- The current workspace is the project root at `D:\Unity\FingerDojo`; `.vscode/settings.json` selects `FingerDojo.sln`.
- Unity Editor 6000.0.51f1 is installed and a process was observed with this project open in TitleScene.
- The `visualstudiotoolsforunity.vstuc` VS Code extension is installed and is recommended by `.vscode/extensions.json`.
- No project-tracked External Script Editor preference was found. The checked per-user Unity External Tools registry preference was also absent, so the selected external editor remains unconfirmed.
- No Unity Editor MCP or desktop UI automation tool was available for this session. Unity scene editing and Play Mode interaction could not be performed automatically.

## Verified facts from the repository
The project currently contains:
- a title scene with home canvas and difficulty selection canvas
- a battle scene with player and enemy hands, command UI, timing bar, health UI, and win/lose panels
- a player/enemy battle action flow driven by `BattleState`
- enemy difficulty data stored as ScriptableObject assets
- commands and enemy AI logic implemented as MonoBehaviours attached to the player/enemy GameObjects

## Observed gameplay loop
The game appears to follow this flow:
1. Start at TitleScene
2. Select a difficulty
3. Load BattleScene with an enemy profile chosen for that difficulty
4. Battle starts with the player and enemy in a state machine
5. Player commands include attack, counter, break, and refresh
6. Enemy AI chooses actions based on its difficulty profile and weights
7. Skill and taunt commands remain unimplemented design proposals
8. Health reaches zero for either side to trigger win/lose result

## Current implementation status
### Implemented
- Title-to-difficulty screen transition
- Difficulty selection that loads the battle scene
- Player health and enemy health systems
- Player and enemy state controllers for Idle, Attack, Counter, Hit, Refresh, Break, Win/Lose states
- Timing QTE bar for attack timing
- Enemy AI weighted action selection
- Five difficulty profiles with profile-driven timings, counter behavior, refresh conditions, and higher-tier gimmicks
- Damage visualization and effect instantiation
- Win/lose panels
- First-result-wins battle termination that blocks further input, damage, healing and action state transitions after a result

### Not fully confirmed in runtime
- Whether the full loop is actually balanced and playable end-to-end
- Whether all Inspector references are assigned correctly in the Unity scene
- Whether telegraph/warning messages are readable in all resolutions
- Whether combo animation events and telegraph interruption work correctly in Play Mode
- Whether any state transitions are broken during real runtime play
- Whether the current skill and taunt proposals match the intended game design; the previous proposal is unapproved and those commands are not implemented
- Whether returning from battle and starting a second battle works in Play Mode (a stale title-canvas reference was fixed by inspection, pending runtime verification)
- Whether Unity's External Script Editor is configured to use VS Code
- Whether the first-result-wins rule and terminal guards behave correctly with active animation events and refresh coroutines in Play Mode

## Important notes
- The project is currently best described as a playable prototype/early build, not a fully reviewed release candidate.
- The build has been verified to compile successfully in the workspace, but Unity Editor runtime verification is still pending.
- The repository's gameplay design baseline is [CombatSkillsAndDifficultySpec.md](./CombatSkillsAndDifficultySpec.md); several numeric values remain initial tuning values pending playtests.
