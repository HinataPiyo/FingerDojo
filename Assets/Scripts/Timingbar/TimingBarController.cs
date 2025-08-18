using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimingBarController : MonoBehaviour
{
    [SerializeField] Slider timingSlider;
    [SerializeField] float power = 2f;     // 2=QuadIn, 3=CubicIn など
    static readonly float BarPosDuration = 1f;
    static readonly float DispTimingbarTime = 0.5f;       // QTE決定後バーの表示時間

    float currentPositionX;         // バーの現在の位置 (-1 ~ 1)

    Coroutine cor_QTE;
    float elapsed;
    bool isTurn;
    bool isClick;

    public void CorDecideTimingQTE(System.Action endAction)
    {
        if (cor_QTE != null)
        {
            return;
        }

        StartCoroutine(DecideTimingQTE(endAction));
    }

    void Start()
    {
        Initialize();
        gameObject.SetActive(false);    // 非表示
    }

    void Initialize()
    {
        // 初期位置を設定
        isClick = false;
        elapsed = 0;
        isTurn = false;
        currentPositionX = 0f;  // 初期位置を-1（最左端）に設定
        timingSlider.value = currentPositionX;
    }

    /// <summary>
    /// Activeになった瞬間に毎回呼ばれる初期化処理
    /// </summary>
    void OnEnable()
    {
        Initialize();       // 初期化
    }

    void Update()
    {
        if (isClick) return;
        AdjustBarPosition();    // バーの位置を変更
    }

    /// <summary>
    /// QTEを決定したときの処理
    /// </summary>
    IEnumerator DecideTimingQTE(System.Action endAction)
    {
        isClick = true;

        float elapsed = DispTimingbarTime;
        while (elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            yield return null;
        }

        endAction?.Invoke();
        gameObject.SetActive(false);        // 非表示
        cor_QTE = null;
    }

    /// <summary>
    /// タイミングバーの判定
    /// </summary>
    public float CheckTiming()
    {
        // 攻撃力の強さが決まる
        return currentPositionX + 0.5f;
    }

    /// <summary>
    /// バーの位置を動的に変更
    /// </summary>
    void AdjustBarPosition()
    {
        // 進む
        if (elapsed < BarPosDuration && !isTurn)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / BarPosDuration);
            float eased = EasingConst.EaseInPow(t, power);
            currentPositionX = Mathf.LerpUnclamped(0, 1, eased);
            if (currentPositionX >= 1f)
            {
                elapsed = 0;
                isTurn = true;
            }
        }
        else    // 戻る
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / BarPosDuration);
            float eased = EasingConst.EaseOutPow(t, power);
            currentPositionX = Mathf.LerpUnclamped(1, 0, eased);
            if (currentPositionX <= 0)
            {
                elapsed = 0;
                isTurn = false;
            }
        }

        timingSlider.value = currentPositionX;
    }

}