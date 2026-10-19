# ゲームプレイ仕様（現状記録）

## 文書の位置づけ

ここでは現在のコード・アセットから読み取れるゲーム進行を要約します。正式な将来仕様や未承認案ではありません。各処理の条件・数値と根拠は[現状総合仕様書](./CurrentProjectSpecification.md)、動作確認の有無は[機能・実装状況一覧](./ImplementationStatus.md)を参照してください。

## 現在確認できる進行

1. TitleSceneでホーム画面から難易度選択を開く。
2. DifficultyButtonがProfileと難易度IDを検査し、BattleSceneをロードする。
3. GameManagerが選択Profileで敵を初期化し、Player／Enemyが各自のHPと戦闘状態を持つ。
4. Playerは指撃QTE、Counter、Refreshを入力する。Breakは条件成立時の状態。
5. 敵AIはIdle待機後にAttackまたは条件付きRefreshを選択し、Attackは予兆・ダメージ処理を行う。
6. どちらかのHPが0以下になると最初の終局要求を保持し、勝敗パネルを表示する。

## 確認されたコマンドと境界

- **指撃：** バー位置に基づき`0.5 + 2x`倍率でダメージ。
- **Counter：** 状態時間内に被弾すると反撃に置き換え、受けたダメージを無効化。
- **Refresh：** 時間経過で回復し、被弾で中断。
- **Break：** Counter状態に関係する被弾／攻撃回数の条件で発生する状態。
- **Skill／Goad：** 現行コマンド処理なし。過去の仕様案は未承認。

## 難易度

5つのProfile資産があり、敵HP・Strength・待機・予兆・Counter・Refresh条件・上位難易度の追加ルールを設定する。5段階の実プレイバランスが成立していることは確認されていない。

## 未確認

Unity Editor上でのScene参照、操作入力、AnimationEvent、各難易度のプレイ、敗北・勝利・再戦の一連動作は未確認です。Wave進行、保存データ、複数敵は現行実装で確認できません。
