# Gameplay Specification

## Scope
This document captures the game rules and flow that are explicitly visible in the implemented code and scene data. It is intentionally limited to what the repository currently proves.

## Title flow
### Home screen
The title scene contains a home canvas and a difficulty canvas.

### Difficulty selection
`TitleUIManager` handles screen transitions between the home view and difficulty selection view. `DifficultyButton` binds the difficulty selection to `SetDifficultyMode()`.

### Difficulty data
Difficulty selection uses `EenmyDifficultyListSO.Data` entries to select an `EnemyProfileSO` instance. The selected difficulty is stored in `TitleUIManager.SelectEnemyData` and loaded into the battle scene.

## Battle flow
### Battle state model
The battle uses the `BattleState` enum to represent game states:
- `BattleIntro`
- `Idle`
- `Attack`
- `Counter`
- `Counter_Success`
- `Hit`
- `Refresh`
- `Refresh_End`
- `Break`
- `Skill`
- `Goad`
- `BattleClear`
- `GameOver`

These enum values are not all active gameplay states: the state controllers implement only a subset of transitions. `GameManager.isGameEnd` is the actual battle-wide terminal gate; `BattleClear` and `GameOver` are not currently assigned as controller states.

### Completion dependencies
| Step | Preconditions and input | Processing and state owner | Completion / failure behavior | Downstream dependency |
|---|---|---|---|---|
| Start battle | BattleScene loaded; `GameManager`, both health components, UI and data assets wired | `GameManager.Awake()` initializes enemy data; `PlayerHealth.Awake()` initializes player health | Missing serialized references can fail scene startup; Editor runtime validation is pending | Player input and enemy AI require initialized health/profile |
| Accept player action | Player is in `Idle`; command buttons enabled | Button command requests a `PlayerStateController` transition and executes its `ICommand` | State controller refuses transitions after the battle ends | Action animation, QTE or refresh coroutine |
| Resolve an attack | Action is active; attack timing or enemy animation event supplies power | `CombatCalculator` forwards computed damage to the target `Health` component | Counter may intercept damage; after terminal result, further damage is ignored | Health update or win/lose determination |
| End battle | Player or enemy health reaches zero | Corresponding health component calls `GameManager.GameOver()` or `GameClear()` | First terminal result wins; command buttons are disabled; later damage, healing and state transitions are ignored | Result panel remains authoritative |
| Return/retry | Result panel's back-to-home action loads TitleScene | Scene load recreates title UI while persistent title manager adopts the new scene's canvas references | Selection and second-battle flow require Play Mode confirmation | A new BattleScene must initialize fresh battle state |

This is the flow represented by the current implementation. It does not define or claim multi-wave progression.

### Player actions
The player can trigger actions through command components attached to the player hand object:
- `Attack` executes a timing-based attack and reveals the QTE bar
- `Counter` attempts a counter state
- `Refresh` heals over time
- `Break` is triggered after repeated valid counter pressure in some logic paths

### Enemy actions
The enemy is controlled by `EnemyDecisionController`, which waits for idle times and then chooses a weighted action based on `EnemyProfileSO` values:
- attack weight
- refresh weight
- others are defined in the profile but currently commented out in the decision logic

The actual execution is handled by the enemy command classes (`Attack`, `Refresh`, `Counter`, `CounterSuccess`, `Break`).

## Timing attack behavior
`TimingBarController` animates a bar moving from left to right and back, and `CheckTiming()` returns a numeric value based on the current bar position.

`CommandTimingQTE` uses the current timing result to apply damage and then finishes the action flow. Player attack damage is computed through `CombatCalculator` and the resulting hit power is passed to `PlayerToEnemyAttack()`.

## Combat rules
- Player damage is influenced by player strength and attack power modifiers
- Enemy damage is influenced by enemy profile strength and attack power modifiers
- Counter actions trigger `PlayerToEnemyCounter()` or `EnemyToPlayerCounter()` when the state is correct
- `TakeDamage()` calls break/counter checks before applying damage
- `Hit()` cancels refresh if present and changes the state to `Hit` when the character is idle

## Win/Lose conditions
- If the player health reaches zero, `PlayerHealth.Die()` calls `GameManager.GameOver()`
- If the enemy health reaches zero, `EnemyHealth.Die()` calls `GameManager.GameClear()`
- `GameUIManager` toggles the win/lose canvas and shows the corresponding panel
- The first terminal result is now authoritative. Later damage, healing and state transitions are ignored, and command buttons are disabled.

## Hard facts versus assumptions
### Verified facts
- The code implements state transitions, enemy AI weighting, and timing-based attacks
- The project contains a win/lose UI implementation and a battle finish flow

### Likely but not yet confirmed
- Whether the pacing and balance feel good during actual playtests
- Whether the game is intended to be single-stage or multi-stage
- Whether the battle loop and UI progression are final versus prototype-only
- Whether a first-result-wins policy is the desired product rule for genuinely simultaneous defeat conditions; Unity's current synchronous call order makes it the minimal deterministic policy
