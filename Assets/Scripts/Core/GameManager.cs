using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [SerializeField] PlayerHealth playerHand;
    [SerializeField] EnemyHand enemyHand;

    Coroutine cor_MomentTime;

    public PlayerHealth PlayerHealth => playerHand;
    public EnemyHand EnemyHand => enemyHand;

    public bool isGameEnd { get; private set;}


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 敵データの初期化処理
        enemyHand.Initialize(TitleUIManager.instance?.SelectEnemyData);
    }

    public void StartMomentTime()
    {
        if (cor_MomentTime != null)
        {
            StopCoroutine(cor_MomentTime);
            cor_MomentTime = null;
        }

        cor_MomentTime = StartCoroutine(MomentTimeFlow());
    }

    IEnumerator MomentTimeFlow()
    {
        Time.timeScale = 0.1f;
        float elapsed = 0.5f;
        while (elapsed > 0)
        {
            elapsed -= Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = 1.0f;

    }

    /// <summary>
    /// GameOver処理
    /// </summary>
    public void GameOver()
    {
        isGameEnd = true;
        GameUIManager.instance?.GameOver();
    }

    /// <summary>
    /// GameClear処理
    /// </summary>
    public void GameClear()
    {
        isGameEnd = true;
        GameUIManager.instance?.GameClear();
    }
}