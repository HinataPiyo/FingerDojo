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

## Completion dependencies and state ownership
| Capability | Required inputs / prerequisites | Processing and state owner | Notification / completion | Failure or downstream dependency |
|---|---|---|---|---|
| Battle initialization | BattleScene objects and serialized references; selected enemy profile or scene fallback | `GameManager` calls `EnemyHealth.Initialize`; `PlayerHealth` initializes its own HP in `Awake` | Health sliders are initialized | Player commands and enemy AI assume these values and references exist; scene validation remains pending |
| Player action | `PlayerStateController.State == Idle`; command button event | Command component calls `StateChange`, which executes the command | State and command-specific animation/UI change | QTE/refresh completion returns to idle; transitions are blocked after terminal result |
| Enemy action | Enemy profile, idle timer and `Idle` state | `EnemyDecisionController` chooses an action; enemy command executes it | Enemy state/animation changes | AI periodically retries; it checks battle completion after waiting |
| Damage and counter | Damage power from QTE/animation; target health and state | `CombatCalculator` calculates damage; target Health owns HP and counter interception | Health UI updates, then hit or terminal callback | Damage is ignored after result; zero HP requests terminal resolution |
| Battle termination | Either side reaches zero HP | Health requests `GameManager.GameClear` or `GameOver`; manager owns `isGameEnd` | `GameUIManager` displays result and disables command buttons | First terminal result wins; late state/damage/healing calls are ignored |
| Retry / title return | Result UI action and TitleScene build index | `BackButton` loads title; persistent `TitleUIManager` rebinds scene canvases | Title and difficulty screens can be used again | Fresh BattleScene reload is expected to initialize a new match; manual runtime validation remains necessary |

The terminal flag is the single match-wide completion owner. Individual player/enemy battle states remain local animation/action state and are not a substitute for the terminal flag.

## Design integrity findings
### A — Resolve before considering the battle flow complete
- **Terminal-state leakage:** animation events and active refresh coroutines may still call health methods after one side has ended the match. Independently, both result methods could previously replace each other's panel. This could mutate HP after the result or show a result inconsistent with the first death.
  - **Chosen:** make the first result authoritative in `GameManager`; disable command buttons; reject post-result damage, healing and state transitions at their ownership boundaries.
  - **Alternatives:** only disable UI (does not stop animation callbacks or coroutines); cancel every individual action coroutine (more references and coupling); add a new global battle state/event system (unnecessary for the current single battle).
  - **Cost/risk:** small guard checks across the manager, health and state transition classes; product rule for simultaneous defeat is deterministic first callback wins.
  - **Verification:** compile verified; Play Mode terminal/coroutine checks remain pending.

### B — Address when affected gameplay is implemented
- **Enemy difficulty break-count display:** `EnemyAction.Break.Awake()` initializes the counter text from the serialized fallback profile, while `GameManager.Awake()` may replace the active profile afterward. If difficulty profiles have different `MaxBreakCount`, the display may be stale for the selected profile until a reset. Reinitialize the display after `EnemyHealth.Initialize()` when verifying profiles with different break counts.
- **Initialization depends on Unity Awake ordering:** enemy health UI/data initialization is invoked from `GameManager.Awake()` while other components initialize their own references in `Awake()`. Serialized references currently provide the dependency, but there is no explicit initialize-ready contract. Confirm scene behavior before introducing a bootstrap abstraction.
- **State enum overstates current feature scope:** `BattleIntro`, `Refresh_End`, `Skill`, `Goad`, `BattleClear`, and `GameOver` are declared, but not all are active controller transitions. Do not implement wave/skill systems without a confirmed product requirement.
- **Tests absent:** no automated tests were found. Key completion behavior still requires Editor playtesting; adding a test framework is deferred until a stable rules boundary exists.

### C — Defer
- **Enemy AI allocations:** action and weight lists plus LINQ `Sum` are used after a wait measured in seconds, not on every frame. No profile demonstrates a practical bottleneck; measure before optimization.
- **Refresh-code duplication:** player/enemy flows share a shape but have different data, animation, and cancellation details. Extracting a shared abstraction before behavior tests could increase coupling and change cancel semantics.
- **Global service pattern:** current single-scene battle uses singleton managers. Replacing them with dependency injection/EventBus would be disproportionate to confirmed game scope.

## Observed design strengths
- Clear separation of player/enemy logic and battle state logic
- ScriptableObject-driven data for enemy/player stats
- Command pattern for player/enemy actions

## Observed design risks
- Scene-driven object references mean runtime issues can occur if a serialized field is not assigned correctly
- The project has some duplicated patterns between player and enemy logic, which makes maintenance somewhat heavier
- There is no automated test project or CI check yet, so runtime regressions are only caught by manual playtest
- Camera shake requests are serialized through a single coroutine so an older shake cannot clear a newer request; verify overlapping hits in Play Mode

## Refactoring candidates and priority
| Priority | Target | Finding and impact | Decision |
|---|---|---|---|
| 1 | `CameraShake.Shake()` | Concurrent hit callbacks could let an earlier coroutine reset the Cinemachine gains while a later shake was still active. This could visibly truncate the later hit response. | Fixed by owning one active coroutine. Build verified; overlapping-hit Play Mode check remains pending. |
| 1 | Terminal match flow | Late animation/coroutine callbacks could modify health or controller states after a result; repeated terminal calls could replace the result panel. | Fixed with first-result-wins, terminal guards and disabled command buttons. Build verified; terminal scenario Play Mode check remains pending. |
| 2 | `Player.Commands.Refresh` and `EnemyAction.Refresh` | The refresh loops are similar but differ in their health/profile sources and components. Sharing them now would introduce coupling and risks changing cancel/state behavior without tests. | Defer until runtime behavior is covered by tests or repeated changes make the duplication costly. |
| 4 | `EnemyDecisionController.DecideState()` | Each AI decision allocates action/weight lists and uses LINQ `Sum`. It runs after an idle wait (configured as seconds), not every frame. No Profiler evidence indicates a meaningful CPU or GC issue. | Defer; measure on a representative device before optimizing. |
| 3 | `EnemyStateController.stateEnter` | The field is assigned but never read, producing the existing CS0414 warning. No gameplay effect is demonstrated by removing it. | Defer as cleanup; not a performance issue and does not block the game loop. |

No Profiler measurements were collected in this session. The CameraShake change addresses overlapping routine correctness; it is not reported as a measured performance improvement.
