using UnityEngine;
using UnityEngine.UI;

public class SelectDifficultyButton : MonoBehaviour
{
    Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ButtonOnClick);
    }

    /// <summary>
    /// タイトルシーンから難易度選択画面へ遷移する処理
    /// </summary>
    void ButtonOnClick()
    {
        TitleUIManager.instance.ChangeTitleToDifficulty();
    }
}
