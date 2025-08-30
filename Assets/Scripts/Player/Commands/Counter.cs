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
            GameUIManager.instance.SetCMDInteractable(false);
            SoundManager.instance.PlaySE("counter_befor");
            p_StateCtrl.P_Animator.SetTrigger("Counter");
        }
    }
}