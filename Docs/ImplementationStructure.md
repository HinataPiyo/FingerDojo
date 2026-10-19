# 実装構造・コード解説

## 1. 読み方と範囲

本書はC#スクリプトの責務と、コード上の主要な処理経路を記録する。静的なコード読解に基づき、Unity Editorでのシリアライズ参照の完全性や実行時挙動は保証しない。状態・数値の詳細は[現状総合仕様書](./CurrentProjectSpecification.md)、機能単位の検証状況は[実装状況一覧](./ImplementationStatus.md)を参照。

## 2. フォルダー構成

| パス | 主な内容 |
|---|---|
| `Assets/Scripts/Core` | GameManager、GameUIManager、状態・定数、インターフェース |
| `Assets/Scripts/Player` | プレイヤー状態、体力、プレイヤーコマンド |
| `Assets/Scripts/Enemy` | 敵状態、体力、AI・敵行動 |
| `Assets/Scripts/Combat` | 戦闘ダメージ計算 |
| `Assets/Scripts/Data/ScriptableObjects` | プレイヤー／敵／難易度／音声の設定データ |
| `Assets/Scripts/UI` | QTE、HP UI、ダメージUI、タイトル／戻るボタン |
| `Assets/Scripts/Animation` | アニメーションから呼ぶ破棄処理 |
| `Assets/Scripts/Sound` | SE/BGM再生 |
| `Assets/Scripts/CameraShake.cs` | Cinemachineカメラ振動 |
| `Assets/Scripts/EffectEvents.cs` | エフェクトの寿命管理 |

プロジェクト内に確認したC#ファイルは41本。シーンとPrefabの詳細は[現状総合仕様書](./CurrentProjectSpecification.md)の一覧を参照。

## 3. 主要クラスと責務

### 3.1 Core／状態／インターフェース

| 型 | 主な責務・入口 |
|---|---|
| `GameManager` | `Awake()`でsingletonと敵初期化を行い、`isGameEnd`を保持。`GameClear()`／`GameOver()`は最初の一回だけ結果UIを呼ぶ。`StartMomentTime()`は短時間のスロー演出 |
| `GameUIManager` | コマンドボタンの一括操作、敵予兆・コマンド通知・連続攻撃警告の表示整理、勝敗パネル表示 |
| `BattleState` | Player／Enemy共通の状態名enum。宣言されているすべての状態が処理されているわけではない |
| `StatusConst` | Inspectorシリアライズ可能なHP、Strength、Defense、Speed、休息、Break、Hit、Counterの設定値 |
| `ICommand` | コマンド共通入口`Execute()` |
| `IDamageble` | `TakeDamage(float)`、`Hit()`、`Die()`の契約（識別子の綴りはコードどおり） |
| `CombatCalculator` | Player／Enemy Healthへのダメージ転送。Strengthと倍率の乗算、反撃の25%係数を実装 |

GameManager、GameUIManager、CombatCalculator、SoundManager、CameraShakeはstatic `instance`を持つ。Sceneに置かれたMonoBehaviourとInspector参照を中心とする構成で、DI/EventBusはない。

### 3.2 Player

| 型 | 主要な処理 |
|---|---|
| `PlayerStateController` | `StateChange(BattleState, ICommand)`で状態・経過時間を初期化しコマンド実行。`Update()`はCounter、Counter_Success、Hit、Breakの時間終了とIdle時ボタン有効化を扱う。`AttackImpactAnimation()`はAnimator Attackフラグを下げる |
| `PlayerHealth` | PlayerDataSOからHP初期化。`TakeDamage()`でBreak／Counterを先に判定し、その後HP、Slider、Damage UI、Hit／Dieを反映。`Healing()`は時間比例のHP加算と最大値Clamp |
| `Command.Attack` | Attack状態、敵Counter判定、敵連続入力履歴、SE／Animator／QTE表示を開始 |
| `Command.Counter` | Counter状態とCounterアニメーションを開始し、敵連続入力履歴を解除 |
| `Command.Refresh` | 回復コルーチン、エフェクト、CancelRefresh、Idle復帰を管理 |
| `Command.Break` | 敵Counter中にPlayerがダメージを受けた回数を記録し、閾値でPlayer Breakへ |
| `Command.CounterSuccess` | PlayerがCounter状態中にダメージを受けると敵Counter反撃を適用し、被ダメージを遮断 |

