using UnityEngine;

/// <summary>
/// 計算を集約させるスクリプト
/// </summary>
public class CombatCalculator : MonoBehaviour
{
    public static CombatCalculator instance { get; private set; }
    [SerializeField] EnemyHand enemyHand;
    float playerStrength = 100;

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlayerToEnemyAttack(float power)
    {
        enemyHand.TakeDamage(playerStrength * power);
        Debug.Log("power" + power);
    }
}