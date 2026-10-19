using TMPro;
using UnityEngine;

namespace Command
{
    /// <summary>
    /// 
    /// </summary>
    public class Break : MonoBehaviour, ICommand
    {
        [SerializeField] TextMeshProUGUI breakCount_Text;
        PlayerStateController p_StateCtrl;
        PlayerDataSO data;
        int nowBreakCount;

        void Awake()
        {
            p_StateCtrl = GetComponent<PlayerStateController>();
            data = GetComponent<PlayerHealth>().P_Data;
            ResetBreakCountUI();
        }

        public void Execute()
        {
            GameUIManager.instance.SetCMDInteractable(false);       // UIを触れなくする
            p_StateCtrl.P_Animator.SetBool("Break", true);
        }

        /// <summary>
        /// 時間経過後の処理
        /// </summary>
        public void DurationEvent()
        {
            ResetBreakCountUI();     // テキストを初期化
            p_StateCtrl.P_Animator.SetBool("Break", false);
            p_StateCtrl.StateChange(BattleState.Idle, null);
        }

        /// <summary>
        /// PlayerがBreakするか確認
        /// </summary>
        public void CheckBreak()
        {
            if (GameManager.instance.EnemyHealth.E_State == BattleState.Counter)    // 敵がカウンター状態
            {
                if (p_StateCtrl.State == BattleState.Break) return;

                nowBreakCount++;
                breakCount_Text.text = (data.Status.MaxBreakCount - nowBreakCount).ToString();

                if (data.Status.MaxBreakCount == nowBreakCount)
                {
                    p_StateCtrl.StateChange(BattleState.Break, this);
                    nowBreakCount = 0;
                    breakCount_Text.text = "0";
                }
            }
        }

        /// <summary>
        /// Break回数のUIを初期化
        /// </summary>
        public void ResetBreakCountUI()
        {
            breakCount_Text.text = data.Status.MaxBreakCount.ToString();
        }
    }
}