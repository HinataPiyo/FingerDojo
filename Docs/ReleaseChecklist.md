# リリース前確認項目

## 現在の確認状況

この一覧は未完了の手動確認項目を整理したものです。Unity Editorの実行確認、ビルド、Console検査をこの調査作業では実施していません。既存資料に記録された過去のビルド結果は、現在の検証記録ではありません。

## Unity Editorで必要な手動確認

- [ ] TitleSceneがエラーなく開き、ホーム画面から難易度選択を開閉できる
- [ ] 5つの難易度ボタンがそれぞれ対応するDifficultyIdとEnemyProfileを選択する
- [ ] 選択されたProfileがBattleSceneのEnemyHealthへ適用され、HPとUIが初期化される
- [ ] Playerの指撃ボタン、QTE入力、ダメージ、判定表示を確認する
- [ ] Player Counterの成功・失敗、持続時間、反撃ダメージを確認する
- [ ] Player Refreshの合計回復量、被弾中断、最大HP上限を確認する
- [ ] Player／Enemy Breakの条件、回数表示、終了後リセットを確認する
- [ ] 敵の予兆から攻撃・被ダメージまでのタイミングを確認する
- [ ] Hardの攻撃予兆中Counter中断／遅延挙動を確認する
- [ ] Master／Transcendentの連続攻撃警告とCounterを確認する
- [ ] Transcendentの二段攻撃で段数表示、防御機会、攻撃停止を確認する
- [ ] Enemy Refreshの中断と回復予算の再利用を確認する
- [ ] 勝利・敗北の両方で結果UI、ボタン無効化、終局後のダメージ停止を確認する
- [ ] TitleSceneへ戻り、難易度画面を再度開いて二戦目を開始できる
- [ ] Consoleの例外、Missing参照、Animator警告、AnimationEventエラーを確認する
- [ ] 必要な解像度、Safe Area、操作デバイスでUIと予兆の可読性を確認する

## 仕様上の未決定事項

- ユーザーが想定する技／煽りの具体仕様。旧案は未承認のため、実装・リリース条件に含めない。
- リリース対象プラットフォーム、ゲームの完成範囲、リリース版のメタデータ。
- 現在の難易度Profile値が意図する難易度曲線と、各難易度の合格条件。

## 現況資料

実装・未実装の一覧は[機能・実装状況一覧](./ImplementationStatus.md)、未検証要素の背景は[仕様差分・課題一覧](./SpecificationDiscrepancies.md)を参照してください。
