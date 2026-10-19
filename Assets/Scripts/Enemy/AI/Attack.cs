using System.Collections;
using UnityEngine;

namespace EnemyAction
{
    /// <summary>
    /// Enemy attack timing, telegraph, and animation-event damage.
    /// </summary>
    public class Attack : MonoBehaviour, ICommand
    {
        EnemyStateController e_StateCtrl;
        EnemyHealth e_Health;
        Coroutine attackFlow;
        // 各段の強さは予兆を出す時点で決め、ダメージ時に引き直さない。
        float attackRoll;
        bool isTelegraphActive;
        // AnimationEventが届くまでダメージを保留し、欠落時は攻撃時間終了で補完する。
        bool damagePending;
        // 複数の終了経路から呼ばれても、攻撃全体の終了処理を一度に保つ。
        bool actionResolved;
        bool comboAttack;
        // AnimationEventと時間切れの両方が来ても、同じ段のダメージを重複適用しない。
        bool stageDamageResolved;
        int currentStage;
        int stageCount = 1;

        public bool IsTelegraphActive => isTelegraphActive;
        public float StateDuration =>
            // Scene上の状態タイマーを、予兆・攻撃・段間待機の合計に合わせる。
            stageCount * (e_Health.E_Profile.AttackTelegraphDuration + e_Health.E_Profile.AttackStateDuration)
            + (stageCount - 1) * 0.35f
            + 0.1f;

        void Awake()
        {
            e_StateCtrl = GetComponent<EnemyStateController>();
            e_Health = GetComponent<EnemyHealth>();
        }

        public void DecideDamageAmount()
        {
            // 攻撃予約がない場合、または解決済みの場合はAnimationEventを無視する。
            if (!damagePending || stageDamageResolved || actionResolved || GameManager.instance.isGameEnd) return;

            damagePending = false;
            stageDamageResolved = true;
            CombatCalculator.instance.EnemyToPlayerAttack(attackRoll);
            if (currentStage == stageCount)
            {
                ResolveAction();
            }
        }

        public void ResolveAtStateEnd()
        {
            // Animatorイベントが届かなかった場合にも、最終フレームで未解決の一撃を処理する。
            DecideDamageAmount();
        }

        public void Execute()
        {
            // 前回の攻撃コルーチンが残っていれば止め、新しい攻撃列だけを実行する。
            if (attackFlow != null)
            {
                StopCoroutine(attackFlow);
            }

            stageCount = comboAttack ? 2 : 1;
            currentStage = 0;
            damagePending = false;
            actionResolved = false;
            isTelegraphActive = false;
            attackFlow = StartCoroutine(AttackFlow());
        }

        public void ConfigureCombo(bool enabled)
        {
            // AIが選んだこの一回の攻撃だけに、二段目を追加する。
            comboAttack = enabled;
        }

        public bool CancelTelegraphForCounter()
        {
            // 敵が攻撃予兆中のときだけ攻撃列を止め、強制Counterに譲る。
            if (e_StateCtrl.State != BattleState.Attack || !isTelegraphActive) return false;

            isTelegraphActive = false;
            damagePending = false;
            if (attackFlow != null)
            {
                StopCoroutine(attackFlow);
                attackFlow = null;
            }

            GameUIManager.instance.ClearEnemyTelegraph();
            ResolveAction();
            return true;
        }

        IEnumerator AttackFlow()
        {
            for (currentStage = 1; currentStage <= stageCount; currentStage++)
            {
                if (currentStage > 1)
                {
                    // 二段目の予兆前に短い間隔を設け、防御入力の機会を作る。
                    float interval = 0.35f;
                    while (interval > 0f)
                    {
                        if (ShouldStopAttackFlow()) yield break;
                        interval -= Time.deltaTime;
                        yield return null;
                    }
                }

                attackRoll = Random.value + Random.Range(0.3f, 0.7f);
                stageDamageResolved = false;
                damagePending = false;
                isTelegraphActive = true;
                string band = GetStrengthBand(attackRoll);
                string stageLabel = stageCount > 1 ? " " + currentStage + "/" + stageCount : string.Empty;
                GameUIManager.instance.ShowEnemyTelegraph(band + stageLabel);

                // 予兆時間は難易度プロフィールで決まり、敵側状態時間と同じ攻撃列で進む。
                float remaining = e_Health.E_Profile.AttackTelegraphDuration;
                while (remaining > 0f)
                {
                    if (ShouldStopAttackFlow()) yield break;
                    remaining -= Time.deltaTime;
                    yield return null;
                }

                isTelegraphActive = false;
                damagePending = true;
                GameUIManager.instance.ClearEnemyTelegraph();
                e_StateCtrl.E_Animator.SetTrigger("Attack");

                // アニメーションイベントが先に来れば即時解決し、来なければ時間切れで補完する。
                float attackRemaining = e_Health.E_Profile.AttackStateDuration;
                while (attackRemaining > 0f && !stageDamageResolved)
                {
                    if (ShouldStopAttackFlow()) yield break;
                    attackRemaining -= Time.deltaTime;
                    yield return null;
                }

                if (!stageDamageResolved)
                {
                    DecideDamageAmount();
                }
            }

            ResolveAction();
            attackFlow = null;
        }

        bool ShouldStopAttackFlow()
        {
            if (!GameManager.instance.isGameEnd && e_StateCtrl.State == BattleState.Attack) return false;

            // 被弾などでAttack状態を離れたら、未解決の後続攻撃を中止する。
            isTelegraphActive = false;
            damagePending = false;
            GameUIManager.instance.ClearEnemyTelegraph();
            ResolveAction();
            attackFlow = null;
            return true;
        }

        void ResolveAction()
        {
            if (actionResolved) return;

            // 複数の終了経路から呼ばれるため、終端処理は一度だけ行う。
            actionResolved = true;
            damagePending = false;
            isTelegraphActive = false;
            GameUIManager.instance.ClearEnemyTelegraph();
        }

        static string GetStrengthBand(float multiplier)
        {
            if (multiplier < 0.75f) return "WEAK";
            if (multiplier < 1.25f) return "MEDIUM";
            return "STRONG";
        }
    }
}
