using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// コマンド入力の「指撃」の操作を管理するクラス
/// </summary>
public class CommandAttack : MonoBehaviour, ICommand
{
    [SerializeField] Button button;

    void Awake()
    {
        button.onClick.AddListener(ButtonOnClick);
    }

    /// <summary>
    /// ボタンが押された時の処理
    /// </summary>
    public void ButtonOnClick()
    {
        // タイミングバーを出現
    }

    
}