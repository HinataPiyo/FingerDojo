using UnityEngine;

/// <summary>
/// 計算を集約させるスクリプト
/// </summary>
public class CombatCalculator : MonoBehaviour
{
    public static CombatCalculator instance { get; private set; }
    [SerializeField] EnemyHand enemyHand;
    [SerializeField] PlayerHand playerHand;

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// プレイヤーから敵にダメージ
    /// </summary>
    /// <param name="power"></param>
    public void PlayerToEnemyAttack(float power)
    {
        enemyHand.TakeDamage(playerHand.Status.Strength * power);
    }

    /// <summary>
    /// 敵からプレイヤーにダメージ
    /// </summary>
    /// <param name="power"></param>
    public void EnemyToPlayerAttack(float power)
    {
        playerHand.TakeDamage(enemyHand.Status.Strength * power);
    }
}