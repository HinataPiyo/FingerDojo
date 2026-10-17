using UnityEngine;
using UnityEngine.UI;

namespace Command
{
    /// <summary>
    /// カウンターコマンドを管理するクラス
    /// </summary>
    public class Counter : MonoBehaviour, ICommand
    {
        [SerializeField] Button button;
        static readonly BattleState buttonState = BattleState.Counter;
        PlayerStateController p_StateCtrl;

        void Awake()
        {
            p_StateCtrl = GetComponent<PlayerStateController>();
            button.onClick.AddListener(() => p_StateCtrl.StateChange(buttonState, this));
        }

        public void Execute()
        {
            // Counterは攻撃連打ではないため、敵の連続攻撃読み状態を解除する。
            GameManager.instance.EnemyHealth.DecisionController.RegisterPlayerCommand(3, false);
            GameUIManager.instance.SetCMDInteractable(false);
            SoundManager.instance.PlaySE("counter_befor");
            p_StateCtrl.P_Animator.SetTrigger("Counter");
        }
    }
}