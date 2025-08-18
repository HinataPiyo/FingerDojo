using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "EnemyBehaviorProfile", menuName = "EnemyBehavior/EnemyBehaviorProfile")]
public class EnemyBehaviorProfile : ScriptableObject
{
    [SerializeField] List<EnemyAction> actions;

    /// <summary>
    /// 次の行動を決定する（重み付きで計算）
    /// </summary>
    /// <param name="playerStateController"></param>
    public BattleState GetNextAction(PlayerStateController playerStateController)
    {
        var availableActions = actions.Where(a => a.IsAvailable(playerStateController)).ToList();

        if (availableActions.Count == 0)
        {
            Debug.LogWarning("No available actions for the current state.");
            // 利用可能なアクションがない場合はデフォルトでアイドル状態になります
            return BattleState.Idle;
        }

        int totalWeight = availableActions.Sum(a => a.Weight);
        int randomValue = Random.Range(0, totalWeight);

        foreach (var action in availableActions)
        {
            if (randomValue < action.Weight)
            {
                return action.ActionState;
            }
            randomValue -= action.Weight;
        }

        // 重みが正の場合、到達してはならない
        return availableActions.Last().ActionState;
    }
}
