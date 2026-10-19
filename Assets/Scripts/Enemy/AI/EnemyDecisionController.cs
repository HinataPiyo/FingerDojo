using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EnemyAction;
using UnityEngine;

/// <summary>
/// 敵のAI。行動決定のロジックを管理するクラス。
/// EnemyProfileSOのパラメータに基づいて、状況に応じた行動を決定する。
/// </summary>
public class EnemyDecisionController : MonoBehaviour
{
    EnemyStateController e_StateCtrl;
    EnemyHealth EnemyHealth;

    Attack cmd_attack;
    Counter cmd_counter;
    CounterSuccess cmd_counter_success;
    Break cmd_break;
    Refresh cmd_refresh;

    WaitForSeconds idleDuration;
    WaitForSeconds speedUpIdleDuration;
    WaitUntil notBreak;
    WaitWhile isRefresh;
    WaitUntil isIdle;
    // 同一攻撃コマンドの連続回数。非攻撃コマンドでリセットする。
    int lastAttackCommand = -1;
    int repeatedAttackCount;
    bool repeatedCounterReady;


    void Awake()
    {
        e_StateCtrl = GetComponent<EnemyStateController>();
        EnemyHealth = GetComponent<EnemyHealth>();

        cmd_attack = GetComponent<Attack>();
        cmd_counter = GetComponent<Counter>();
        cmd_counter_success = GetComponent<CounterSuccess>();
        cmd_break = GetComponent<Break>();
        cmd_refresh = GetComponent<Refresh>();
    }

    void Start()
    {
        idleDuration = new WaitForSeconds(EnemyHealth.E_Profile.IdleTime);
        speedUpIdleDuration = new WaitForSeconds(EnemyHealth.E_Profile.IdleTime * EnemyHealth.E_Profile.IdleTimeMultiplier);
        notBreak = new WaitUntil(() => e_StateCtrl.State != BattleState.Break);
        isRefresh = new WaitWhile(() => e_StateCtrl.State == BattleState.Refresh);
        isIdle = new WaitUntil(() => e_StateCtrl.State == BattleState.Idle);

        // AIの行動決定フローを開始する
        StartCoroutine(DecideNextStateFlow());
    }

    /// <summary>
    /// 敵の行動を定期的に決定するためのコルーチン。
    /// </summary>
    IEnumerator DecideNextStateFlow()
    {
        while (true)
        {
            if (DurationSpeedUp())
            {
                yield return speedUpIdleDuration;
            }
            else
            {
                yield return idleDuration;
            }

            yield return notBreak;
            yield return isRefresh;
            yield return isIdle;

            if (GameManager.instance.isGameEnd) yield break;

            DecideState();
        }
    }

    /// <summary>
    /// 実際の行動決定ロジック。
    /// EnemyProfileSOのAIパラメータに基づいて、次に取るべき行動を決定する。
    /// </summary>
    void DecideState()
    {
        if (e_StateCtrl.State != BattleState.Idle) return;

        // --- 実行可能なアクションのリストアップ ---
        var availableActions = new List<BattleState>();
        var weights = new List<int>();

        // 攻撃は常に追加
        availableActions.Add(BattleState.Attack);
        weights.Add(EnemyHealth.E_Profile.AttackWeight);

        // 回復（HPが満タンでなければ選択肢に入れる）
        bool canRefresh = EnemyHealth.GetCurrentHealth < EnemyHealth.E_Profile.Status.Health;
        if (canRefresh && EnemyHealth.E_Profile.RefreshOnlyAtHalfHealth)
        {
            // 高難易度ではHPが半分以下かつ、一回分の回復量を活かせる欠損がある時だけ回復候補にする。
            canRefresh = EnemyHealth.GetCurrentHealth <= EnemyHealth.E_Profile.Status.Health * 0.5f
                && EnemyHealth.E_Profile.Status.Health - EnemyHealth.GetCurrentHealth
                >= EnemyHealth.E_Profile.Status.RefreshAmount;
        }

        if (canRefresh)
        {
            availableActions.Add(BattleState.Refresh);
            weights.Add(EnemyHealth.E_Profile.RefreshWeight);
        }

        // Counterは重み付き行動には含めず、プレイヤー攻撃入力時にCounter側で判定する。
        // 現在の敵AIが重み付きで選ぶ行動はAttackとRefreshのみ。

        // --- 重み付けランダム選択 ---
        if (availableActions.Count > 0)
        {
            // 念のため、回復中ならこの処理を抜ける
            if (e_StateCtrl.State == BattleState.Refresh)
            {
                return;
            }

            BattleState state = GetWeightedRandomAction(availableActions, weights);
            if (state == BattleState.Attack)
            {
                // 攻撃を選んだ時点で一度だけ連携抽選し、攻撃処理へ結果を渡す。
                cmd_attack.ConfigureCombo(ShouldUseComboAttack());
            }

            e_StateCtrl.StateChange(state, GetEnemyAction(state));
        }
    }

