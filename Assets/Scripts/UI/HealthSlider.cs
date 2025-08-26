using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthSlider : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI breakCount_Text;
    [SerializeField] Slider healthSlider;
    [SerializeField] Slider beforSlider;

    [SerializeField] float updateWaitTime = 1f;
    Coroutine cor;

    float beforHP;
    WaitForSeconds updateWait;

    void Awake()
    {
        updateWait = new WaitForSeconds(updateWaitTime);
    }

    /// <summary>
    /// スライダーを更新
    /// </summary>
    public void HitSetHealthUI(float now, float max)
    {
        beforHP = healthSlider.value;
        beforSlider.value = beforHP;
        healthSlider.value = now / max;

        if (cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }

        cor = StartCoroutine(HealthSliderMotion());
    }

    public void RefreshSetHealthUI(float now, float max)
    {
        healthSlider.value = now / max;
    }

    /// <summary>
    /// 体力スライダーのアニメーション
    /// </summary>
    IEnumerator HealthSliderMotion()
    {
        yield return updateWait;

        float elapsed = 1;
        while (elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            float val = Mathf.LerpUnclamped(healthSlider.value, beforHP, elapsed);
            beforSlider.value = val;

            yield return null;
        }
    }

    /// <summary>
    /// ブレイク回数を更新
    /// </summary>
    public void SetBreakCount(int count)
    {
        breakCount_Text.text = count.ToString();
    }
}