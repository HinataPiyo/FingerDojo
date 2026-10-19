# 技・煽り・難易度に関する現状記録

## 1. この文書の位置づけ

この文書は旧版の「技・煽り仕様案」を置き換え、現在の実装に関する確認事項を記録するものです。過去に記載された技名、効果、コスト、煽り応答などはユーザーの想定と異なったため、実装前に撤去されました。**過去案は承認済み仕様ではなく、今後の実装目標として扱わないでください。**

現行の機能全体は[現状総合仕様書](./CurrentProjectSpecification.md)、コード上の条件は[実装構造](./ImplementationStructure.md)、未確認事項は[仕様差分・課題一覧](./SpecificationDiscrepancies.md)に記載しています。

## 2. 技と煽り

- `BattleState.Skill`と`BattleState.Goad`のenum値は存在します。
- BattleScene上に`skill`、`goad`と命名されたGameObjectが存在します。
- Player側にSkill／Goadを実行する`ICommand`クラスや効果処理は確認できません。
- 敵Profileの`skillWeight`、`goadWeight`値はシリアライズされていますが、現在のEnemyDecisionControllerの行動抽選には使われません。
- 技の習得、装備、コスト、効果、使用条件、煽り後の敵反応、繰り返し制限は現行コードから確認できません。

## 3. 難易度Profileの現行値

以下は`Assets/ScriptableObject/EnemyProfileSO_*.asset`から読み取った保存値です。調整済み・プレイテスト済みバランスではありません。EnemyProfileSOは5段階で同じ構造を使い、難易度別に設定値と一部条件フラグを与えています。

| 項目 | 易しい | 普通 | 難しい | 達人 | 超越者 |
|---|---:|---:|---:|---:|---:|
| 難易度ID | 0 | 1 | 2 | 3 | 4 |
| HP | 2400 | 3000 | 3300 | 3600 | 3900 |
| Strength | 96 | 120 | 132 | 138 | 144 |
| Defense（現行ダメージ式で未使用） | 20 | 20 | 20 | 20 | 20 |
| Speed（現行処理で未使用） | 1 | 1 | 1 | 1 | 1 |
| IdleTime | 3.6秒 | 3.0秒 | 2.7秒 | 2.4秒 | 2.2秒 |
| IdleTimeMultiplier | 0.5 | 0.5 | 0.5 | 0.5 | 0.5 |
| AttackTelegraphDuration | 1.4秒 | 1.1秒 | 0.9秒 | 0.8秒 | 0.75秒 |
| AttackStateDuration | 0.75秒 | 0.75秒 | 0.75秒 | 0.75秒 | 0.75秒 |
| ReactiveCounterChance | 0.25 | 1.0 | 1.0 | 1.0 | 1.0 |
| CounterAttemptDelayMin～Max | 0.5～1.0秒 | 0～1.0秒 | 0～0.8秒 | 0～0.7秒 | 0～0.6秒 |
| CounterStateDuration | 0.65秒 | 0.50秒 | 0.50秒 | 0.45秒 | 0.40秒 |
| MaxBreakCount／BreakTime | 2／3.0秒 | 3／3.0秒 | 3／3.2秒 | 4／3.5秒 | 4／3.5秒 |
| RefreshAmount／RefreshTime | 100／2秒 | 100／2秒 | 100／2秒 | 100／2秒 | 100／2秒 |
| RefreshOnlyAtHalfHealth | false | false | true | true | true |
| RepeatedCommandCounter | false | false | false | true | true |
| ComboAttackEnabled／Chance | false／0 | false／0 | false／0 | false／0 | true／0.2 |

全ProfileでAttackWeight=80、RefreshWeight=20、CounterWeight=50、GoadWeight=20、SkillWeight=30である。現在のAIで重み付き抽選に使用されるのはAttackWeightとRefreshWeightだけ。CounterはプレイヤーのAttackに反応する個別処理。

## 4. コードで確認できる難易度差

- Easyは敵Health／Strength、待機時間、予兆時間、Counter発生確率がNormalより低／長めに設定される。
- Hard以降は敵HP半分以下で、失われたHPがRefreshAmount以上のときにだけRefreshを行動候補に加える。
- Hardでは敵Attack予兆中のPlayer AttackもReactiveCounter判定対象。成功時は予兆攻撃を中断してCounter試行へ進む。
- Master／Transcendentでは同じPlayer Attack入力が3回続くと読みCounter準備を知らせ、次の攻撃入力にCounterを強制的に試みる。
- Transcendentは敵HPが50%以下かつ攻撃履歴が3以上のとき、次の攻撃で0.2の確率で2段攻撃を選ぶ。
- 攻撃QTEの速度と判定閾値に難易度差を付けるコードは確認できない。

上記の動作がゲームとして期待どおりか、予兆が読めるか、各段階の難易度差が適切かはPlay Mode未確認です。

## 5. 解釈上の注意

過去版には技2種、技共通回数、煽り攻撃誘発、フェイント、敵ギミック連携など実装されていない案がありました。過去案の数字や用語を現在のコード動作と混同しないでください。新しい技・煽り仕様はユーザーの意図を反映した別途の確定が必要です。
