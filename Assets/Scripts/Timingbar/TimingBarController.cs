using UnityEngine;
using UnityEngine.UI;

public class TimingBarController : MonoBehaviour
{
    [SerializeField] RectTransform timingBar;
    [SerializeField] RectTransform perfectArea;
    [SerializeField] RectTransform greatArea;
    [SerializeField] RectTransform goodArea;
    [SerializeField] RectTransform background;

    [SerializeField] float barSpeed = 0.5f;    // バーの移動速度（-1 ~ 1の範囲に合わせる）
    [SerializeField] float perfectRange = 0.05f; // Perfect判定の範囲（-1 ~ 1）
    [SerializeField] float greatRange = 0.2f;   // Great判定の範囲（-1 ~ 1）
    [SerializeField] float goodRange = 0.6f;    // Good判定の範囲（-1 ~ 1）

    float currentPositionX;         // バーの現在の位置 (-1 ~ 1)
    float targetPositionX = 0f;     // Perfectの位置 (通常は0に設定)

    bool isClick;

    void Start()
    {
        // 初期位置を設定
        currentPositionX = -1f;  // 初期位置を-1（最左端）に設定
        AdjustPerfectPosition(-0.3f);
    }

    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Space))
        {
            isClick = true;
            CheckTiming();
        }

        if (isClick) return;

        // タイミングバーの位置を-1~1の範囲で移動
        currentPositionX += barSpeed * Time.deltaTime;

        // バーが範囲を超えた場合、反対側にループ
        if (currentPositionX > 1f) currentPositionX = -1f;
        if (currentPositionX < -1f) currentPositionX = 1f;

        // タイミングバーの位置を背景の幅にスケーリングして設定
        // `currentPositionX` を -1 ~ 1 の範囲から、背景の幅に合わせた位置に変換
        float barPositionX = Mathf.Lerp(-background.sizeDelta.x / 2, background.sizeDelta.x / 2, (currentPositionX + 1) / 2);
        timingBar.anchoredPosition = new Vector2(barPositionX, timingBar.anchoredPosition.y);

        // 判定領域の幅を調整
        AdjustAreaWidth(perfectArea, perfectRange);
        AdjustAreaWidth(greatArea, greatRange);
        AdjustAreaWidth(goodArea, goodRange);
    }

    /// <summary>
    /// タイミングバーの判定
    /// </summary>
    public void CheckTiming()
    {
        float distance = Mathf.Abs(currentPositionX - targetPositionX);

        if (distance <= perfectRange)
        {
            Debug.Log("Perfect!");
        }
        else if (distance <= greatRange)
        {
            Debug.Log("Great!");
        }
        else if (distance <= goodRange)
        {
            Debug.Log("Good!");
        }
        else
        {
            Debug.Log("Too early or too late!");
        }
    }


    /// <summary>
    /// 判定領域の幅を動的に変更
    /// </summary>
    private void AdjustAreaWidth(RectTransform area, float range)
    {
        // 判定範囲に基づいて幅を調整（背景の幅を基準に）
        float width = Mathf.Abs(range) * background.sizeDelta.x;  // 背景の幅に基づいて範囲をスケーリング
        area.sizeDelta = new Vector2(width, area.sizeDelta.y);  // 幅を変更
    }

    /// <summary>
    /// 判定範囲の位置を調整
    /// </summary>
    /// <param name="newPosition">Perfect判定の位置を基準</param>
    public void AdjustPerfectPosition(float newPosition)
    {
        // 新しい位置が -1 ~ 1 の範囲に収まるように制限
        targetPositionX = Mathf.Clamp(newPosition, -1f, 1f);

        // 背景の幅を基準にして位置をスケーリング
        float scaledPositionX = Mathf.Lerp(-background.sizeDelta.x / 2, background.sizeDelta.x / 2, (targetPositionX + 1) / 2);

        // 判定領域（Perfect, Great, Good）を背景に合わせて位置を調整
        perfectArea.anchoredPosition = new Vector2(scaledPositionX, perfectArea.anchoredPosition.y);
        greatArea.anchoredPosition = new Vector2(scaledPositionX, greatArea.anchoredPosition.y);
        goodArea.anchoredPosition = new Vector2(scaledPositionX, goodArea.anchoredPosition.y);
    }

    /// <summary>
    /// 判定範囲の動的調整
    /// </summary>
    public void AdjustRange(float newPerfectRange, float newGreatRange, float newGoodRange)
    {
        perfectRange = Mathf.Clamp(newPerfectRange, 0f, 1f);  // -1 ~ 1範囲に調整
        greatRange = Mathf.Clamp(newGreatRange, 0f, 1f);
        goodRange = Mathf.Clamp(newGoodRange, 0f, 1f);

        // 判定領域の幅を調整
        AdjustAreaWidth(perfectArea, perfectRange);
        AdjustAreaWidth(greatArea, greatRange);
        AdjustAreaWidth(goodArea, goodRange);
    }
}