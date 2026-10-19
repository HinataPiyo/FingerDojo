using UnityEngine;

/// <summary>
/// 計算を集約させるスクリプト
/// </summary>
public class CombatCalculator : MonoBehaviour
{
    public static CombatCalculator instance { get; private set; }
    [SerializeField] EnemyHealth e_Health;
    [SerializeField] PlayerHealth p_Health;


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
        e_Health.TakeDamage(p_Health.P_Data.Status.Strength * power);
    }

    /// <summary>
    /// 敵からプレイヤーにダメージ
    /// </summary>
    /// <param name="power"></param>
    public void EnemyToPlayerAttack(float power)
    {
        p_Health.TakeDamage(e_Health.E_Profile.Status.Strength * power);
    }

    /// <summary>
    /// プイレイヤーから敵にカウンター
    /// </summary>
    public void PlayerToEnemyCounter()
    {
        e_Health.TakeDamage(p_Health.P_Data.Status.Strength * 0.25f);
    }

    /// <summary>
    /// プイレイヤーから敵にカウンター
    /// </summary>
    public void EnemyToPlayerCounter()
    {
        p_Health.TakeDamage(e_Health.E_Profile.Status.Strength * 0.25f);
    }
}