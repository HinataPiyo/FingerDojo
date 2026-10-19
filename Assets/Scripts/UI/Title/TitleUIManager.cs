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
        if (instance != null && instance != this)
        {
            // Scene再読込時は生存中のManagerへ新SceneのCanvas参照を引き継いで複製を破棄する。
            instance.homeCnavas = homeCnavas;
            instance.difficultyCanvas = difficultyCanvas;
            instance.Initialize();
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        Initialize();
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
        // 有効なEnemyProfileがなければ、未設定データのままBattleSceneへ進ませない。
        if (select == null || select.profileSO == null)
        {
            Debug.LogError("Cannot start a battle without a selected enemy difficulty profile.", this);
            return;
        }

        // Scene上のボタン割当とプロフィールの難易度IDの食い違いを遷移前に検出する。
        if (select.profileSO.DifficultyId != select.difficultyId)
        {
            Debug.LogError("The selected difficulty does not match its enemy profile.", this);
            return;
        }

        SelectEnemyData = select;        // 難易度を設定
        SceneManager.LoadScene(1);  // GameSceneに遷移
    }
}