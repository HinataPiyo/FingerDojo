using System.Collections;
using UnityEngine;

/// <summary>
/// 敵の手を管理するクラス
/// </summary>
public class PlayerHand : MonoBehaviour, IDamageble
{
    [SerializeField] GameObject taleDamageUI;
    [SerializeField] GameObject effect_Hit;
    [Header("手の位置")]
    [SerializeField] Transform handPos;
    [SerializeField] Transform handPos_Top;
    [SerializeField] Transform counterPos;
    [SerializeField] Transform refreshPos;
    [Header("ステート管理"), SerializeField] PlayerStateController playerState;
    [Header("データ"), SerializeField] PlayerDataSO playerData;
    [Header("アニメーション"), SerializeField] Animator handAnim;
    [Header("UI"), SerializeField] HealthSlider healthSlider;

    public PlayerDataSO P_Data => playerData;
    public Transform HandPos => handPos;
    public Transform HandPos_Top => handPos_Top;
    public Transform CounterPos => counterPos;
    public Transform RefreshPos => refreshPos;
    public BattleState P_State => playerState.State;

    float health;
    Coroutine cor_Refresh;

    void Awake()
    {
        health = playerData.Status.Health;
        healthSlider.HitSetHealthUI(health, playerData.Status.Health);
    }

    /// <summary>
    /// ダメージの受け取り処理
    /// </summary>
    public void TakeDamage(float damage)
    {
        // カウンター中だった場合
        if (playerState.State == BattleState.Counter)
        {
            CombatCalculator.instance.PlayerToEnemyCounter();
            playerState.StateChange(BattleState.Counter_Success);
            return;
        }

        health -= damage;
        healthSlider.HitSetHealthUI(health, playerData.Status.Health);

        GameObject obj = Instantiate(taleDamageUI, handPos.position, Quaternion.identity, GameUIManager.instance.WorldCanvas);
        obj.GetComponent<TakeDamageUI>().SetDamageUI(damage);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            Hit();
        }
    }

    public void Hit()
    {
        SoundManager.instance.PlaySE("hit");
        playerState.StateChange(BattleState.Hit);       // ステートチェンジ
        Instantiate(effect_Hit, handPos.position, Quaternion.identity);     // Hitエフェクト
        CameraShake.instance.Shake(0.2f, 1f, 5f);       // カメラシェイク
    }

    public void Die()
    {

    }

    /// <summary>
    /// 休息コマンドの実行処理
    /// </summary>
    public void Refresh()
    {
        if (cor_Refresh != null)
        {
            StopCoroutine(cor_Refresh);
            cor_Refresh = null;
        }

        cor_Refresh = StartCoroutine(RefreshFlow());
    }

    /// <summary>
    /// 休息フロー
    /// </summary>
    IEnumerator RefreshFlow()
    {
        // 回復時間が0以下に設定されている場合は、何もせずに終了します。
        if (playerData.Status.RefreshTime <= 0f)
        {
            playerState.StateChange(BattleState.Idle);
            yield break;
        }

        float healPerSecond = playerData.Status.RefreshAmount / playerData.Status.RefreshTime;
        float timer = 0f;

        while (timer < playerData.Status.RefreshTime)
        {
            // 回復中にダメージを食らうとIdle状態に戻る
            if (playerState.State == BattleState.Hit)
            {
                playerState.StateChange(BattleState.Refresh_End);
                yield break;
            }
            
            // 1秒あたりの回復量に基づいて体力を加算します。
            health += healPerSecond * Time.deltaTime;
            health = Mathf.Clamp(health, 0, playerData.Status.Health);
            healthSlider.RefreshSetHealthUI(health, playerData.Status.Health);

            timer += Time.deltaTime;
            yield return null;
        }

        playerState.StateChange(BattleState.Refresh_End);
    }
}