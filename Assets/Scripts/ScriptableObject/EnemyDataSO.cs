using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    [SerializeField] StatusConst status;
    public StatusConst Status => status;
}