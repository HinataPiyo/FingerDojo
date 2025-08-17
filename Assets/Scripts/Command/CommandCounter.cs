using UnityEngine;
using UnityEngine.UI;

public class CommandCounter : MonoBehaviour
{
    [SerializeField] PlayerStateController playerState;
    Button button;

    static readonly double counterJudgeTime = 0.5f;
    double elapsed;
    public bool IsCounter { get; private set; }


    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonOnClick);
    }

    void Update()
    {
        if (IsCounter)
        {
            elapsed += Time.deltaTime;

            // カウンター終了
            if (elapsed >= counterJudgeTime)
            {
                IsCounter = false;
                elapsed = 0;
            }
        }
    }

    /// <summary>
    /// カウンターボタンが押された時の処理
    /// </summary>
    void ButtonOnClick()
    {
        IsCounter = true;      // カウンター中
        playerState.StateChange(BattleState.Counter);       // アニメーション開始
    }
}