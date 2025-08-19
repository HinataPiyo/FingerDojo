using UnityEngine;

/// <summary>
/// Base class for all command ScriptableObjects.
/// </summary>
public abstract class CommandSO : ScriptableObject
{
    [SerializeField] private string commandName;
    [SerializeField] private BattleState targetState;

    public string CommandName => commandName;
    public BattleState TargetState => targetState;

    /// <summary>
    /// Executes the command.
    /// </summary>
    /// <param name="executor">The MonoBehaviour that is executing the command.</param>
    public abstract void Execute(GameObject executor);
}
