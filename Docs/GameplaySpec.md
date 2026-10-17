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

## Hard facts versus assumptions
### Verified facts
- The code implements state transitions, enemy AI weighting, and timing-based attacks
- The project contains a win/lose UI implementation and a battle finish flow

### Likely but not yet confirmed
- Whether the pacing and balance feel good during actual playtests
- Whether the game is intended to be single-stage or multi-stage
- Whether the battle loop and UI progression are final versus prototype-only
