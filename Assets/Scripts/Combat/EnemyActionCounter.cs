using UnityEngine;

public class EnemyActionCounter : MonoBehaviour
{
    [SerializeField] EnemyStateController enemyState;
    [SerializeField] EnemyProfileSO enemyProfile;

    /// <summary>
    /// カウンターできるかどうかを判定する
    /// </summary>
    public bool CheckCounterAction()
    {   
        // 現在のステートが行動中でなければ、カウンターを試みる
        if (enemyState.State == BattleState.Idle)
        {
            // プロファイルに設定された確率でカウンターを試行
            if (Random.value < enemyProfile.ReactiveCounterChance)
            {
                Debug.Log("カウンター開始");
                enemyState.StateChange(BattleState.Counter);
                return true;
            }
        }

        return false;
    } 
}