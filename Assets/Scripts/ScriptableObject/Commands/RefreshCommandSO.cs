using UnityEngine;

[CreateAssetMenu(fileName = "RefreshCommand", menuName = "Commands/RefreshCommand")]
public class RefreshCommandSO : CommandSO
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
