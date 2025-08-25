using UnityEngine;

/// <summary>
/// ParticleEffect などの再生終了後などのイベントを処理する
/// </summary>
public class EffectEvents : MonoBehaviour
{
    [SerializeField] float delayDestroyTime = 0;
    [SerializeField] bool isAwakeDeth = true;

    void Start()
    {
        if (isAwakeDeth)
        {
            Destroy(gameObject, delayDestroyTime);
        }
    }

    /// <summary>
    /// 任意のタイミングで破棄
    /// </summary>
    public void EventDestroy()
    {
        Destroy(gameObject);
    }
}