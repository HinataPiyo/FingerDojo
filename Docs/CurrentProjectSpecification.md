# 指道場 現状総合仕様書

## 1. 文書の目的と確実性

本書は、現在のリポジトリに存在するコード、Unityのシリアライズ済み設定、既存資料から確認できた状態をまとめた現状記録である。ゲームの理想仕様や将来案を定義するものではない。Unity EditorでのPlay Mode確認は実施していないため、静的ファイルで確認できることと実行時に未検証のことを分けて記載する。

記述の状態は次のとおり。

- **実装確認済み：** C#処理またはシリアライズ済みアセットで存在を確認した。動作確認済みを意味しない。
- **資料記載のみ：** Markdown資料にはあるが、現在の実装で確認できない。
- **部分実装：** 一部の構成や処理のみ確認できる。
- **不一致：** 資料、コード、または設定の記述が一致しない。
- **未実装：** 関連コードを確認し、機能処理が存在しない。
- **要確認：** 静的調査だけでは実行時の挙動や設定の有効性を決定できない。

## 2. ゲーム概要

**実装確認済み：** プロジェクト名はFingerDojo（指道場）。タイトル画面で敵難易度を選び、BattleSceneでプレイヤーの手と1体の敵の手が戦う、タイミング入力を中心とする試作ゲームである。実装されている戦闘行動は指撃、カウンター、休息、および条件付きのブレイク状態である。敵HPが0以下になると勝利、プレイヤーHPが0以下になると敗北画面を表示する。

**未実装：** Wave進行、複数敵、技の効果、煽りの効果、セーブ／ロード、恒久成長は確認できない。`BattleState`に`Skill`、`Goad`、`BattleClear`等が宣言されていることだけでは、対応機能が実装済みとはいえない。

## 3. シーンと進行

### 3.1 ビルド登録シーン

| 順番 | シーン | 静的ファイルから確認した役割 |
|---|---|---|
| 0 | `Assets/Scenes/TitleScene/TitleScene.unity` | ホーム画面、難易度選択画面、難易度選択UI、タイトル管理 |
| 1 | `Assets/Scenes/BattleScene/BattleScene.unity` | プレイヤー／敵、戦闘UI、タイミングQTE、勝敗表示、戦闘管理 |

両シーンは`ProjectSettings/EditorBuildSettings.asset`で有効登録されている。シーン名や階層はYAMLから確認したもので、Editor上での表示・操作・参照解決は未確認。

### 3.2 タイトルから戦闘まで

1. ホーム画面の選択ボタンから難易度画面を表示する。
2. `DifficultyButton`が参照する`EenmyDifficultyListSO.Data`を検査し、難易度IDとプロフィールIDが一致するときだけBattleScene（ビルドIndex 1）をロードする。
3. 選択値は`TitleUIManager.SelectEnemyData`に保持される。`TitleUIManager`は`DontDestroyOnLoad`でシーンをまたいで残る。
4. `GameManager.Awake()`から`EnemyHealth.Initialize()`を呼び、選択プロフィールを敵へ設定する。引数がnullの場合はEnemyHealth側のInspectorプロフィールを維持する。
5. プレイヤーHPは`PlayerHealth.Awake()`でPlayerDataSOの最大HPに初期化される。敵HPは選択プロフィールの最大HPに初期化される。
6. 敵AIは`EnemyDecisionController.Start()`から行動決定コルーチンを始める。

ボタンのプロファイル割当とシーン内の参照が正しく解決するか、二戦目が新しい状態で開始するかはPlay Modeで未確認。

### 3.3 戦闘終了と再挑戦

- プレイヤーHPが0以下になると`PlayerHealth.Die()`から`GameManager.GameOver()`へ進む。
- 敵HPが0以下になると`EnemyHealth.Die()`から`GameManager.GameClear()`へ進む。
- `GameManager.isGameEnd`は最初の結果後にtrueとなる。二度目の結果要求は無視され、戦闘ボタンが無効化される。ダメージ、回復、状態遷移も所有クラスで終局後に拒否する。
- 結果UIの戻るボタンはTitleScene（Index 0）をロードする。既存TitleUIManagerは新シーンのCanvas参照を引き継ぐ実装を持つ。
- 再挑戦時の状態リセット、アニメーションやコルーチンの終了、UI参照更新は実機動作未確認。

## 4. 戦闘状態

