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
        if (EnemyHealth.GetCurrentHealth < EnemyHealth.E_Profile.Status.Health)
        {
            availableActions.Add(BattleState.Refresh);
            weights.Add(EnemyHealth.E_Profile.RefreshWeight);
        }

        // カウンターはPlayerが攻撃時、随時判定

        // 挑発（常に可能とする）
        // availableActions.Add(BattleState.Goad);
        // weights.Add(enemyProfile.GoadWeight);

        // スキル（常に可能とする）
        // availableActions.Add(BattleState.Skill);
        // weights.Add(enemyProfile.SkillWeight);

        // --- 重み付けランダム選択 ---
        if (availableActions.Count > 0)
        {
            // 念のため、回復中ならこの処理を抜ける
            if (e_StateCtrl.State == BattleState.Refresh)
            {
                return;
            }

            BattleState state = GetWeightedRandomAction(availableActions, weights);

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
}
