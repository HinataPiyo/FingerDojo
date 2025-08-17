using System.Collections;
using UnityEngine;

/// <summary>
/// 敵の攻撃ステートを管理するクラス
/// </summary>
public class StateAttack : MonoBehaviour
{
    /// <summary>
    /// 与えるダメージ量を確定
    /// </summary>
    /// <returns></returns>
    public void DecideDamageAmount()
    {
        float range = Random.value;
        CombatCalculator.instance.EnemyToPlayerAttack(range + Random.Range(0.3f, 0.7f)) ;        // 攻撃力の強さが決まる
    }
}