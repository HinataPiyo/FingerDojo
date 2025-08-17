using UnityEngine;

/// <summary>
/// 敵の手を管理するクラス
/// </summary>
public class PlayerHand : MonoBehaviour, IDamageble
{
    [SerializeField] GameObject taleDamageUI;
    [SerializeField] Transform handPos;

    [SerializeField] PlayerDataSO playerData;
    float health;

    void Awake()
    {
        health = playerData.Status.Health;
    }

    /// <summary>
    /// ダメージの受け取り処理
    /// </summary>
    public void TakeDamage(float damage)
    {
        health -= damage;

        GameObject obj = Instantiate(taleDamageUI, handPos.position, transform.rotation, GameUIManager.instance.WorldCanvas);
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
}