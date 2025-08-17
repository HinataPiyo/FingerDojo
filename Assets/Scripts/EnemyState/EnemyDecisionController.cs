using System.Collections;
using UnityEngine;

/// <summary>
/// プレイヤーの状態を監視し、敵の行動を決定するクラス
/// </summary>
public class EnemyDecisionController : MonoBehaviour
{
    [SerializeField] EnemyDataSO enemyDataSO;
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
        // プレイヤーが待機状態なら
        if (playerState.State == BattleState.Idle || playerState.State == BattleState.Counter)
        {
            int rand = Random.Range(0, 1);

            if (rand == 0) enemyState.StatChange(BattleState.Attack);
            else if (rand == 1) enemyState.StatChange(BattleState.Refresh);
            else if (rand == 2) enemyState.StatChange(BattleState.Skill);
            else if (rand == 3) enemyState.StatChange(BattleState.Goad);
            isDecide = true;
        }
        else if (playerState.State == BattleState.Attack)
        {
            int rand = Random.Range(0, 2);

            if (rand == 0) enemyState.StatChange(BattleState.Attack);
            else if (rand == 1) enemyState.StatChange(BattleState.Counter);
            isDecide = true;
        }
    }
}