`BattleState`に宣言されている値は`BattleIntro`、`Idle`、`Attack`、`Counter`、`Counter_Success`、`Hit`、`Refresh`、`Refresh_End`、`Break`、`Skill`、`Goad`、`BattleClear`、`GameOver`。

**実装確認済み：** PlayerStateControllerとEnemyStateControllerはそれぞれローカル状態、状態経過時間、状態変更を管理する。PlayerのIdle時にコマンドボタンが有効になる。敵Attackの時間切れではAnimationEvent欠落時のダメージ解決を補う。各コントローラーの状態切り替えは終局後に拒否される。

**未実装／未使用：** 上記の全enum値が遷移として使われるわけではない。`Skill`と`Goad`の実行処理、`Refresh_End`、Wave進行用`BattleClear`、状態としての`GameOver`は現在の実際の進行フローで確認できない。

## 5. プレイヤーのデータと戦闘値

`Assets/ScriptableObject/PlayerDataSO.asset`で確認できた初期値：

| 項目 | 値 | 現在の利用 |
|---|---:|---|
| 最大HP | 1000 | HP初期化、上限、HP UI |
| Strength | 100 | 攻撃・カウンターのダメージ計算 |
| Defense | 50 | `CombatCalculator`のダメージ式では未使用 |
| Speed | 1 | 現在確認した戦闘計算では未使用 |
| RefreshAmount | 100 | 休息中の合計回復予算 |
| RefreshTime | 2秒 | プレイヤー休息の継続時間 |
| MaxBreakCount | 2 | 敵Counter中にプレイヤーが被弾した際のカウント上限 |
| BreakTime | 3秒 | Break状態からIdleへ戻る時間 |
| HitDuration | 0.25秒 | PlayerStateControllerのHit状態時間 |
| CounterStateDuration | 0.5秒 | プレイヤーCounter状態の継続時間 |

値はScriptableObjectの静的設定。実行中のHPはPlayerHealthのprivate値で、恒久保存やセーブ処理は確認できない。

## 6. プレイヤーコマンド

### 6.1 指撃（Attack）

**実装確認済み：** `Command.Attack`がボタン押下時にPlayerStateControllerをAttackへ遷移させる。敵Counter判定と連続攻撃履歴登録を行い、攻撃アニメーション、効果音、ボタン無効化、TimingBarとCommandTimingQTEを開始する。

QTE入力は`CommandTimingQTE.OnPointerDown()`で受け取り、現在位置`x`から`TimingBarController.CheckTiming()`が倍率`0.5 + 2x`を返す。バーは0から1まで1秒で移動し、1秒で戻る。曲線はInspectorの`power`（シーンでの値は未確認）を使ったEase-in/Ease-out。倍率式の値域はx=0で0.5倍、x=1で2.5倍。

判定表示閾値は`Bad < 0.30`、`Good >= 0.30`、`Great >= 0.50`、`Perfect >= 0.98`。表示名はコード上の文字列・Prefab名に依存する。閾値はダメージ倍率の計算には別途掛けられず、QTE位置による倍率に直接反映される。

ダメージ式：`PlayerStrength × (0.5 + 2x)`。CombatCalculatorではDefenseを差し引かない。敵HP側がCounter状態なら、ダメージの代わりに敵のカウンター成功処理へ分岐する。

### 6.2 カウンター（Counter）

CounterボタンでPlayerStateControllerをCounterへ遷移させ、ボタンを無効化し、効果音とアニメーションを再生する。入力可能時間はPlayerDataSOの0.5秒。敵の攻撃がPlayerHealth.TakeDamageに到達した際、プレイヤーがCounter状態ならPlayer側CounterSuccessが敵へ反撃し、受けた攻撃ダメージを無効化する。

成功時の反撃式は`PlayerStrength × 0.25`（現在の設定では25）。敵がCounter状態のときにプレイヤー指撃を解決した場合は敵CounterSuccess側が`EnemyStrength × 0.25`をプレイヤーへ与える。敵Counter時間は難易度Profileごとに異なる。

**要確認：** 入力窓とAnimationEvent／攻撃予兆の実時間の重なり、Counter失敗時のUI・演出。

### 6.3 休息（Refresh）

Player側Refreshはボタン入力で`BattleState.Refresh`へ入り、コルーチンで`RefreshAmount / RefreshTime`を毎秒量として時間比例で回復する。実際の回復処理は`Healing(healPerSecond)`内でさらに`Time.deltaTime`を掛ける。HPは0から最大HPの範囲にClampされる。2秒完了でRefreshEndがIdleへ戻す。敵から被弾するとCancelRefreshを呼び、途中回復量を維持して休息を終了する。効果中は無防備で、コマンドボタンは無効。

