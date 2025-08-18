using UnityEngine;
using UnityEngine.UI;

public class CommandRefresh : MonoBehaviour, ICommand
{
    [SerializeField] PlayerStateController playerState;
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonOnClick);
    }

    /// <summary>
    /// 休息ボタンを押したときの処理
    /// </summary>
    public void ButtonOnClick()
    {
        playerState.StateChange(BattleState.Refresh);
    }
}