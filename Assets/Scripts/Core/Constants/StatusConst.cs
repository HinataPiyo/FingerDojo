using UnityEngine;

[System.Serializable]
public class StatusConst
{
    [SerializeField] string name;
    [SerializeField] RANK rank;
    [SerializeField] float health;       // 体力
    [SerializeField] float strength;     // 攻撃力
    [SerializeField] float defense;      // 防御力
    [SerializeField] float speed;        // CTの長さに起因
    
    [Header("回復設定")]
    [SerializeField, Tooltip("休息コマンドで回復する総量")] float refreshAmount;
    [SerializeField, Tooltip("休息コマンドで回復する時間")] float refreshTime;

    [Header("Breakの設定")]
    [SerializeField] int maxBreakCount = 3;
    [SerializeField] float breakTime = 3f;

    public string Name => name;
    public RANK Rank => rank;
    public float Health => health;
    public float Strength => strength;
    public float Defense => defense;
    public float Speed => speed;

    public float RefreshTime => refreshTime;
    public float RefreshAmount => refreshAmount;

    public int MaxBreakCount => maxBreakCount;
    public float BreakTime => breakTime;

    public enum RANK
    {
        Apprentice,   // 見習い
        FirstDan,     // 初段
        SecondDan,    // 二段
        ThirdDan,     // 三段
        Master,       // 師範
        Grandmaster,  // 宗師
        FingerSaint   // 指聖
    }
}