数値上はプレイヤーの上限100・2秒。フレーム端数による実回復合計、被弾キャンセルのフレーム順序は未検証。プレイヤー側は敵Refreshのような残り回復予算の管理を持たない。

### 6.4 Break状態

PlayerHealth.TakeDamageがBreak条件を先に確認する。敵がCounter状態のときに被弾するたびプレイヤー側Breakカウンターを増やし、MaxBreakCount回でPlayerStateControllerをBreakにする（プレイヤー設定は2回）。Break中はボタンを無効化し、3秒後にカウントUIを戻してIdleへ遷移する。

Breakをプレイヤーが選択する独立コマンドではなく、被弾履歴から起きる状態として実装されている。表示数値の更新／0表示はシーン参照とPlay Mode未確認。

### 6.5 技（Skill）と煽り（Goad）

**未実装：** BattleScene YAML上に`skill`と`goad`という名前のUI GameObjectはあるが、Player CommandsにはSkill/GoadのICommand実装がない。`BattleState.Skill`および`BattleState.Goad`は宣言値に留まり、技効果・煽りによる敵反応・コスト・制限はコードから確認できない。敵プロフィールのskillWeight/goadWeightも現在の敵AI選択処理では使われない。

既存`CombatSkillsAndDifficultySpec.md`に書かれた旧技・煽り案は、ユーザーの想定と異なるため実装前に撤去された未承認案であり、現在仕様ではない。

## 7. 敵の実装と行動

### 7.1 行動選択

`EnemyDecisionController`は敵Idle待機後に行動を決定する。通常はプロフィールIdleTime待ち、プレイヤーがAttack／Goad／Break状態なら`IdleTime × IdleTimeMultiplier`待ち（すべての現プロフィールでは倍率0.5）。Break終了、Refresh終了、Idle復帰を待ってから決定する。

現在重み付き選択に入る行動はAttackとRefreshのみ。AttackWeightは80、RefreshWeightは20だが、敵HP最大時はRefreshを除外する。通常（Easy/Normal）はHPが最大未満ならRefreshを候補に含める。Hard/Master/Transcendentでは、HPが最大HPの50%以下かつ失われたHPがRefreshAmount以上のときだけ候補に加える。候補がAttackとRefreshのとき重み比は80:20。

プロフィールにはCounterWeight、GoadWeight、SkillWeightも保存されているが、現在の行動選択メソッドはそれらを参照しない。Counterはプレイヤー攻撃入力への反応処理からのみ開始される。

### 7.2 敵攻撃

EnemyAction.Attackが1段または条件付き2段のAttackFlowを行う。各段は倍率`Random.value + Random.Range(0.3, 0.7)`を予兆開始時に一度抽選し、Weak（倍率<0.75）、Medium（0.75以上1.25未満）、Strong（1.25以上）の帯を通知する。攻撃予兆時間はプロフィール値。予兆終了時にAnimatorのAttack Triggerを設定し、AnimationEventの`DecideDamageAmount()`でダメージを処理する。イベント欠落時にはAttackStateDuration経過で代替処理し、同一段を二度処理しないフラグがある。

敵ダメージ式：`EnemyStrength × attackRoll`。敵ProfileのDefense値はこの式に使われない。

Master/Transcendentでは、同じ指撃入力が3回連続すると`ENEMY READS YOUR ATTACK`等の警告状態を設定し、次の指撃入力に確定Counterを試みる。Player Counter、Refresh、Goadなど非攻撃入力の通知は履歴を解除する。ただし現在のプレイヤー実装で通常使用できる非攻撃ボタンはCounterとRefresh等。Transcendentでは敵HPが50%以下、連続攻撃履歴3以上、プロフィール確率（0.2）の条件が揃った後の攻撃選択時、2段攻撃を抽選する。2段間は0.35秒。

### 7.3 敵CounterとBreak

プレイヤーAttack開始時、EnemyAction.Counter.CheckCounterActionがプロフィールに従ってCounter反応を判定する。敵Idleならすべての難易度で抽選。Hardでは敵Attack予兆中も抽選対象で、成立時に予兆をキャンセルする。確率はEasy 0.25、他の4難易度1.0。待機は各ProfileのCounterAttemptDelayMin～Max。

敵がCounter状態の間にプレイヤー攻撃が解決すると、敵CounterSuccessでプレイヤーへEnemyStrengthの25%を与え、入力した指撃のダメージは適用しない。

