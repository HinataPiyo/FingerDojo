using UnityEngine;

namespace Command
{
    /// <summary>
    /// カウンターコマンドを管理するクラス
    /// </summary>
    public class CounterSuccess : MonoBehaviour, ICommand
    {
        [Header("Effect")]
        [SerializeField] GameObject effect_Counter_Success;
        PlayerStateController p_StateCtrl;
        PlayerHealth p_Health;

        void Awake()
        {
            p_StateCtrl = GetComponent<PlayerStateController>();
            p_Health = GetComponent<PlayerHealth>();
        }

        public void Execute()
        {
            GameUIManager.instance.ActiveCounterText();
            p_StateCtrl.P_Animator.SetTrigger("Counter_Success");
            SoundManager.instance.PlaySE("counter_success");
            GameManager.instance.StartMomentTime();     // 一瞬止める
            Instantiate(effect_Counter_Success, p_Health.CounterPos.position, Quaternion.identity);      // エフェクトを生成
        }

        public bool CheckCounter()
        {
            // カウンター中だった場合
            if (p_StateCtrl.State == BattleState.Counter)
            {
                CombatCalculator.instance.PlayerToEnemyCounter();
                p_StateCtrl.StateChange(BattleState.Counter_Success, this);
                return true;
            }

            return false;
        }
    }
}