    /// <summary>
    /// 与えられたアクションのリストとそれぞれの重みに基づいて、ランダムに行動を1つ選択する。
    /// </summary>
    /// <param name="actions">選択肢となるアクションのリスト</param>
    /// <param name="weights">各アクションに対応する重みのリスト</param>
    /// <returns>選択されたアクション（BattleState）</returns>
    BattleState GetWeightedRandomAction(List<BattleState> actions, List<int> weights)
    {
        int totalWeight = weights.Sum();
        int randomValue = Random.Range(0, totalWeight);

        for (int i = 0; i < actions.Count; i++)
        {
            if (randomValue < weights[i])
            {
                return actions[i];
            }
            randomValue -= weights[i];
        }

        // フォールバック
        return actions[0];
    }

    ICommand GetEnemyAction(BattleState state)
    {
        switch (state)
        {
            case BattleState.Attack:
                return cmd_attack;
            case BattleState.Refresh:
                return cmd_refresh;
            case BattleState.Counter:
                return cmd_counter;
            case BattleState.Counter_Success:
                return cmd_counter_success;
            case BattleState.Break:
                return cmd_break;
            default:
                return null;
        }
    }

    bool DurationSpeedUp()
    {
        return GameManager.instance.PlayerHealth.P_State == BattleState.Attack
            || GameManager.instance.PlayerHealth.P_State == BattleState.Goad
            || GameManager.instance.PlayerHealth.P_State == BattleState.Break;
    }

    bool ShouldUseComboAttack()
    {
        EnemyProfileSO profile = EnemyHealth.E_Profile;
        // 超越者の二段攻撃はHP半分以下・同じ攻撃3回以上・プロフィール抽選の全条件が必要。
        return profile.ComboAttackEnabled
            && EnemyHealth.GetCurrentHealth <= profile.Status.Health * 0.5f
            && repeatedAttackCount >= 3
            && Random.value < profile.ComboAttackChance;
    }

    public void RegisterPlayerCommand(int commandId, bool isAttack)
    {
        if (!EnemyHealth.E_Profile.RepeatedCommandCounter) return;

        if (!isAttack)
        {
            // 行動を変えれば同じ攻撃の連打ではなくなるため、読み状態を解除する。
            lastAttackCommand = -1;
            repeatedAttackCount = 0;
            repeatedCounterReady = false;
            GameUIManager.instance.SetRepeatedAttackWarning(false);
            return;
        }

        if (lastAttackCommand == commandId)
        {
            repeatedAttackCount++;
        }
        else
        {
            lastAttackCommand = commandId;
            repeatedAttackCount = 1;
        }

        if (repeatedAttackCount == 3 && !repeatedCounterReady)
        {
            // 3回目の連続入力で予告を一度だけ出す。以降の強制Counter消費で履歴を初期化する。
            repeatedCounterReady = true;
            GameUIManager.instance.SetRepeatedAttackWarning(true);
        }
    }

    public bool ConsumeRepeatedCommandCounter()
    {
        if (!repeatedCounterReady) return false;

        // Counterを消費したら、次の連打判定を新しい履歴から始める。
        repeatedCounterReady = false;
        repeatedAttackCount = 0;
        lastAttackCommand = -1;
        GameUIManager.instance.SetRepeatedAttackWarning(false);
        return true;
    }

    public bool IsRepeatedCounterReady => repeatedCounterReady;
}
