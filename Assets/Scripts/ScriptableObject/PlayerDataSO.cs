using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "PlayerDataSO")]
public class PlayerDataSO : ScriptableObject
{
    [SerializeField] StatusConst status;
    [SerializeField] float refreshTime;
    [SerializeField, Tooltip("休息コマンドで回復する総量")] float refreshAmount;
    public StatusConst Status => status;
    public float RefreshTime => refreshTime;
    public float RefreshAmount => refreshAmount;
}