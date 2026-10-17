using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineBasicMultiChannelPerlin))]
public class CameraShake : MonoBehaviour
{
    public static CameraShake instance { get; private set; }

    CinemachineBasicMultiChannelPerlin perlin;
    Coroutine cor_PlayShake;

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        perlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    /// <summary>
    /// カメラ振動
    /// </summary>
    public void Shake(float time, float amplitudeGain, float frequencyGain)
    {
        if (cor_PlayShake != null)
        {
            StopCoroutine(cor_PlayShake);
        }

        cor_PlayShake = StartCoroutine(PlayShake(time, amplitudeGain, frequencyGain));
    }

    IEnumerator PlayShake(float time, float amplitudeGain, float frequencyGain)
    {
        float _time = time;
        while (_time > 0)
        {
            SetGain(amplitudeGain, frequencyGain);
            _time -= Time.deltaTime;
            yield return null;
        }

        SetGain(0, 0);
        cor_PlayShake = null;
    }

    void SetGain(float amplitudeGain, float frequencyGain)
    {
        perlin.AmplitudeGain = amplitudeGain;
        perlin.FrequencyGain = frequencyGain;
    }
}
