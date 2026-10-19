using System.Collections;
using UnityEngine;

namespace EnemyAction
{

    public class Refresh : MonoBehaviour, ICommand
    {
        [SerializeField] Transform refreshPos;
        [SerializeField] GameObject effect_Refresh;
        EnemyStateController e_StateCtrl;
        EnemyHealth e_Health;

        GameObject refresh_Particle;
        Coroutine cor_Refresh;
        // 中断しても同じ回復量を再度獲得できないよう、今回の回復予算を保持する。
        float remainingHealAmount;

        public bool isRefresh { get; private set; }

        void Awake()
        {
            e_StateCtrl = GetComponent<EnemyStateController>();
            e_Health = GetComponent<EnemyHealth>();
        }

        void Start()
        {
            // EnemyProfileが確定した後に、この戦闘で使える回復量を初期化する。
            remainingHealAmount = e_Health.E_Profile.Status.RefreshAmount;
        }

        public void Execute()
        {
            if (cor_Refresh != null)
            {
                StopCoroutine(cor_Refresh);
                cor_Refresh = null;
            }

            e_StateCtrl.E_Animator.SetBool("Refresh", true);
            cor_Refresh = StartCoroutine(RefreshFlow());
            refresh_Particle = Instantiate(effect_Refresh, refreshPos.position, Quaternion.identity);
        }

        /// <summary>
        /// 休息フロー
        /// </summary>
        IEnumerator RefreshFlow()
        {
            isRefresh = true;

            float healPerSecond = e_Health.E_Profile.Status.RefreshAmount / e_Health.E_Profile.Status.RefreshTime;
            float timer = 0f;

            while (timer < e_Health.E_Profile.Status.RefreshTime
                && remainingHealAmount > 0f
                && !GameManager.instance.isGameEnd)
            {
                // フレーム時間と残り予算の両方で上限をかけ、時間経過に応じて回復させる。
                float elapsed = Mathf.Min(Time.deltaTime, e_Health.E_Profile.Status.RefreshTime - timer);
                float requestedHealing = Mathf.Min(healPerSecond * elapsed, remainingHealAmount);
                // HP上限で実際に増えなかった分は消費扱いにせず、実回復量だけ予算から引く。
                remainingHealAmount -= e_Health.Healing(requestedHealing);

                timer += elapsed;
                yield return null;
            }

            bool completed = !GameManager.instance.isGameEnd;
            RefreshEnd(completed);
            cor_Refresh = null;
        }

        /// <summary>
        /// 休息コマンドをキャンセル
        /// </summary>
        public void CancelRefresh()
        {
            if (cor_Refresh != null)
            {
                // 不完了終了では回復予算を戻さず、残量を次の回復動作へ持ち越す。
                RefreshEnd(false);
                StopCoroutine(cor_Refresh);
                cor_Refresh = null;
            }

            isRefresh = false;
        }
        
        void RefreshEnd(bool completed)
        {
            if (completed)
            {
                // 一回分の時間を最後まで使い切った場合だけ、次の回復用予算を補充する。
                remainingHealAmount = e_Health.E_Profile.Status.RefreshAmount;
            }

            Destroy(refresh_Particle);
            e_StateCtrl.E_Animator.SetBool("Refresh", false);
            e_StateCtrl.StateChange(BattleState.Idle, null);
            isRefresh = false;
        }
    }
}