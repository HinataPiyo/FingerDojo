# プロジェクト概要

## 基本情報

| 項目 | 確認した内容 |
|---|---|
| 製品名 | FingerDojo（指道場） |
| Unity | 6000.0.51f1 |
| 描画 | Universal Render Pipeline 17.0.4 |
| 主なシーン | TitleScene、BattleScene |
| 戦闘形式 | コードとScene上で確認できる範囲では、プレイヤー対単体敵のタイミング戦闘 |
| 難易度 | Easy、Normal、Hard、Master、Transcendentの5 Profileアセット |
| 現況確認 | コード・アセットの静的調査。Unity Play Mode未確認 |

## ゲームの流れ

タイトルから難易度を選びBattleSceneへ進み、タイミングに応じた指撃、敵の攻撃に合わせるCounter、HPを回復するRefreshを使う。HPが0以下になると勝敗パネルが出る。Wave進行や複数敵の仕組みは確認できない。

## 開発状況

現在のコードとScriptableObjectに、QTE、Player／Enemyの状態管理、敵の重み付き行動、難易度別の敵パラメータ・一部ギミックがある。技・煽りはUIオブジェクト名やBattleState値が残る一方でコマンド処理がなく、未実装として扱う。各Profileのバランス、Scene参照、AnimationEventは未検証。

## 調査対象と制約

今回、既存Markdown、C#スクリプト41本、2つのゲームScene、Prefab 6件、Player／Enemy／Audio ScriptableObject、主要ProjectSettings、Input Actions、Package manifestを確認した。Unity EditorのInspector表示やPlay Modeは操作していないため、YAML上の参照だけでは実行時に正しく動くと断定しない。

詳しい現況は[現状総合仕様書](./CurrentProjectSpecification.md)、作業単位の確認事項は[仕様差分・課題一覧](./SpecificationDiscrepancies.md)を参照。
