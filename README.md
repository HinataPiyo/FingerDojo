# FingerDojo

FingerDojo is a Unity project for a timing-based hand battle game prototype. The player fights a single dojo opponent using timing attacks, counters, recovery, and five difficulty profiles. Skill and taunt commands are design proposals and are not currently implemented.

## Project status
- Unity version: 6000.0.51f1
- Current code verification: `dotnet build FingerDojo.sln --no-restore` succeeds with 0 errors and one existing `EnemyStateController.stateEnter` unused-field warning
- Runtime verification status: Unity Play Mode, scene references, animation events, and balance are still unverified

## Game overview
The game flow currently implemented in code is:
1. Title screen
2. Difficulty selection
3. Battle scene with enemy profile selection
4. Player and enemy turns based on a state machine
5. Timing attacks, counters, and refresh/break behavior
6. Win/lose result screen

The five enemy profiles (Easy, Normal, Hard, Master, Transcendent) use common combat logic with profile-driven values and difficulty gimmicks. See [Docs/CombatSkillsAndDifficultySpec.md](./Docs/CombatSkillsAndDifficultySpec.md) for the proposed skill/taunt rules, initial tuning values, implementation status, and unresolved items. The values are not playtested balance.

The repository currently does not include a formal product requirements document, so the project summary above is based on code inspection and scene configuration rather than a final design spec.

## Documentation
See the project documentation in [Docs/README.md](./Docs/README.md):
- [Project Overview](./Docs/ProjectOverview.md)
- [Gameplay Specification](./Docs/GameplaySpec.md)
- [Skills, Taunt & Difficulty Specification](./Docs/CombatSkillsAndDifficultySpec.md)
- [Architecture Overview](./Docs/Architecture.md)
- [Release Checklist](./Docs/ReleaseChecklist.md)

## Repository structure
- [Assets/Scripts](./Assets/Scripts): game logic and systems
- [Assets/Scenes](./Assets/Scenes): TitleScene and BattleScene
- [Assets/ScriptableObject](./Assets/ScriptableObject): enemy, player, and command data assets
- [ProjectSettings](./ProjectSettings): Unity project settings

## Notes
This repository is best treated as an early prototype with code-level validation completed, while actual Unity runtime validation remains the next required checkpoint before calling the project release-ready.
