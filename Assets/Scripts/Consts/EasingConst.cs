using UnityEngine;

public static class EasingConst
{
    // 0〜1のtを「最初は遅く、徐々に速く」に変換
    public static float EaseInPow(float t, float power = 2f)
    {
        t = Mathf.Clamp01(t);
        return Mathf.Pow(t, power); // power=2で二乗、3で三乗
    }

    public static float EaseOutPow(float t, float power = 2f)
    {
        t = Mathf.Clamp01(t);
        return 1f - Mathf.Pow(1f - t, power);
    }
}