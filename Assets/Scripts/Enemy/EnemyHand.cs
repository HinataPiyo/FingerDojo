using System.Collections;
using UnityEngine;

/// <summary>
/// 敵の手を管理するクラス
/// </summary>
public class EnemyHand : MonoBehaviour, IDamageble
{
    [SerializeField] GameObject takeDamageUI;
    [SerializeField] GameObject effect_Hit;
    [SerializeField] Transform handPos;
    [Header("ステート管理"), SerializeField] EnemyStateController enemyState;
    [Header("データ"), SerializeField] EnemyProfileSO enemyProfile;

    [Header("UI")]
    [SerializeField] HealthSlider healthSlider;
    float health;
    Coroutine cor_Refresh;
    public StatusConst Status => enemyProfile.Status;
    public float GetCurrentHealth => health;

    void Awake()
    {
        health = enemyProfile.Status.Health;
        healthSlider.HitSetHealthUI(health, enemyProfile.Status.Health);
    }

    /// <summary>
    /// ダメージの受け取り処理
    /// </summary>
    public void TakeDamage(float damage)
    {
        // カウンター中だった場合
        // 現 : カウンターは「指撃」ボタンが押された時に判定がとられる
        if (enemyState.State == BattleState.Counter)
        {
            CombatCalculator.instance.EnemyToPlayerCounter();
            enemyState.StateChange(BattleState.Counter_Success);
            return;
        }

        health -= damage;
        healthSlider.HitSetHealthUI(health, enemyProfile.Status.Health);

        GameObject obj = Instantiate(takeDamageUI, handPos.position, Quaternion.identity, GameUIManager.instance.WorldCanvas);
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
        if (enemyProfile.Status.RefreshTime <= 0f)
        {
            enemyState.StateChange(BattleState.Idle);
            yield break;
        }

        float healPerSecond = enemyProfile.Status.RefreshAmount / enemyProfile.Status.RefreshTime;
        float timer = 0f;

        while (timer < enemyProfile.Status.RefreshTime)
        {
            // 1秒あたりの回復量に基づいて体力を加算します。
            health += healPerSecond * Time.deltaTime;
            health = Mathf.Clamp(health, 0, enemyProfile.Status.Health);
            healthSlider.RefreshSetHealthUI(health, enemyProfile.Status.Health);

            timer += Time.deltaTime;
            yield return null;
        }

        enemyState.StateChange(BattleState.Idle);
    }
}