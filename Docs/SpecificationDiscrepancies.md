# 仕様差分・課題一覧

本書は現在確認できた不一致、未実装、実行時未確認の記録であり、改修を実施する文書ではない。問題点に対する変更・仕様決定は含めない。関連するコード解説は[実装構造](./ImplementationStructure.md)、現況の詳細は[現状総合仕様](./CurrentProjectSpecification.md)を参照。

## 1. 資料と現行実装の不一致

| ID | 内容と根拠 | 影響／状態 |
|---|---|---|
| D-01 | `CombatSkillsAndDifficultySpec.md`には震脚崩し／受けの構え／煽り等の詳細案が記載される。一方、過去資料自身が「ユーザー想定と異なるため撤去・未承認」と明記し、現在のPlayer CommandsにもSkill/Goad実装がない | 旧案は承認済み機能仕様ではなく、実装着手の根拠にできない |
| D-02 | 過去の`GameplaySpec.md`はPlayerのCounterを「攻撃予兆に合わせて」と要約するが、現在のPlayerStateControllerは固定CounterStateDurationを使い、EnemyAction.Attackの予兆状態との直接連動はない | 成功条件を正確に説明する際に、状態時間と攻撃到達時点の実挙動をEditorで確認する必要 |
| D-03 | 過去資料は一部で「EnemyProfileSO_Normal 1件」「DifficultyButtonのプロフィール対応未確認」と記載する。現在のAssetsにはEasy～Transcendentの5 Profileがあり、enumとProfileデータを確認した | 資料が古い。Scene内ボタンへの全参照割当が正しいかは別の未確認事項 |
| D-04 | 既存資料の多くは英語で、現行コードやアセットの記述と同期していない。READMEおよび仕様・構造・チェックリストの説明を日本語の現況記録へ整理する | 読者が過去案と現在の挙動を混同しやすい |
| D-05 | `BattleState`にSkill、Goad、BattleClear、Refresh_End等が存在するが、その宣言と機能実装が一致しない | enumだけを根拠に実装済み機能と扱えない。Wave/結果状態の実装範囲も誤読のおそれ |
| D-06 | `EnemyProfileSO`はCounterWeight、GoadWeight、SkillWeightを保持するが、`EnemyDecisionController.DecideState()`ではAttack／条件付きRefreshのみが候補 | Inspectorにある数値が実際の選択確率を意味しない。調整時の誤解に注意 |
| D-07 | Player/EnemyのStatusConstにDefenseとSpeedがあるが、現行のCombatCalculatorとAI待機計算で利用が確認できない | Inspector値を変更しても当該ステータス効果は変わらない可能性。現仕様では未使用として記録 |
| D-08 | Input ActionsにはPlayer操作名やControl bindingがあるが、ゲーム用C#スクリプトは生成Actionを参照せずUI Button／Pointer経路を使う | Input Actions上の割当からゲーム操作が機能すると判断できない |
| D-09 | ReleaseChecklist等に過去の`dotnet build`成功が記録されるが、今回の現況調査ではビルドもPlay Modeも実行していない | 過去の検証結果と今回確認した事実を混同しない。現在のコンパイル状態は未検証 |

## 2. 未実装と部分実装

### 未実装を確認したもの

- Playerの技コマンド処理と技データ／使用条件／コスト。
- Playerの煽りコマンド処理と敵の煽り反応。
- Wave／複数敵の生成と進行。
- セーブ／ロード／永続的成長。
- 敵のプロフィール重みを使ったGoad／Skill／Counter選択。現AIの重み選択はAttack／Refreshに限定。

### 部分実装、または用途が限定されるもの

- Skill/Goad UIらしいScene GameObjectとBattleState値は存在するが、対応するPlayer commandは存在しない。
- EnemyProfileには行動重みが複数あるが一部しか消費されない。
- `BattleClear`等の状態値はあるが、現行は敵1体の撃破で即時結果表示。
- 難易度ごとのプロフィール値・AI条件は存在するが、プレイヤー体験としての難易度／公平性／攻略成立はPlaytest前。

