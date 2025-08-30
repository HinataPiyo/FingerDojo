using UnityEngine;
using UnityEngine.UI;

namespace Command
{
    /// <summary>
    /// 攻撃コマンドを管理するクラス
    /// </summary>
    public class Attack : MonoBehaviour, ICommand
    {
        static readonly BattleState buttonState = BattleState.Attack;
        [SerializeField] Button button;

        [SerializeField] TimingBarController timingBarCtrl;
        [SerializeField] CommandTimingQTE cmd_TimingQTE;

        PlayerStateController p_StateCtrl;

        void Awake()
        {
            p_StateCtrl = GetComponent<PlayerStateController>();
            button.onClick.AddListener(() => p_StateCtrl.StateChange(buttonState, this));
        }

        public void Execute()
        {
            SoundManager.instance.PlaySE("sigekibutton_click");
            p_StateCtrl.P_Animator.SetBool("Attack", true);
            GameUIManager.instance.SetCMDInteractable(false);
            GameManager.instance.EnemyHand.E_Counter.CheckCounterAction();      // 敵がカウンターをするか判定

            // タイミングバーを出現
            timingBarCtrl.gameObject.SetActive(true);
            cmd_TimingQTE.gameObject.SetActive(true);
        }
    }
}