Playerのボタン入力コールバックは各commandの`Awake()`でButton.onClickへ登録。`StateChange`にPlayerがIdleかどうかの共通検査はなく、実際の入力可否はボタンinteractable管理に依存する。

### 3.3 Enemy／AI

| 型 | 主要な処理 |
|---|---|
| `EnemyStateController` | Enemy状態と経過時間を管理。Attack時間終了時に`ResolveAtStateEnd()`を呼び、Break／Hit／Counter等の時間終了を処理 |
| `EnemyHealth` | `Initialize(Data)`で難易度プロフィール、HP、Sliderを設定。`TakeDamage()`はBreak／Counterを処理してからHP、UI、Hit／Dieを更新。`Healing()`は実回復量を返す |
| `EnemyDecisionController` | `Start()`からAIコルーチン。Idle待ち・Break/Refresh/Idle完了待ち後、Attackと条件付きRefreshを重み付き選択。難易度別履歴・二段攻撃条件も保持 |
| `EnemyAction.Attack` | 予兆、倍率抽選、予兆文字、1段／2段攻撃、AnimationEventダメージ、時間切れ補完、重複防止を管理 |
| `EnemyAction.Counter` | プレイヤーAttack開始に応答するCounter確率・遅延・Hard予兆中処理 |
| `EnemyAction.CounterSuccess` | 敵Counter中に攻撃を受けた際、Playerへ反撃し攻撃ダメージを無効化 |
| `EnemyAction.Refresh` | 敵の回復コルーチン、残回復予算、被弾キャンセル、完了時補充 |
| `EnemyAction.Break` | PlayerがCounter状態で敵が被弾した回数を記録して敵Breakへ |

`EnemyDecisionController`にCounter／Break等の参照はあるが、重み付き候補として現在選ぶのはAttack／Refreshのみ。CounterはEnemyAction.Counterの反応処理から起動する。

### 3.4 UI、データ、演出

| 型 | 責務 |
|---|---|
| `TitleUIManager` | 難易度画面切り替え、選択Data保持、難易度ID/Profile一致の確認、Sceneロード、永続化と再ロード時Canvas参照更新 |
| `DifficultyButton` | ボタンに割り当てられたData/Profileを検査してTitleUIManagerへ依頼 |
| `SelectDifficultyButton` | ホームから難易度画面を開く |
| `BackButton` | 難易度画面を閉じる、またはBuild Index 0へ戻る |
| `CommandTimingQTE` | PointerDownを取り、倍率計算・判定表示後にQTE終了を通知 |
| `TimingBarController` | バー位置・イージング・QTE終了待ち・倍率・判定表示 |
| `TimingText` | 判定演出終了時に自身を非表示 |
| `HealthSlider` | 即時HPバーと遅れて追従するバーを更新 |
| `TakeDamageUI` | ダメージ値を小数0桁で表示 |
| `PlayerDataSO`／`EnemyProfileSO`／`EenmyDifficultyListSO` | 静的なプレイヤー値、敵値／難易度ルール、難易度対応Data |
| `AudioListSO`／`AudioTag`／`SoundManager` | タグからClipを引き、AudioSourceで再生 |
| `CameraShake` | Cinemachine Perlin振動を単一のコルーチンで管理 |
| `EffectEvents`／`AnimateAction` | エフェクト遅延破棄／任意破棄イベント |
| `EasingConst` | 0～1入力をClampするEase-in/ease-out power関数 |

## 4. ゲーム開始から結果表示まで

