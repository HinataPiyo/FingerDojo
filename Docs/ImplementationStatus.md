# 機能・実装状況一覧

この表の「実装確認済み」はコードまたはアセット上の存在を確認した意味であり、Unity Editorでの実動作確認を意味しない。動作列の「静的確認」はコード経路を読んだだけで、Play Modeテストはしていない。

| 機能 | 現状の概要 | 実装状況 | 関連ファイル／主な型 | 動作確認 | 既知の問題・未解決 |
|---|---|---|---|---|---|
| タイトル／難易度画面 | ホームと難易度Canvasを切り替える | 実装確認済み | `TitleUIManager`, `SelectDifficultyButton`, `BackButton` | 静的確認のみ | Canvas参照の再バインド、再入場はEditor確認要 |
| 難易度選択 | 5つのDifficultyId、5つのProfile asset | 実装確認済み | `DifficultyButton`, `EenmyDifficultyListSO`, 5 Profile asset | 静的確認のみ | Scene上の各ボタンData/Profile対応をEditorで確認要 |
| Player初期HP | PlayerDataSOからHealthを読み込む | 実装確認済み | `PlayerHealth.Awake`, `PlayerDataSO.asset` | 静的確認のみ | UI参照と初期表示を実行確認要 |
| Enemy初期化 | 選択プロフィールを適用しHP/UI設定 | 実装確認済み | `GameManager.Awake`, `EnemyHealth.Initialize` | 静的確認のみ | Awake呼出時の参照準備・シーンFallbackを確認要 |
| Player指撃QTE | 位置に基づく倍率で敵へ攻撃 | 実装確認済み | `Command.Attack`, `CommandTimingQTE`, `TimingBarController`, `CombatCalculator` | 静的確認のみ | UIイベント、アニメーション・QTE終了順は未検証 |
| Player Counter | 時間窓中に受けた敵攻撃を反撃へ変換 | 実装確認済み | `Command.Counter`, `Command.CounterSuccess`, `PlayerHealth` | 静的確認のみ | 0.5秒窓とAnimationEventの境界未検証 |
| Player Refresh | 時間経過回復、被弾で中断 | 実装確認済み | `Command.Refresh`, `PlayerHealth.Healing` | 静的確認のみ | 中断時・完了時の正確な回復量は未検証 |
| Player Break | 敵Counter中の被弾回数によりBreak | 部分実装／条件確認済み | `Command.Break`, `PlayerHealth.TakeDamage` | 静的確認のみ | UIカウントと意味のプレイヤー向け説明が未確認 |
| 技 | UI名skillとBattleState.Skillはある | 未実装 | BattleScene `skill` GameObject、`BattleStat` | 未実施 | ICommand実装、効果、費用、使用条件なし。仕様再設計が必要 |
| 煽り | UI名goad、BattleState.Goad、AI重みはある | 未実装 | BattleScene `goad`、`BattleStat`、`EnemyProfileSO` | 未実施 | プレイヤー処理と敵反応なし |
| Enemy weighted AI | Attack／条件付きRefreshを選択 | 実装確認済み | `EnemyDecisionController.DecideState` | 静的確認のみ | 行動結果・停止条件・確率の実行確認なし |
| Enemy reactive Counter | プレイヤーAttackに確率で反応 | 実装確認済み | `EnemyAction.Counter`, `EnemyProfileSO` | 静的確認のみ | 特にHard予兆中キャンセルとCounter遅延を実機確認要 |
| Enemy attack telegraph | 攻撃倍率帯・段番号を表示 | 実装確認済み | `EnemyAction.Attack`, `GameUIManager` | 静的確認のみ | 予兆UIの可読性・イベント連動未検証 |
| Enemy attack damage | AnimationEventを本処理とし、時間切れ補完あり | 部分実装（静的構造） | `EnemyAction.Attack.DecideDamageAmount`, `EnemyStateController` | 未検証 | Animatorのイベント配置／遅延／重複／コンボ再生をEditor確認要 |
| Master反復攻撃Counter | 同じ指撃3回後、次の指撃に確定Counterを用意 | 実装確認済み | `EnemyDecisionController.RegisterPlayerCommand`, `EnemyAction.Counter` | 静的確認のみ | 警告と実際のCounter状態のタイミング未検証 |
| Transcendent二段攻撃 | 半分以下・履歴3以上・確率0.2で2段攻撃 | 実装確認済み | `EnemyDecisionController.ShouldUseComboAttack`, `EnemyAction.Attack` | 静的確認のみ | Animation／段間防御／終局中断は未検証 |
| Enemy Refresh | 回復予算、被弾中断、時間完了時補充 | 実装確認済み | `EnemyAction.Refresh`, `EnemyHealth.Healing` | 静的確認のみ | 低体力条件・中断時の再利用性は未検証 |
| Enemy Break | Player Counter状態中の被弾をカウント | 実装確認済み | `EnemyAction.Break`, `EnemyHealth.TakeDamage` | 静的確認のみ | 実際の表示カウントと攻撃タイミング未検証 |
| 勝敗表示 | 最初の終局要求を維持し結果パネルを表示 | 実装確認済み | `GameManager`, `GameUIManager` | 静的確認のみ | 終局直前のAnimationEvent／コルーチン検証要 |
| 戻る／再挑戦 | Build Index 0へ戻す | 部分実装 | `BackButton`, `TitleUIManager` | 静的確認のみ | 戻り後のCanvas・Profile・全状態の再初期化未検証 |
| ダメージ／HP UI | SliderとDamageTextを更新 | 実装確認済み | `HealthSlider`, `TakeDamageUI`, `PlayerHealth`, `EnemyHealth` | 静的確認のみ | prefab/UI参照と解像度ごとの表示未検証 |
| 音声 | AudioListSOタグからSEを再生 | 実装確認済み | `AudioListSO`, `SoundManager` | 静的確認のみ | BGM配列空。音源参照・出音未確認 |
| カメラ振動 | Cinemachine Perlin強度をコルーチンで設定 | 実装確認済み | `CameraShake` | 静的確認のみ | CameraのNoise参照と連続Hit時動作未確認 |
| 保存／ロード | 保存APIや永続化処理 | 未実装 | 該当処理未確認 | 未実施 | 戦闘をまたぐ進行保存は存在しない |
| Wave／複数敵 | enumの一部名称以外に進行処理なし | 未実装 | `BattleStat`, `GameManager` | 未実施 | 現行は単体敵の単戦闘 |
| 自動テスト／CI | 検査用テスト／CI定義 | 未確認（見つからず） | package manifestにはTest Framework | 未実施 | Package存在だけではTestがある根拠にならない |

詳細なコード呼び出しは[実装構造・コード解説](./ImplementationStructure.md)、資料と実装の相違は[仕様差分・課題一覧](./SpecificationDiscrepancies.md)を参照。
