# 指道場 ドキュメント案内

このフォルダーは、実際にリポジトリで確認した現在の実装・設定と、未実装／未検証の事項を区別して記録します。仕様の提案を現行動作として扱わず、Unity Editorで確認していない動作は未検証と明記します。

## 現状調査の中心資料

1. [現状総合仕様書](./CurrentProjectSpecification.md)：現在のゲーム構成、進行、数値、コマンド、難易度、アセットと設定。
2. [実装構造・コード解説](./ImplementationStructure.md)：41本のC#スクリプトの役割、依存関係、処理の流れ。
3. [機能・実装状況一覧](./ImplementationStatus.md)：機能ごとの実装状態、関連クラス、確認状況。
4. [仕様差分・課題一覧](./SpecificationDiscrepancies.md)：既存資料と実装の不一致、未実装項目、実行時確認が必要な項目。

## 既存資料の役割

- [プロジェクト概要](./ProjectOverview.md)：Unityプロジェクトの識別情報と調査範囲の概要。
- [ゲームプレイ仕様](./GameplaySpec.md)：戦闘の現状を短く参照する入口。詳しい根拠は現状総合仕様書に集約。
- [アーキテクチャ概要](./Architecture.md)：コードの大分類と依存関係の入口。クラス単位の詳細は実装構造を参照。
- [技・煽り・難易度資料](./CombatSkillsAndDifficultySpec.md)：旧技・煽り案が未承認であること、および現行難易度Profileの記録。旧案は実装目標ではない。
- [リリース確認項目](./ReleaseChecklist.md)：現在未検証のUnity Editor確認事項。チェック済みの実行証跡ではない。

## 確実性の表示

- **実装確認済み**：コードまたは保存済みアセットを静的に確認した状態。実動作の保証ではありません。
- **資料記載のみ**：Markdownに記載がある一方、現在の実装では確認できない内容。
- **部分実装**：UI・状態名・一部処理だけが存在するもの。
- **不一致**：資料とコード／アセットの内容が異なるもの。
- **未実装**：関連する機能処理が確認できないもの。
- **要確認**：SceneやPrefabのInspector、Unity Editor、Play Mode等で確認が必要なもの。

## 現時点での注意

- Unityは6000.0.51f1。ビルド登録SceneはTitleSceneとBattleScene。
- 技と煽りの現行実装はない。過去資料の仕様案はユーザーの想定と異なり撤去された案。
- 易しい、普通、難しい、達人、超越者のProfileアセットは確認したが、Editorでボタン参照や実動作を検証していない。
- Unity EditorのPlay Mode、UI接続、AnimationEvent、実際のバランスは未確認。
- 本調査ではコード、Scene、Prefab、ScriptableObject、ProjectSettingsを変更していない。
