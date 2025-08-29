using UnityEngine;

public class TimingText : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// アニメーションが終了したときの処理
    /// </summary>
    public void EndAnimation()
    {
        gameObject.SetActive(false);
    }
}