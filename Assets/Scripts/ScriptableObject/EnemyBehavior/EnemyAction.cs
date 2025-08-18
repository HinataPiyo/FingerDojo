using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyAction", menuName = "EnemyBehavior/EnemyAction")]
public class EnemyAction : ScriptableObject
{
    [SerializeField] BattleState actionState;
    [SerializeField, Range(1, 100)] int weight = 50;
    [SerializeField] List<PlayerStateCondition> playerStateConditions;

    public BattleState ActionState => actionState;
    public int Weight => weight;

    public bool IsAvailable(PlayerStateController playerStateController)
    {
        foreach (var condition in playerStateConditions)
        {
            // プレイヤーが設定したアクションをしているか確認
            if (condition.IsMet(playerStateController))
            {
                return true;
            }
        }
        return playerStateConditions.Count == 0;
    }
}

[System.Serializable]
public class PlayerStateCondition
{
    [SerializeField] BattleState requiredPlayerState;

    public bool IsMet(PlayerStateController playerStateController)
    {
        return playerStateController.State == requiredPlayerState;
    }
}
