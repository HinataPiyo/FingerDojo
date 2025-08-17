using UnityEngine;

/// <summary>
/// 計算を集約させるスクリプト
/// </summary>
public class CombatCalculator : MonoBehaviour
{
    public static CombatCalculator instance { get; private set; }

    float enemyHealth;
    float playerHealth;
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
        enemyHealth -= playerStrength * power;
        Debug.Log("health" + enemyHealth);
        Debug.Log("power" + power);
    }
}