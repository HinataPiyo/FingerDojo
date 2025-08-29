using UnityEngine;
using UnityEngine.SceneManagement;
public class BackButton : MonoBehaviour
{
    /// <summary>
    /// 難易度選択画面からホーム画面へ遷移
    /// </summary>
    public void DifficultyToHome()
    {
        TitleUIManager.instance.ChangeDifficultyToTitle();
    }

    /// <summary>
    /// バトル画面からホーム画面へ遷移
    /// </summary>
    public void BattleToHome()
    {
        SceneManager.LoadScene(0);
    }
}
