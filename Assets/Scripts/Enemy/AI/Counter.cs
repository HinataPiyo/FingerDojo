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
            EnemyDecisionController decisionController = e_Health.DecisionController;
            Attack enemyAttack = GetComponent<Attack>();
            // 難しいでは、攻撃予兆中にプレイヤーが攻撃した場合も反応Counterの抽選対象にする。
            bool isHardTelegraphCounter = e_Health.E_Profile.DifficultyId == DifficultyId.Hard
                && e_StateCtrl.State == BattleState.Attack
                && enemyAttack.IsTelegraphActive;
            bool canForceCounter = e_StateCtrl.State == BattleState.Idle
                || (e_StateCtrl.State == BattleState.Attack
                    && enemyAttack.IsTelegraphActive);
            if (decisionController.IsRepeatedCounterReady && canForceCounter)
            {
                // 連続攻撃を読まれた場合は確率を使わずCounterし、予兆中なら敵の攻撃を取り消す。
                decisionController.ConsumeRepeatedCommandCounter();
                if (e_StateCtrl.State == BattleState.Attack)
                {
                    enemyAttack.CancelTelegraphForCounter();
                }

                StartCounterAttempt(0f);
                return;
            }

            bool canReactToPlayerAttack = e_StateCtrl.State == BattleState.Idle
                || isHardTelegraphCounter;
            // 通常反応はプロフィール確率に従う。Hard予兆中は成立した場合だけ元攻撃を中断する。
            if (canReactToPlayerAttack
                && Random.value < e_Health.E_Profile.ReactiveCounterChance)
            {
                if (isHardTelegraphCounter && !enemyAttack.CancelTelegraphForCounter()) return;

                float delay = Random.Range(
                    e_Health.E_Profile.CounterAttemptDelayMin,
                    e_Health.E_Profile.CounterAttemptDelayMax);
                StartCounterAttempt(delay);
            }
        }

        void StartCounterAttempt(float delay)
        {
            // 新しい入力で抽選が更新された場合、古い待機中Counterを残さない。
            if (cor_Counter != null)
            {
                StopCoroutine(cor_Counter);
                cor_Counter = null;
            }

            cor_Counter = StartCoroutine(CounterFlow(delay));
        }

        /// <summary>
        /// カウンターするか否か
        /// </summary>
        IEnumerator CounterFlow(float startTime)
        {
            while (startTime > 0)
            {
                startTime -= Time.deltaTime;
                yield return null;

                // 待機中にプレイヤーが攻撃をやめたら、Counterへ移らず敵をIdleへ戻す。
                if (GameManager.instance.PlayerHealth.P_State != BattleState.Attack)
                {
                    e_StateCtrl.StateChange(BattleState.Idle, null);
                    yield break;
                }
            }

            // カウンター開始
            e_StateCtrl.StateChange(BattleState.Counter, this);
        }

    }
}