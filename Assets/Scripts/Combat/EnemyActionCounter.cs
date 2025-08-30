using System.Collections;
using UnityEngine;

public class EnemyActionCounter : MonoBehaviour
{
    EnemyStateController enemyState;
    EnemyHand enemyHand;
    Coroutine cor_Counter;

    void Awake()
    {
        enemyState = GetComponent<EnemyStateController>();
        enemyHand = GetComponent<EnemyHand>();
    }

    /// <summary>
    /// カウンターできるかどうかを判定する
    /// </summary>
    public void CheckCounterAction()
    {
        // 現在のステートが行動中でなければ、カウンターを試みる
        if (enemyState.State == BattleState.Idle)
        {
            // プロファイルに設定された確率でカウンターを試行
            if (Random.value < enemyHand.E_Profile.ReactiveCounterChance)
            {
                if (cor_Counter != null)
                {
                    StopCoroutine(cor_Counter);
                    cor_Counter = null;
                }

                cor_Counter = StartCoroutine(CounterFlow());
            }
        }
    }

    /// <summary>
    /// カウンターするか否か
    /// </summary>
    IEnumerator CounterFlow()
    {
        float startTime = Random.Range(0f, 1f);
        while (startTime > 0)
        {
            // startTimeより早くにPlayerがAttackしてきたらIdleに戻る
            if (GameManager.instance.PlayerHealth.P_State != BattleState.Attack)
            {
                enemyState.StateChange(BattleState.Idle);
                yield break;
            }
            
            startTime -= Time.deltaTime;
            yield return null;
        }
        
        // カウンター開始
        enemyState.StateChange(BattleState.Counter);
    }

}