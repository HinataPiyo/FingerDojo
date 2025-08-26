using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "PlayerDataSO")]
public class PlayerDataSO : ScriptableObject
{
    [SerializeField] StatusConst status;
    [SerializeField] float attackStateDuration;
    public StatusConst Status => status;
    public float AttackStateDuration => attackStateDuration;
}