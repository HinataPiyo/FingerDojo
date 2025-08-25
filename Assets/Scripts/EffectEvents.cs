using UnityEngine;

/// <summary>
/// ParticleEffect などの再生終了後などのイベントを処理する
/// </summary>
public class EffectEvents : MonoBehaviour
{
    [SerializeField] float delayDestroyTime = 0;

    void Start()
    {
        Destroy(gameObject, delayDestroyTime);
    }
}