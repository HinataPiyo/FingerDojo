# Architecture Overview

## High-level structure
The project organizes logic by gameplay responsibility:
- Core managers for scene/battle state and UI coordination
- Player and enemy controllers for state transitions and animation
- Commands for button-triggered actions
- Data assets for configuration values
- UI components for health, damage numbers, and timing bars

## Main systems
### Core managers
- [GameManager.cs](../Assets/Scripts/Core/GameManager.cs): owns the battle lifecycle and exposes player/enemy health references
- [GameUIManager.cs](../Assets/Scripts/Core/GameUIManager.cs): toggles command interactivity and win/lose panels
- [CombatCalculator.cs](../Assets/Scripts/Combat/CombatCalculator.cs): computes damage for player and enemy actions

### Player side
- [PlayerStateController.cs](../Assets/Scripts/Player/PlayerStateController.cs): player battle state and transitions
- [PlayerHealth.cs](../Assets/Scripts/Player/PlayerHealth.cs): health, hit handling, healing, death, and damage UI
- command classes under [Assets/Scripts/Player/Commands](../Assets/Scripts/Player/Commands): Attack, Counter, Refresh, Break, CounterSuccess

### Enemy side
- [EnemyStateController.cs](../Assets/Scripts/Enemy/EnemyStateController.cs): enemy battle state and transitions
- [EnemyHealth.cs](../Assets/Scripts/Enemy/EnemyHealth.cs): enemy health management and death condition
- enemy AI classes under [Assets/Scripts/Enemy/AI](../Assets/Scripts/Enemy/AI): Attack, Counter, Break, Refresh, CounterSuccess, EnemyDecisionController

### Data model
- [StatusConst.cs](../Assets/Scripts/Core/Constants/StatusConst.cs): shared battle status values, including HP, attack, refresh, break, and counter timing
- [EnemyProfileSO.cs](../Assets/Scripts/Data/ScriptableObjects/EnemyProfileSO.cs): holds enemy profile settings and AI weights
- [PlayerDataSO.cs](../Assets/Scripts/Data/ScriptableObjects/PlayerDataSO.cs): holds player status data
- [EenmyDifficultyListSO.cs](../Assets/Scripts/Data/ScriptableObjects/EnemyBehavior/EenmyDifficultyListSO.cs): difficulty list used for enemy selection

### UI and effects
- [CommandTimingQTE.cs](../Assets/Scripts/UI/CommandTimingQTE.cs): handles attack timing button input
- [TimingBarController.cs](../Assets/Scripts/UI/TimingBar/TimingBarController.cs): visual timing bar logic
- [HealthSlider.cs](../Assets/Scripts/UI/HealthSlider.cs): HP UI updates
- [TakeDamageUI.cs](../Assets/Scripts/UI/TakeDamageUI.cs): floating damage text
- [CameraShake.cs](../Assets/Scripts/CameraShake.cs): screen shake via Cinemachine
- [SoundManager.cs](../Assets/Scripts/Sound/SoundManager.cs): SE/BGM routing

## Dependency summary
The code is strongly centered on MonoBehaviour components and scene object references. In practice:
- `GameManager` references the active `PlayerHealth` and `EnemyHealth`
- `CombatCalculator` references both health objects
- `PlayerStateController` and `EnemyStateController` manage the battle state machine
- UI objects directly reference the relevant controller classes through serialized fields
- Difficulty selection stores the chosen enemy configuration in `TitleUIManager` and passes it into `EnemyHealth.Initialize()`

## Observed design strengths
- Clear separation of player/enemy logic and battle state logic
- ScriptableObject-driven data for enemy/player stats
- Command pattern for player/enemy actions

## Observed design risks
- Scene-driven object references mean runtime issues can occur if a serialized field is not assigned correctly
- The project has some duplicated patterns between player and enemy logic, which makes maintenance somewhat heavier
- There is no automated test project or CI check yet, so runtime regressions are only caught by manual playtest
