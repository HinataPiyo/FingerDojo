using TMPro;
using UnityEngine;

namespace EnemyAction
{
    public class Break : MonoBehaviour, ICommand
    {
        [SerializeField] TextMeshProUGUI breakCount_Text;
        EnemyStateController e_StateCtrl;
        EnemyHealth e_Health;
        int nowCounterCount;

        void Awake()
        {
            e_StateCtrl = GetComponent<EnemyStateController>();
            e_Health = GetComponent<EnemyHealth>();
            ResetBreakCountUI();
        }

        public void Execute()
        {
            e_StateCtrl.E_Animator.SetBool("Break", true);
        }

        /// <summary>
        /// 時間経過後の処理
        /// </summary>
        public void DurationEvent()
        {
            ResetBreakCountUI();     // テキストを初期化
            e_StateCtrl.E_Animator.SetBool("Break", false);
            e_StateCtrl.StateChange(BattleState.Idle, null);
        }

        public void CheckBreak()
        {
            if (GameManager.instance.PlayerHealth.P_State == BattleState.Counter)
            {
                if (e_StateCtrl.State == BattleState.Break) return;

                nowCounterCount++;
                breakCount_Text.text = (e_Health.E_Profile.Status.MaxBreakCount - nowCounterCount).ToString();

                // 最大カウント数を超えていたら
                if (e_Health.E_Profile.Status.MaxBreakCount == nowCounterCount)
                {
                    StartBreak();        // Breakする
                    breakCount_Text.text = "0";
                }
            }
        }

        /// <summary>
        /// ブレイク開始
        /// </summary>
        void StartBreak()
        {
            // ステートをブレイク状態にする
            e_StateCtrl.StateChange(BattleState.Break, this);
            nowCounterCount = 0;
        }

        /// <summary>
        /// ブレイク回数のUIを初期化
        /// </summary>
        public void ResetBreakCountUI()
        {
            breakCount_Text.text = e_Health.E_Profile.Status.MaxBreakCount.ToString();
        }
    }
}