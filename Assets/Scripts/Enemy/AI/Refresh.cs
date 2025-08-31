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

        public bool isRefresh { get; private set; }

        void Awake()
        {
            e_StateCtrl = GetComponent<EnemyStateController>();
            e_Health = GetComponent<EnemyHealth>();
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

            while (timer < e_Health.E_Profile.Status.RefreshTime)
            {
                // 1秒あたりの回復量に基づいて体力を加算します。
                e_Health.Healing(healPerSecond);

                timer += Time.deltaTime;
                yield return null;
            }

            RefreshEnd();
            isRefresh = false;
        }

        /// <summary>
        /// 休息コマンドをキャンセル
        /// </summary>
        public void CancelRefresh()
        {
            if (cor_Refresh != null)
            {
                RefreshEnd();
                StopCoroutine(cor_Refresh);
                cor_Refresh = null;
            }

            isRefresh = false;
        }
        
        void RefreshEnd()
        {
            Destroy(refresh_Particle);
            e_StateCtrl.E_Animator.SetBool("Refresh", false);
            e_StateCtrl.StateChange(BattleState.Idle, null);
        }
    }
}