EnemyHealth.TakeDamageは敵Break条件をダメージ適用前に確認する。プレイヤーがCounter状態で敵がダメージを受けると敵Breakカウントを増やし、プロフィールMaxBreakCount回で敵をBreak状態にする。敵BreakTime経過後Idleに戻る。これはBreakの加算条件であり、Counterボタン押下そのものだけでBreakするという仕様ではない。

### 7.4 敵Refresh

敵HPが低下しAIがRefreshを選ぶと、プロフィールRefreshAmount/RefreshTimeに基づいて時間経過で回復する。被弾で中断される。残り回復予算は実回復量分だけ消費され、Refreshを時間完了したときだけ予算を補充する。HP上限で実際に増えなかった量は消費しない。

## 8. 5段階難易度の実データ

難易度プロフィール資産の値はプレイテスト済みバランスではなく、現在の初期設定値である。攻撃／回復重みは全段階80/20、RefreshAmount/Timeは100/2秒、Defenseは20、Speedは1、HitDurationは0.25秒、AttackStateDurationは0.75秒。各プロフィールにCounterWeight 50、GoadWeight 20、SkillWeight 30があるが、現行AIは選択に使わない。

| 難易度 | HP | Strength | IdleTime | 予兆秒 | 敵Counter窓 | Reactive確率 | Counter遅延秒 | Break回数／秒 | 半分以下回復限定 | 連続攻撃Counter | 2段攻撃 |
|---|---:|---:|---:|---:|---:|---:|---|---|---|---|---|
| 易しい | 2400 | 96 | 3.6 | 1.40 | 0.65 | 0.25 | 0.5–1.0 | 2／3.0 | いいえ | いいえ | いいえ |
| 普通 | 3000 | 120 | 3.0 | 1.10 | 0.50 | 1.00 | 0–1.0 | 3／3.0 | いいえ | いいえ | いいえ |
| 難しい | 3300 | 132 | 2.7 | 0.90 | 0.50 | 1.00 | 0–0.8 | 3／3.2 | はい | いいえ | いいえ |
| 達人 | 3600 | 138 | 2.4 | 0.80 | 0.45 | 1.00 | 0–0.7 | 4／3.5 | はい | はい | いいえ |
| 超越者 | 3900 | 144 | 2.2 | 0.75 | 0.40 | 1.00 | 0–0.6 | 4／3.5 | はい | はい | はい、0.20抽選 |

全難易度でIdleTimeMultiplierは0.5。プロフィールに明示的な攻撃予兆帯やQTE操作速度の差はない。難易度差の実装は敵数値、待機・予兆・Counter時間、回復条件、反復攻撃対策、二段攻撃に限られる。仕様文書で提案された他ギミック（煽り反応、複数ギミック連携、技固有対応等）は未実装。

## 9. UI、入力、演出

- BattleSceneには`main_attack`、`counter`、`refresh`、`skill`、`goad`という名称のGameObjectがあり、GameUIManagerはボタン配列をまとめて有効／無効化する。シーン上の全Button参照・OnClick接続はPlay Mode未確認。
- QTEのクリックは`IPointerDownHandler`で取得する。タイミング評価はPerfect/Great/Good/BadのText GameObjectを切り替える。
- HealthSliderはHP比率を表示し、被ダメージ時は遅延する前値スライダーを動かす。TakeDamageUIは整数表示（`F0`）。
- `GameUIManager`はCounterText領域を敵攻撃予兆、コマンド通知、連続攻撃警告に共用し、予兆終了後に予約通知を表示する。
- CameraShakeはCinemachine 3の`CinemachineBasicMultiChannelPerlin`を使用する。SE/BGMはSoundManagerとAudioListSOのタグ参照で再生される。AudioListSOには4種のSEタグがあり、BGM配列は空。
- シーン上のUI文言・イベント登録の正しさ、画面解像度ごとの視認性、効果音参照、Animation Eventのタイミングは未確認。

Input System 1.14.0の`InputSystem_Actions.inputactions`にはPlayer/UIアクションマップが保存され、`ProjectSettings/EditorBuildSettings.asset`からInput Actions assetが登録されている。Player mapにはMove、Look、Attack等、UI mapにはNavigate、Submit、Cancel、Point、Click等がある。一方、今回調査したゲームスクリプトには生成Input Actionsの直接呼び出しはなく、戦闘ボタンはUnity UI Button経由である。アセットに操作割当があることを、ゲーム内でキーボード操作が有効な根拠とはしない。`activeInputHandler: 2`は新旧入力システムの両方を有効にする設定値。

