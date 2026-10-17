# FingerDojo

FingerDojo is a Unity project for a timing-based hand battle game prototype. It includes a title scene, difficulty selection, a battle scene, combat state logic, enemy AI, health systems, and win/lose flow.

## Project status
- Unity version: 6000.0.51f1
- Current verified status: code compiles successfully (`dotnet build Assembly-CSharp.csproj --no-restore` completed with 0 errors and 1 warning)
- Runtime verification status: manual Unity Editor playthrough is still pending

## Game overview
The game flow currently implemented in code is:
1. Title screen
2. Difficulty selection
3. Battle scene with enemy profile selection
4. Player and enemy turns based on a state machine
5. Attack timing QTE, counter, refresh, and break behavior
6. Win/lose result screen

The repository currently does not include a formal product requirements document, so the project summary above is based on code inspection and scene configuration rather than a final design spec.

## Documentation
See the project documentation in [Docs/README.md](./Docs/README.md):
- [Project Overview](./Docs/ProjectOverview.md)
- [Gameplay Specification](./Docs/GameplaySpec.md)
- [Architecture Overview](./Docs/Architecture.md)
- [Release Checklist](./Docs/ReleaseChecklist.md)

## Repository structure
- [Assets/Scripts](./Assets/Scripts): game logic and systems
- [Assets/Scenes](./Assets/Scenes): TitleScene and BattleScene
- [Assets/ScriptableObject](./Assets/ScriptableObject): enemy, player, and command data assets
- [ProjectSettings](./ProjectSettings): Unity project settings

## Notes
This repository is best treated as an early prototype with code-level validation completed, while actual Unity runtime validation remains the next required checkpoint before calling the project release-ready.