1. TitleScene上のSelectDifficultyButtonがTitleUIManagerへ難易度画面表示を依頼。
2. DifficultyButtonがData/Profileの有無、DifficultyId一致を確認。
3. TitleUIManagerが選択Dataを保持し、Build Index 1をロード。
4. BattleSceneのGameManager.AwakeがEnemyHealth.Initialize(selectedData)を実行。EnemyHealthはProfile、HP、HealthSliderを設定。
5. PlayerHealth.AwakeはPlayerDataSOのHealthで自身のHPを初期化。EnemyDecisionController.StartはAIコルーチンを開始。
6. PlayerのButton入力はCommandクラスのonClick ListenerからPlayerStateController.StateChangeへ進む。
7. Command ExecuteはUI／Animator／効果音／コルーチンなどを開始。
8. 攻撃はCommandTimingQTEまたは敵Attack AnimationEventからCombatCalculatorへ倍率を渡す。
9. CombatCalculatorがターゲットHealth.TakeDamageへダメージを渡す。ターゲットHealthはCounter／Break判定の後でHPとUIを更新。
10. HP<=0ならHealth.DieからGameManagerへ結果要求。GameManagerが終局フラグを確定しGameUIManagerが結果画面を表示。

処理の呼び出し順はコード上のもの。Scene参照が全て割り当てられ実行可能かは未検証。

## 5. コマンド・敵行動の処理フロー

### Player指撃

`Button.onClick` → `PlayerStateController.StateChange(Attack, command)` → `Command.Attack.Execute()` → 敵Counter抽選・連続攻撃履歴登録・ボタン無効化・QTE表示 → `CommandTimingQTE.OnPointerDown()` → `TimingBarController.CheckTiming()` → `CombatCalculator.PlayerToEnemyAttack()` → `EnemyHealth.TakeDamage()` → Counter／Break判定 → HP、UI、Hit／Die → QTE終了時Idle（Break中の場合はBreak状態を維持）。

### Player Counter／敵攻撃

Player Counter Button → PlayerStateControllerをCounterへ → 敵AttackFlowがプロフィール予兆時間後に攻撃を実行 → AnimationEventの`DecideDamageAmount()`（未到達なら状態期限で補完） → `CombatCalculator.EnemyToPlayerAttack()` → `PlayerHealth.TakeDamage()` → Player Counter状態ならCounterSuccessが25%Strength反撃を敵へ送り、元ダメージを無効化。

### Enemy AI

`EnemyDecisionController.DecideNextStateFlow()` → Idle待機（Playerの行動状態に応じ短縮） → Break/Refresh/Idleの状態待ち → `DecideState()` → Attack always eligible、Refresh if health conditions met → weighted draw → Attack時`ShouldUseComboAttack()` → EnemyStateController.StateChange → ICommand.Execute.

### Enemy Refresh

EnemyAction.Refresh.Execute → RefreshFlow → elapsed timeごとにrequestedHealingを計算 → EnemyHealth.Healingが上限適用後実回復量を返す → 実回復分だけremainingHealAmountを減算。被弾でCancelRefreshし残予算は保持。一定時間完了時のみ予算を補充。

## 6. 依存関係と状態所有権

- GameManagerは戦闘全体の終局フラグとPlayerHealth／EnemyHealth参照を持つ。
- PlayerHealth／EnemyHealthがHPを所有し、HealthSliderは表示のみ。
- 各側のStateControllerが自身のBattleStateを所有。
- EnemyProfileSOは敵の調整値を所有、EnemyDecisionControllerは戦闘中の連続攻撃履歴・Counter準備状態を保持。
- CombatCalculatorは計算／振り分けを行うが、HPや勝敗を所有しない。
- GameUIManagerは表示を所有し、戦闘判断そのものはしない。
- Command側は直接static Manager、相手Health、UI等を参照する。データ駆動の一方で、Scene参照とsingletonへの依存がある。
- 終局時はGameManagerのフラグが優先され、Health、StateController、コマンドUIが後続処理を拒否する。

## 7. 実装上の制約と注意点

