using System.Collections;
using UnityEngine;

namespace EnemyAction
{
    public class Counter : MonoBehaviour, ICommand
    {
        EnemyStateController e_StateCtrl;
        EnemyHealth e_Health;
        Coroutine cor_Counter;

        void Awake()
        {
            e_StateCtrl = GetComponent<EnemyStateController>();
            e_Health = GetComponent<EnemyHealth>();
        }

        public void Execute()
        {
            e_StateCtrl.E_Animator.SetTrigger("Counter");
        }

        /// <summary>
        /// カウンターできるかどうかを判定する
        /// </summary>
        public void CheckCounterAction()
        {
            // 現在のステートが行動中でなければ、カウンターを試みる
            if (e_StateCtrl.State == BattleState.Idle)
            {
                // プロファイルに設定された確率でカウンターを試行
                if (Random.value < e_Health.E_Profile.ReactiveCounterChance)
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
                    e_StateCtrl.StateChange(BattleState.Idle, null);
                    yield break;
                }

                startTime -= Time.deltaTime;
                yield return null;
            }

            // カウンター開始
            e_StateCtrl.StateChange(BattleState.Counter, this);
        }

    }
}