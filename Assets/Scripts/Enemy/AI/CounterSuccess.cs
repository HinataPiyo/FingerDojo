using UnityEngine;

namespace EnemyAction
{
    public class CounterSuccess : MonoBehaviour, ICommand
    {
        [SerializeField] Transform counterPos;
        [SerializeField] GameObject effect_Counter_Success;
        EnemyStateController e_StateCtrl;

        void Awake()
        {
            e_StateCtrl = GetComponent<EnemyStateController>();
        }

        public void Execute()
        {
            Debug.Log("確認");
            e_StateCtrl.E_Animator.SetTrigger("Counter_Success");
            GameUIManager.instance.ActiveCounterText();     // カウンターテキストをアクティブ化
            SoundManager.instance.PlaySE("counter_success");
            Instantiate(effect_Counter_Success, counterPos.position, Quaternion.identity);      // エフェクトを生成

            GameManager.instance.StartMomentTime();     // 一瞬止める
        }

        public bool CheckCounter()
        {
            // カウンター中だった場合
            // 現 : カウンターは「指撃」ボタンが押された時に判定がとられる
            if (e_StateCtrl.State == BattleState.Counter)
            {
                CombatCalculator.instance.EnemyToPlayerCounter();
                e_StateCtrl.StateChange(BattleState.Counter_Success, this);
                return true;
            }

            return false;
        }
    }
}