1. `StatusConst.Defense`と`Speed`は設定データに存在するが、現在のダメージ／待機計算では参照されない。
2. EnemyProfileSOのCounterWeight、GoadWeight、SkillWeightは保存されるが、重み付き候補作成に使われていない。
3. `BattleState`に宣言されたSkill、Goad、BattleClear等の一部は実装されず、enumと実機能の範囲に差がある。
4. `GameManager.Awake()`からEnemyHealth.Initialize()を呼ぶ。EnemyHealthのコンポーネント参照はAwakeで取得し、実行時初期化の準備完了プロトコルはない。Script Execution Order／Editor挙動を確認していない。
5. Enemy攻撃はAnimationEventと時間切れ代替処理の両方を持つ。イベントの欠落・遅延・二重発火や二段攻撃中断はPlay Modeで検証が必要。
6. EnemyDecisionControllerの`WaitForSeconds`はProfile値をStart時にキャッシュする。戦闘中にProfile変更するフローは見つからない。変更を加える場合、キャッシュ更新が必要になる可能性がある。
7. `EnemyDecisionController`は行動選択ごとにListを生成しLINQ Sumを使うが、数秒単位の待機後の処理。Profiler測定なしに性能問題とは断定しない。
8. `EnemyStateController.stateEnter`は値を代入するが読み出されない。Player側とは異なり、使用箇所がないためコンパイラー警告候補。
9. `TimingBarController.CorDecideTimingQTE()`内で`cor_QTE`の値が設定されていないように見える。Button.interactableの制御はあるが、重複開始保護の実効性は要確認。
10. nullチェックがないInspector参照が多数ある。静的YAMLのみではInspector上のMissing参照や再生時エラーを排除できない。
11. 自動テスト／CIは確認できない。実動作はUnity Editorで手動検証が必要。

## 8. スクリプト一覧（41本）

### Core

- `Assets/Scripts/Core/GameManager.cs`
- `Assets/Scripts/Core/GameUIManager.cs`
- `Assets/Scripts/Core/Constants/AudioTag.cs`
- `Assets/Scripts/Core/Constants/BattleStat.cs`
- `Assets/Scripts/Core/Constants/EasingConst.cs`
- `Assets/Scripts/Core/Constants/StatusConst.cs`
- `Assets/Scripts/Core/Interfaces/ICommand.cs`
- `Assets/Scripts/Core/Interfaces/IDamageble.cs`

### Player

- `Assets/Scripts/Player/PlayerHealth.cs`
- `Assets/Scripts/Player/PlayerStateController.cs`
- `Assets/Scripts/Player/Commands/Attack.cs`
- `Assets/Scripts/Player/Commands/Break.cs`
- `Assets/Scripts/Player/Commands/Counter.cs`
- `Assets/Scripts/Player/Commands/CounterSuccess.cs`
- `Assets/Scripts/Player/Commands/Refresh.cs`

### Enemy

- `Assets/Scripts/Enemy/EnemyHealth.cs`
- `Assets/Scripts/Enemy/EnemyStateController.cs`
- `Assets/Scripts/Enemy/AI/Attack.cs`
- `Assets/Scripts/Enemy/AI/Break.cs`
- `Assets/Scripts/Enemy/AI/Counter.cs`
- `Assets/Scripts/Enemy/AI/CounterSuccess.cs`
- `Assets/Scripts/Enemy/AI/EnemyDecisionController.cs`
- `Assets/Scripts/Enemy/AI/Refresh.cs`

### Combat／data／UI／effects

- `Assets/Scripts/Combat/CombatCalculator.cs`
- `Assets/Scripts/Data/ScriptableObjects/AudioListSO.cs`
- `Assets/Scripts/Data/ScriptableObjects/EnemyProfileSO.cs`
- `Assets/Scripts/Data/ScriptableObjects/PlayerDataSO.cs`
- `Assets/Scripts/Data/ScriptableObjects/EnemyBehavior/EenmyDifficultyListSO.cs`
- `Assets/Scripts/UI/CommandTimingQTE.cs`
- `Assets/Scripts/UI/HealthSlider.cs`
- `Assets/Scripts/UI/TakeDamageUI.cs`
- `Assets/Scripts/UI/TimingBar/TimingBarController.cs`
- `Assets/Scripts/UI/TimingBar/TimingText.cs`
- `Assets/Scripts/UI/Back/BackButton.cs`
- `Assets/Scripts/UI/Title/DifficultyButton.cs`
- `Assets/Scripts/UI/Title/SelectDifficultyButton.cs`
- `Assets/Scripts/UI/Title/TitleUIManager.cs`
- `Assets/Scripts/Animation/AnimateAction.cs`
- `Assets/Scripts/CameraShake.cs`
- `Assets/Scripts/EffectEvents.cs`
- `Assets/Scripts/Sound/SoundManager.cs`
