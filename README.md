# 指道場（FingerDojo）

Unity製のタイミング入力型・1対1バトルゲームの試作プロジェクトです。タイトル画面で敵の難易度を選び、戦闘画面で指撃、カウンター、休息を使って敵と戦います。

## 現在の状態

- Unity Editor：6000.0.51f1
- ビルド登録Scene：TitleScene、BattleScene
- 実装をコードとアセットから確認した機能：難易度選択、タイミングQTE、指撃、Counter、休息、Break状態、敵AI、5段階の敵Profile、HP表示、勝敗画面
- 技（Skill）と煽り（Goad）：UI名や状態名は残っていますが、プレイヤー側のコマンド処理は未実装です。過去の技・煽り案はユーザーの想定と異なるため、承認済み仕様ではありません。
- Unity EditorでのPlay Mode、Inspector参照、AnimationEvent、実際の操作性・バランスは未確認です。過去資料のビルド結果を今回の現況調査で再実行した結果として扱っていません。

## ドキュメント

現状の根拠と実装状況は[Docs/README.md](./Docs/README.md)から確認してください。

- [現状総合仕様書](./Docs/CurrentProjectSpecification.md)：シーン、進行、コマンド、敵、難易度、アセット・プロジェクト設定
- [実装構造・コード解説](./Docs/ImplementationStructure.md)：クラス責務、呼び出し経路、状態所有者
- [機能・実装状況一覧](./Docs/ImplementationStatus.md)：機能ごとの状態と未検証項目
- [仕様差分・課題一覧](./Docs/SpecificationDiscrepancies.md)：資料との相違、未実装、実行時確認が必要な点

## 主なフォルダー

- `Assets/Scenes`：タイトルとバトルのScene
- `Assets/Scripts`：ゲームロジック、UI、データ、演出
- `Assets/Prefab`：難易度ボタン、HP UI、ダメージ表示、戦闘エフェクト
- `Assets/ScriptableObject`：プレイヤー値、敵Profile、音声一覧
- `ProjectSettings`、`Packages`：Unity設定と依存パッケージ
