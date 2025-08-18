using System.Collections;
using UnityEngine;

/// <summary>
/// 敵の手を管理するクラス
/// </summary>
public class PlayerHand : MonoBehaviour, IDamageble
{
    [SerializeField] GameObject taleDamageUI;
    [SerializeField] Transform handPos;

    [Header("ステート管理"), SerializeField] PlayerStateController playerState;

    [Header("データ"), SerializeField] PlayerDataSO playerData;
    [Header("アニメーション"), SerializeField] Animator handAnim;
    [Header("UI"), SerializeField] HealthSlider healthSlider;
    public StatusConst Status => playerData.Status;
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
        playerState.StateChange(BattleState.Hit);
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
        if (playerData.RefreshTime <= 0f)
        {
            playerState.StateChange(BattleState.Idle);
            yield break;
        }

        float healPerSecond = playerData.RefreshAmount / playerData.RefreshTime;
        float timer = 0f;

        while (timer < playerData.RefreshTime)
        {
            // 1秒あたりの回復量に基づいて体力を加算します。
            health += healPerSecond * Time.deltaTime;
            health = Mathf.Clamp(health, 0, playerData.Status.Health);
            healthSlider.RefreshSetHealthUI(health, playerData.Status.Health);

            timer += Time.deltaTime;
            yield return null;
        }

        playerState.StateChange(BattleState.Idle);
    }
}