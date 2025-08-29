using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 各々の難易度ボタン
/// </summary>
public class DifficultyButton : MonoBehaviour
{
    Button button;
    [SerializeField] EenmyDifficultyListSO.Data difficulty;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonOnClick);
    }

    /// <summary>
    /// ボタンが押された時の処理(難易度設定)
    /// </summary>
    void ButtonOnClick()
    {
        TitleUIManager.instance.SetDifficultyMode(difficulty);
    }
}