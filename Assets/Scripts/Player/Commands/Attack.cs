using UnityEngine;
using UnityEngine.UI;

namespace Command
{
    /// <summary>
    /// Player攻撃コマンドを管理するクラス
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
            GameManager.instance.EnemyHealth.E_Counter.CheckCounterAction();      // 敵がカウンターをするか判定
            // 難易度ギミック用に、今回の入力が連続攻撃履歴へ加算されることを通知する。
            GameManager.instance.EnemyHealth.DecisionController.RegisterPlayerCommand(0, true);
            SoundManager.instance.PlaySE("sigekibutton_click");
            p_StateCtrl.P_Animator.SetBool("Attack", true);
            GameUIManager.instance.SetCMDInteractable(false);

            // タイミングバーを出現
            timingBarCtrl.gameObject.SetActive(true);
            cmd_TimingQTE.gameObject.SetActive(true);
        }
    }
}