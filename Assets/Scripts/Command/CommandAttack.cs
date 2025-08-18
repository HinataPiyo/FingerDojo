using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// コマンド入力の「指撃」の操作を管理するクラス
/// </summary>
public class CommandAttack : MonoBehaviour, ICommand
{
    [SerializeField] PlayerStateController playerState;
    [SerializeField] TimingBarController timingBarCtrl;
    [SerializeField] CommandTimingQTE cmd_TimingQTE;
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonOnClick);

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 指撃ボタンが押された時の処理
    /// </summary>
    public void ButtonOnClick()
    {
        playerState.StateChange(BattleState.Attack);
        // タイミングバーを出現
        timingBarCtrl.gameObject.SetActive(true);
        cmd_TimingQTE.gameObject.SetActive(true);

        // 自身のボタンは非表示
        gameObject.SetActive(false);
    }
}