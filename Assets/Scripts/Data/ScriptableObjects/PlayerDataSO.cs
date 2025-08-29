using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "PlayerDataSO")]
public class PlayerDataSO : ScriptableObject
{
    [SerializeField] StatusConst status;
    public StatusConst Status => status;
}