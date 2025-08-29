using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// タイトルシーンの全てのUIを管理するクラス
/// </summary>
public class TitleUIManager : MonoBehaviour
{
    public static TitleUIManager instance { get; private set; }
    [SerializeField] Canvas homeCnavas;
    [SerializeField] Canvas difficultyCanvas;

    public EenmyDifficultyListSO.Data SelectEnemyData { get; private set; }

    void Awake()
    {
        Initialize();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Initialize()
    {
        homeCnavas.enabled = true;
        difficultyCanvas.enabled = false;
    }

    /// <summary>
    /// タイトルシーンから難易度選択画面へ遷移する処理
    /// </summary>
    public void ChangeTitleToDifficulty()
    {
        homeCnavas.enabled = false;
        difficultyCanvas.enabled = true;
    }

    /// <summary>
    /// 難易度選択画面からタイトル画面へ遷移する処理
    /// </summary>
    public void ChangeDifficultyToTitle()
    {
        homeCnavas.enabled = true;
        difficultyCanvas.enabled = false;
    }

    /// <summary>
    /// 難易度を設定する処理
    /// </summary>
    /// <param name="select">難易度</param>
    public void SetDifficultyMode(EenmyDifficultyListSO.Data select)
    {
        SelectEnemyData = select;        // 難易度を設定
        SceneManager.LoadScene(1);  // GameSceneに遷移
    }
}