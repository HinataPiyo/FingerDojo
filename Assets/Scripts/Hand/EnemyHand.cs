using UnityEngine;

/// <summary>
/// 敵の手を管理するクラス
/// </summary>
public class EnemyHand : MonoBehaviour, IDamageble
{
    [SerializeField] GameObject taleDamageUI;
    [SerializeField] Transform handPos;
    [SerializeField] EnemyDataSO enemyData;
    float health;

    void Awake()
    {
        health = enemyData.Status.Health;
    }

    /// <summary>
    /// ダメージの受け取り処理
    /// </summary>
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("health" + health);

        GameObject obj = Instantiate(taleDamageUI, handPos.position, Quaternion.identity, GameUIManager.instance.WorldCanvas);
        obj.GetComponent<TakeDamageUI>().SetDamageUI(damage);

        if(health <= 0)
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
}