using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataSO", menuName = "EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
    [SerializeField] StatusConst status;
    [SerializeField] float idleTime;        // 何もしない時間
    public StatusConst Status => status;
    public float IdleTime => idleTime;
}