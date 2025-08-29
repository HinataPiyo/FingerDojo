using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI関連全てを管理するクラス
/// </summary>
public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance { get; private set; }
    [Header("WorldCanvas"), SerializeField] Transform worldCanvas;

    [SerializeField] Button[] cmd_Buttons;
    [SerializeField] GameObject counterText;

    [Header("Win or Lose Panel")]
    [SerializeField] Canvas win_Lose_Canvas;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    public Transform WorldCanvas => worldCanvas;

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        win_Lose_Canvas.enabled = false;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
    }

    /// <summary>
    /// コマンドボタンを押せないようにする
    /// </summary>
    public void SetCMDInteractable(bool interactable)
    {
        foreach (Button b in cmd_Buttons)
        {
            b.interactable = interactable;
        }
    }

    /// <summary>
    /// カウンターテキストをアクティブ化する
    /// </summary>
    public void ActiveCounterText()
    {
        // もしすでにAcriveなら一度非アクティブにする
        if (counterText.activeSelf == true)
        {
            counterText.SetActive(false);
        }

        counterText.SetActive(true);
    }

    public void GameClear()
    {
        win_Lose_Canvas.enabled = true;
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    public void GameOver()
    {
        win_Lose_Canvas.enabled = true;
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }

}