using UnityEngine;

[CreateAssetMenu(fileName = "AttackCommand", menuName = "Commands/AttackCommand")]
public class AttackCommandSO : CommandSO
{
    public override void Execute(GameObject executor)
    {
        PlayerStateController playerState = executor.GetComponent<PlayerStateController>();
        if (playerState != null)
        {
            playerState.StateChange(TargetState);
        }
    }
}
