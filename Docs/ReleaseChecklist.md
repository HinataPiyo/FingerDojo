# Release Checklist

## Verified baseline
- [x] Repository structure inspected
- [x] Unity version confirmed
- [x] Main scenes identified
- [x] Core game systems inspected
- [x] C# project compiles successfully (`dotnet build` completed with 0 errors)
- [x] One warning remains: unused field in `EnemyStateController` (`stateEnter`)
- [ ] Unity Editor runtime playthrough is complete
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
- [ ] Back-to-title flow works after battle

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
