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
        // 未割当のボタンからScene遷移せず、設定ミスを明示する。
        if (difficulty == null || difficulty.profileSO == null)
        {
            Debug.LogError($"Difficulty button '{name}' has no difficulty profile assigned.", this);
            return;
        }

        // ボタン表示の難易度と実際に読み込む敵プロフィールの難易度を一致させる。
        if (difficulty.profileSO.DifficultyId != difficulty.difficultyId)
        {
            Debug.LogError(
                $"Difficulty button '{name}' is configured as {difficulty.difficultyId}, but its profile is {difficulty.profileSO.DifficultyId}.",
                this);
            return;
        }

        TitleUIManager.instance.SetDifficultyMode(difficulty);
    }
}