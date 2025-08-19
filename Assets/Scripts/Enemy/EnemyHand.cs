using System.Collections;
using UnityEngine;

/// <summary>
/// 敵の手を管理するクラス
/// </summary>
public class EnemyHand : MonoBehaviour, IDamageble
{
    [SerializeField] GameObject taleDamageUI;
    [SerializeField] Transform handPos;
    [Header("ステート管理"), SerializeField] EnemyStateController enemyState;
    [Header("データ"), SerializeField] EnemyProfileSO enemyProfileSO;

    [Header("UI")]
    [SerializeField] HealthSlider healthSlider;
    float health;
    Coroutine cor_Refresh;
    public StatusConst Status => enemyProfileSO.Status;
    public float GetCurrentHealth() { return health; }

    void Awake()
    {
        health = enemyProfileSO.Status.Health;
        healthSlider.HitSetHealthUI(health, enemyProfileSO.Status.Health);
    }

    /// <summary>
    /// ダメージの受け取り処理
    /// </summary>
    public void TakeDamage(float damage)
    {
        health -= damage;
        healthSlider.HitSetHealthUI(health, enemyProfileSO.Status.Health);

        GameObject obj = Instantiate(taleDamageUI, handPos.position, Quaternion.identity, GameUIManager.instance.WorldCanvas);
        obj.GetComponent<TakeDamageUI>().SetDamageUI(damage);

        if (health <= 0)
        {
            Die();
        }
    }

    public void Hit()
    {

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
        if (enemyProfileSO.Status.RefreshTime <= 0f)
        {
            enemyState.StatChange(BattleState.Idle);
            yield break;
        }

        float healPerSecond = enemyProfileSO.Status.RefreshAmount / enemyProfileSO.Status.RefreshTime;
        float timer = 0f;

        while (timer < enemyProfileSO.Status.RefreshTime)
        {
            // 1秒あたりの回復量に基づいて体力を加算します。
            health += healPerSecond * Time.deltaTime;
            health = Mathf.Clamp(health, 0, enemyProfileSO.Status.Health);
            healthSlider.RefreshSetHealthUI(health, enemyProfileSO.Status.Health);

            timer += Time.deltaTime;
            yield return null;
        }

        enemyState.StatChange(BattleState.Idle);
    }
}