# アーキテクチャ概要（現状）

## 全体構成

プロジェクトはUnityのMonoBehaviour、Scene上のGameObjectとSerializeField参照を中心とする。プレイヤーと敵に別々の状態・HP・コマンドクラスがあり、敵Profileやプレイヤー値はScriptableObjectから読む。

## 主な責務

- **Core：** `GameManager`が戦闘終局、`GameUIManager`が戦闘UI、`CombatCalculator`がダメージ式とターゲット転送を担当。
- **Player：** `PlayerStateController`がプレイヤー状態、`PlayerHealth`がHP、`Command`配下の各クラスが入力行動を実行。
- **Enemy：** `EnemyStateController`が敵状態、`EnemyHealth`がHP、`EnemyDecisionController`がAI選択、`EnemyAction`配下が選択された行動を実行。
- **Data：** PlayerDataSO、EnemyProfileSO、EenmyDifficultyListSO、AudioListSOが設定値を保持。
- **UI／演出：** QTE、HP表示、ダメージ表示、タイトルUI、音声、カメラ振動、エフェクトを個別クラスで処理。

## 依存と状態所有者

`GameManager`、`GameUIManager`、`CombatCalculator`、`SoundManager`、`CameraShake`はsingleton形式の`instance`を持つ。コマンドとHealthはManager、敵AI、UIを直接参照する。Player／EnemyそれぞれのStateControllerが状態を所有し、HealthコンポーネントがHPを所有する。GameManagerの終局フラグが戦闘全体の終了を所有する。

## 処理の主経路

- Playerボタン → Command → StateController → QTEまたはRefreshコルーチン → CombatCalculator／Health → UIと勝敗判定。
- EnemyDecisionController → EnemyStateController → EnemyAction → AnimationEventまたは状態時間 → CombatCalculator／Health → UIと勝敗判定。
- TitleのDifficultyButton → TitleUIManagerにData保持 → BattleScene → GameManagerからEnemyHealthへProfile設定。

詳しいメソッド単位の流れは[実装構造・コード解説](./ImplementationStructure.md)を参照。

## 実装上の留意点

- 全状態enum値が実装済み機能を表すわけではない。Skill／Goadは実装されていない。
- EnemyProfileの一部重み、Defense、Speedは現在の計算処理で未使用。
- Scene内Inspector参照とAnimator／AnimationEventをEditorで検証していない。
- Enemy AIは重み付き行動Listを行動決定時に作成する。常時毎フレームの処理ではない。
- コード上の存在はPlay Mode確認済みを意味しない。

## 課題と確認状況

資料と実装の差、未実装、Play Modeで確認すべき項目は[仕様差分・課題一覧](./SpecificationDiscrepancies.md)に記録する。本資料では設計変更を提案・実施しない。