## 3. 要確認事項（静的調査では断定不可）

| ID | 項目 | 確認根拠／必要な確認 |
|---|---|---|
| Q-01 | Scene上の全Inspector参照とDifficultyButtonのProfile対応 | Scene YAMLにシリアライズ参照があるが、Editor起動時のMissing参照・実際のクリック結果を未確認 |
| Q-02 | タイトル復帰後の永続TitleUIManagerとCanvas再結合 | TitleUIManagerに再バインド処理あり。戻り／二度目の戦闘をPlay Modeで確認していない |
| Q-03 | Animator／AnimationEvent | 敵攻撃の`DecideDamageAmount`イベント、Player Attackイベント、Trigger名・レイヤーが実際のControllerと一致するか未確認 |
| Q-04 | 0.5秒Player Counterと敵攻撃解決の競合 | 状態時間とAnimationEvent実タイミングの組合せを実行確認していない |
| Q-05 | Hardの攻撃予兆中Counter | 攻撃キャンセル、元のAttack状態時間、遅延Counter開始が同時進行するコード経路。境界挙動はPlay Mode未確認 |
| Q-06 | Transcendent二段攻撃 | 段間0.35秒、2つのAnimationEvent、状態タイムアウト、終局割込みの相互作用を未確認 |
| Q-07 | BreakカウントUI | C#のカウント条件・テキスト更新は確認。初期化順、表示値、リセット挙動は未確認 |
| Q-08 | UI入力 | EventSystem、InputModule、Button.onClick、PointerDownの実際の接続と操作デバイス未確認 |
| Q-09 | 解像度と予兆可読性 | Settingsには720×1280基本値があるが、実行画面のレイアウト／Safe Areaを未確認 |
| Q-10 | ビルド／自動テスト | 今回ビルド実施なし。Test Framework packageはあるがテストファイル・CIは未確認 |
| Q-11 | 対象プラットフォームとリリース範囲 | ProjectSettingsにAndroid設定がある一方、対象プラットフォーム・製品要件は文書化されていない |
| Q-12 | Unity Consoleでの実行時例外 | Editorを操作していないため未確認 |

## 4. コード上の制約・技術的な観察

以下は静的コード上の観察であり、発生した実行時不具合としては確認していない。

1. `TimingBarController.CorDecideTimingQTE()`の`cor_QTE`を設定する処理が見当たらないため、nullによる二重起動判定が機能していない可能性がある。
2. `EnemyStateController.stateEnter`は書き込みがあるが読まれていない。ビルド警告の有無は今回未確認。
3. Scene参照をSerializeFieldから直接使用するクラスが多い。null防御のない参照もあり、Inspector参照不備が実行時例外になる可能性がある。
4. EnemyHealth.Initialize()はGameManager.Awake()から呼ばれ、EnemyHealth側が依存するコンポーネント参照もAwake()で取得する。UnityのAwake実行順や対象Sceneの構成に依存するためEditor検証が必要。
5. Enemy攻撃のダメージはAnimationEventを使い、コルーチン／状態時間でフォールバックする。アニメーションの実時間とProfileの状態時間の整合は実行検証が必要。
6. 終局ガードは複数の所有点に分散している。結果直後の全コールバックをEditorで確認していない。
7. `EnemyDecisionController`は行動候補ListとLINQ集計を行うが、行動間の待機後にだけ実行される。Profiler計測なしに性能問題とは結論しない。

## 5. 今後の調査で必要な確認（変更作業ではない）

- Unity Editorでタイトルから難易度選択、戦闘、勝敗、タイトル復帰、再戦を通しで再生する。
- Scene／PrefabのInspector参照、Buttonイベント、EventSystem、Animator Controller、AnimationEventを確認する。
- 5難易度の選択結果、敵Profileの反映、通常攻撃・Counter・Break・休息・攻撃中断・二段攻撃の境界ケースを記録する。
- 現在の実コンパイル結果とConsole警告を取得し、過去資料の検証記録と分けて保管する。
- 技・煽りはユーザーの意図を確認した新しい仕様ができるまで、旧案を実装目標にしない。
