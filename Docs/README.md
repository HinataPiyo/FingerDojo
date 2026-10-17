# FingerDojo Documentation

This folder collects the project overview, gameplay rules, architecture summary, and release checklist for the Unity project.

## Quick links
- [Project Overview](./ProjectOverview.md)
- [Gameplay Specification](./GameplaySpec.md)
- [Architecture Overview](./Architecture.md)
- [Release Checklist](./ReleaseChecklist.md)

## Confirmed project status
- Project name: FingerDojo
- Engine: Unity 6000.0.51f1 (from ProjectSettings/ProjectVersion.txt)
- Major scenes: TitleScene and BattleScene
- Core gameplay systems: title flow, difficulty selection, battle state machine, timing-based attack QTE, enemy AI, health and counter/break logic
- Verified build status: `dotnet build Assembly-CSharp.csproj --no-restore` succeeded with 0 errors and 1 warning
- Unverified in Unity Editor: actual runtime playthrough, scene behavior, and polished release quality

## Current state
The project is a prototype or early game loop built around a turn-based timing battle. The title screen selects a difficulty profile, then the battle scene loads an enemy profile and begins a fight where the player can attack, counter, break the enemy, or refresh.

The repository currently has no dedicated gameplay design document or QA document outside this folder, so the documentation here is intended to reflect only the implemented code and the verification that was possible in the workspace.