## 10. シーン内アセットとデータ管理

### ScriptableObject

- `PlayerDataSO.asset`：プレイヤーのStatusConst。
- `EnemyProfileSO_Easy/Normal/Hard/Master/Transcendent.asset`：敵のStatusConstと難易度別AIパラメータ。
- `AudioListSO.asset`：SEタグとAudioClip参照。BGMは空。
- `EenmyDifficultyListSO`のC#型は存在する。独立した該当`.asset`はAssets内のasset一覧から確認できず、難易度DataがTitleSceneのシリアライズフィールド内に設定されているか、参照解決しているかは別途Editor確認が必要。

### Prefab

確認したPrefabは6件：`DifficultiesButton.prefab`、`HealthUI.prefab`、`DamageText.prefab`、`Particle_Refresh.prefab`、`Particle_Hit.prefab`、`Particle_Counter_Success.prefab`。プレイヤー／敵の手は独立PrefabとしてではなくScene内GameObjectとして配置されているように見える。全参照・アニメーションクリップは未検証。

### 保存・初期化

設定データはScriptableObjectから読み、HP・敵AIの連続攻撃履歴・回復残量等はMonoBehaviour実行時状態で保持される。ファイル保存、PlayerPrefs、セーブ／ロード処理は確認できない。

## 11. Unity、依存関係、設定

- Unity Editor：6000.0.51f1（`ProjectSettings/ProjectVersion.txt`）。
- 主要パッケージ（`Packages/manifest.json`）：URP 17.0.4、Input System 1.14.0、Cinemachine 3.1.4、UGUI 2.0.0、Timeline 1.8.7、Test Framework 1.5.1、2D Feature 2.0.1、Recorder 5.1.2、Visual Scripting 1.9.7、Collab Proxy 2.8.2、Rider 3.0.36、Visual Studio 2.0.23、Multiplayer Center 1.0.0。`packages-lock.json`が依存解決情報を保持する。
- ProjectSettingsの表示基本値は720×1280、製品名FingerDojo、companyName DefaultCompany。プロジェクト設定にはAndroid関連設定が含まれるが、対象リリースプラットフォームは未決定・未確認。
- TagManagerのユーザー定義Tagsは空。LayersにDefault、TransparentFX、Ignore Raycast、Water、UI等。Sorting LayerはDefaultとUI。
- `.gitignore`はUnity標準生成物やビルド、IDE出力などを除外する。生成`.sln`/`.csproj`はignore対象で、Git上のCI設定／自動テストプロジェクトは今回の確認範囲で見つかっていない。
- `.NET build`やUnity Editorの現在の実行検証は今回実施していない。過去資料にあるコンパイル結果を今回の検証結果として扱わない。

## 12. 仕様と実装の区別、未解決事項

| 項目 | 現状 |
|---|---|
| 技・煽りの旧提案 | 資料記載のみ。ユーザー期待と異なり撤去済み。承認仕様ではない |
| 難易度ごとのゲーム意図 | プロフィール値・コード分岐は実装確認済み。プレイ感・攻略難度は未検証 |
| 防御力 | Player/Enemy両方のStatusConstに保存されるが、確認したダメージ式では未使用 |
| EnemyProfileのCounter/Goad/Skill重み | 資産に存在するが、現在のAI選択から未参照 |
| Skill/Goadボタン | Scene YAML上のオブジェクト名は確認。ボタン挙動は未確認、該当プレイヤーコマンド実装なし |
| Break表示とCounter境界 | C#処理は確認、実行中の表示・タイミングは未確認 |
| シーン参照・ボタン割当 | 静的な一部参照を確認、Editorで完全検証していない |
| 2段攻撃・AnimationEvent | C#上の処理あり。実際のアニメーションイベントとPlay Mode挙動未確認 |
| リリース範囲／対象プラットフォーム | 要確認 |

## 13. 参照先

- [実装構造・コード解説](./ImplementationStructure.md)
- [機能・実装状況一覧](./ImplementationStatus.md)
- [仕様差分・課題一覧](./SpecificationDiscrepancies.md)
- [技・煽り・難易度に関する旧資料](./CombatSkillsAndDifficultySpec.md)
- [既存ゲーム進行資料](./GameplaySpec.md)
- [プロジェクト概要](./ProjectOverview.md)
- [アーキテクチャ概要](./Architecture.md)
