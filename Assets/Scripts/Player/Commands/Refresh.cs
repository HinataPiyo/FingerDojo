using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Command
{
    /// <summary>
    /// 休息コマンドの実行クラス
    /// ButtonUIにアタッチ
    /// </summary>
    public class Refresh : MonoBehaviour, ICommand
    {
        static readonly BattleState buttonState = BattleState.Refresh;
        [SerializeField] Button button;

        [Header("Effect")]
        [SerializeField] GameObject effect_Refresh;

        [Header("Hand Position")]
        [SerializeField] Transform refreshPos;
        PlayerStateController p_StateCtrl;
        PlayerHealth p_Health;
        GameObject refresh_Particle;
        Coroutine cor_Refresh;
        PlayerDataSO data;

        public bool isRefresh { get; private set; }

        void Awake()
        {
            p_StateCtrl = GetComponent<PlayerStateController>();
            button.onClick.AddListener(() => p_StateCtrl.StateChange(buttonState, this));
        }

        void Start()
        {
            p_Health = GetComponent<PlayerHealth>();
            data = p_Health.P_Data;
        }

        /// <summary>
        /// 休息コマンドの実行処理
        /// </summary>
        public void Execute()
        {
            if (cor_Refresh != null)
            {
                StopCoroutine(cor_Refresh);
                cor_Refresh = null;
            }

            p_StateCtrl.P_Animator.SetBool("Refresh", true);
            cor_Refresh = StartCoroutine(RefreshFlow());
            GameUIManager.instance.SetCMDInteractable(false);       // ボタンを押せなくする
            refresh_Particle = Instantiate(effect_Refresh, refreshPos.position, Quaternion.identity);
        }

        /// <summary>
        /// 休息フロー
        /// </summary>
        IEnumerator RefreshFlow()
        {
            isRefresh = true;
            float healPerSecond = data.Status.RefreshAmount / data.Status.RefreshTime;
            float timer = 0f;

            while (timer < data.Status.RefreshTime)
            {
                // 1秒あたりの回復量に基づいて体力を加算します。
                p_Health.Healing(healPerSecond);
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
            p_StateCtrl.P_Animator.SetBool("Refresh", false);
            p_StateCtrl.StateChange(BattleState.Idle, null);
        }
    }
}