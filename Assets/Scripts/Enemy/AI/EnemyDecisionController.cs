using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 敵のAI。行動決定のロジックを管理するクラス。
/// EnemyProfileSOのパラメータに基づいて、状況に応じた行動を決定する。
/// </summary>
public class EnemyDecisionController : MonoBehaviour
{
    EnemyStateController enemyState;
    EnemyHand enemyHand;

    void Awake()
    {
        enemyState = GetComponent<EnemyStateController>();
        enemyHand = GetComponent<EnemyHand>();
    }

    void Start()
    {
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
            if (GameManager.instance.PlayerHand.P_State == BattleState.Attack
            || GameManager.instance.PlayerHand.P_State == BattleState.Goad
            || GameManager.instance.PlayerHand.P_State == BattleState.Break)
            {
                yield return new WaitForSeconds(enemyHand.E_Profile.IdleTime * 0.35f);
            }
            else
            {
                yield return new WaitForSeconds(enemyHand.E_Profile.IdleTime);
            }

            yield return new WaitUntil(() => enemyState.State != BattleState.Break);
            yield return new WaitWhile(() => enemyState.State == BattleState.Refresh);

            DecideState();
        }
    }

    /// <summary>
    /// 実際の行動決定ロジック。
    /// EnemyProfileSOのAIパラメータに基づいて、次に取るべき行動を決定する。
    /// </summary>
    void DecideState()
    {
        if (enemyState.State != BattleState.Idle) return;
        
        // --- 優先順位1: 緊急回復 ---
        // HPが閾値を下回っている場合、最優先で回復を試みる
        if (enemyHand.GetCurrentHealth / enemyHand.E_Profile.Status.Health <= enemyHand.E_Profile.LowHealthRefreshThreshold)
        {
            enemyState.StateChange(BattleState.Refresh);
            return;
        }

        // --- 実行可能なアクションのリストアップ ---
        var availableActions = new List<BattleState>();
        var weights = new List<int>();

        // 攻撃は常に追加
        availableActions.Add(BattleState.Attack);
        weights.Add(enemyHand.E_Profile.AttackWeight);

        // 回復（HPが満タンでなければ選択肢に入れる）
        if (enemyHand.GetCurrentHealth < enemyHand.E_Profile.Status.Health)
        {
            availableActions.Add(BattleState.Refresh);
            weights.Add(enemyHand.E_Profile.RefreshWeight);
        }

        // 挑発（常に可能とする）
        // availableActions.Add(BattleState.Goad);
        // weights.Add(enemyProfile.GoadWeight);

        // スキル（常に可能とする）
        // availableActions.Add(BattleState.Skill);
        // weights.Add(enemyProfile.SkillWeight);

        // --- 重み付けランダム選択 ---
        if (availableActions.Count > 0)
        {
            enemyState.StateChange(GetWeightedRandomAction(availableActions, weights));
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
}
