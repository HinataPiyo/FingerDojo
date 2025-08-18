using System.Collections;
using UnityEngine;

/// <summary>
/// プレイヤーの状態を監視し、敵の行動を決定するクラス
/// </summary>
public class EnemyDecisionController : MonoBehaviour
{
    [SerializeField] EnemyDataSO enemyDataSO;
    [SerializeField] EnemyBehaviorProfile behaviorProfile;
    [SerializeField] EnemyStateController enemyState;
    [SerializeField] PlayerStateController playerState;

    bool isDecide;

    void Awake()
    {
        StartCoroutine(DecideNextStateFlow());
    }

    IEnumerator DecideNextStateFlow()
    {
        while (true)
        {
            isDecide = false;
            // アイドル状態に遷移
            enemyState.StatChange(BattleState.Idle);
            yield return new WaitForSeconds(enemyDataSO.IdleTime);

            while (!isDecide)
            {
                DecideState();
                yield return null;
            }
        }
    }

    void DecideState()
    {
        if (enemyState.State == BattleState.Idle)
        {
            var nextAction = behaviorProfile.GetNextAction(playerState);
            enemyState.StatChange(nextAction);
            isDecide = true;
        }
    }
}