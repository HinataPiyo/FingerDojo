# Release Checklist

## Verified baseline
- [x] Repository structure inspected
- [x] Unity version confirmed
- [x] Main scenes identified
- [x] Core game systems inspected
- [x] C# project compiles successfully (`dotnet build` completed with 0 errors)
- [x] One warning remains: unused field in `EnemyStateController` (`stateEnter`)
- [x] Static review found and fixed the title-manager canvas reference becoming stale after returning from battle
- [x] Camera shake requests now share a single active coroutine to prevent overlapping shakes from resetting each other
- [x] Battle result is now first-result-wins; terminal state blocks subsequent input, damage, healing and state transitions
- [ ] Unity Editor runtime playthrough is complete
- [ ] Confirm Unity External Script Editor points to VS Code
- [ ] Final gameplay goals and release scope are documented
- [ ] Manual QA pass is complete
- [ ] Versioning and packaging metadata are confirmed for the target platform

## Required playtest checks
- [ ] Title scene loads and difficulty selection is usable
- [ ] Difficulty selection loads the battle scene with the intended profile
- [ ] Player attack flow runs without missing references
- [ ] Counter flow succeeds/fails as intended
- [ ] Break logic works and resets correctly
- [ ] Refresh action restores health without breaking state
- [ ] Enemy AI chooses valid actions and does not get stuck in Idle
- [ ] HP bars and damage UI update correctly
- [ ] Win/lose panel triggers correctly for both sides
- [ ] After win/lose, delayed enemy animation events and ongoing refresh actions do not change HP or replace the result
- [ ] Back-to-title flow works after battle
- [ ] After returning to title, open and close difficulty selection, then start another battle

## Risk review
- [ ] Ensure no serialized object references are missing in Unity scenes
- [ ] Confirm the battle state machine does not allow impossible transitions
- [ ] Check if the QTE and timing pressure are sufficiently readable to players
- [ ] Validate that all scripts attach correctly to the intended GameObjects in scene files

## Completion gate before release
This project should only be considered release-ready after:
1. Manual playthrough validation in Unity Editor
2. No critical or high-severity runtime issues remain
3. Final requirements for game content and scope are agreed
4. Release checklist items above are marked complete

## Notes
The repository is currently a prototype-level build with successful compile verification but unverified gameplay release quality. The checklist is therefore a forward validation plan rather than a claim of complete release readiness.

## Required Unity Editor checks
The Unity Editor is available and currently opens this project, but no Unity Editor control MCP or desktop interaction tool is available in this environment. Run the following manual checks in Unity:

1. Open `Assets/Scenes/TitleScene/TitleScene.unity` and enter Play Mode.
2. Open difficulty selection and select a difficulty; confirm `Assets/Scenes/BattleScene/BattleScene.unity` loads and the enemy uses the selected `EnemyProfileSO`.
3. Use the battle back-to-home button to load scene index 0.
4. Open and close the difficulty panel. Confirm there is no `MissingReferenceException` and both canvases switch correctly.
5. Start a second battle and confirm the selected profile is still initialized.
6. Exercise player attack/QTE, player counter, refresh, enemy attacks, both result panels, and return-to-home.

The title manager now refreshes its canvas references from the newly loaded TitleScene when Unity creates a duplicate manager. This fix is compile-verified but has not yet been runtime-verified in Play Mode.

## Refactoring record
### Camera shake coroutine ownership
- **Problem:** Every `Shake()` call started an independent coroutine. If another hit occurred before the previous shake ended, the older coroutine could set the Cinemachine noise gains to zero while the newer shake was still active.
- **Change:** Cache the required Cinemachine noise component in `Awake`, keep the active coroutine handle, and stop the previous shake routine before starting the latest request.
- **Behavior impact:** A single shake retains its requested duration and gains. For overlapping requests, the latest request now controls one continuous shake rather than being prematurely cleared by an older routine.
- **Performance:** No Profiler measurement was available. This is a reliability fix, not a claimed performance improvement.
- **Verification:** C# build and diff checks; Unity Play Mode overlap behavior remains unverified.
