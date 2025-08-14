using UnityEngine;

/// <summary>
/// 計算を集約させるスクリプト
/// </summary>
public class CombatCalculator : MonoBehaviour
{
    public static CombatCalculator instance { get; private set; }

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}