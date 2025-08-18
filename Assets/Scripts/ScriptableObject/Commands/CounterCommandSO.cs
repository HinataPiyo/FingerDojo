using UnityEngine;

[CreateAssetMenu(fileName = "CounterCommand", menuName = "Commands/CounterCommand")]
public class CounterCommandSO : CommandSO
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
