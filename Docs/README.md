# FingerDojo Documentation

This folder collects the project overview, gameplay rules, architecture summary, and release checklist for the Unity project.

## Quick links
- [Project Overview](./ProjectOverview.md)
- [Gameplay Specification](./GameplaySpec.md)
- [Skills, Taunt & Difficulty Proposal](./CombatSkillsAndDifficultySpec.md)
- [Architecture Overview](./Architecture.md)
- [Release Checklist](./ReleaseChecklist.md)

## Confirmed project status
- Project name: FingerDojo
- Engine: Unity 6000.0.51f1 (from ProjectSettings/ProjectVersion.txt)
- Major scenes: TitleScene and BattleScene
- Core gameplay systems: title flow, difficulty selection, battle state machine, timing-based attack QTE, enemy AI, health and counter/break logic
- Verified build status: `dotnet build FingerDojo.sln --no-restore` succeeded with 0 errors and one existing unused-field warning
- Five difficulty profiles have been added to the code/assets and compile; skill and taunt proposals are not currently implemented
- Unverified in Unity Editor: actual runtime playthrough, scene behavior, animation-event timing, and gameplay balance

## Current state
The project is a prototype or early game loop built around a timing battle. The title screen selects one of five difficulty profiles, then the battle scene loads a single enemy. The active combat loop includes attack, counter, break, and refresh. Skill and taunt commands remain unimplemented.

The gameplay specification records implemented behavior and validation limits. The skills, taunt, and five-difficulty document contains the prior unapproved skill/taunt proposal plus the difficulty implementation status; do not treat the old skill/taunt proposal as an approved implementation target. Difficulty values are initial tuning targets, not playtested balance.
