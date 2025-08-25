using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [SerializeField] AudioSource source_SE;
    [SerializeField] AudioSource source_BGM;

    [SerializeField] AudioListSO audioList;

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /// <summary>
    /// 効果音再生
    /// </summary>
    /// <param name="tag">効果音のタグ</param>
    public void PlaySE(string tag)
    {
        AudioClip clip = audioList.GetSE(tag);
        if (clip == null)
        {
            Debug.LogWarning("設定されたSEがありません");
            return;
        }

        source_SE.PlayOneShot(clip);
    }

    /// <summary>
    /// BGM再生
    /// </summary>
    /// <param name="tag">効果音のタグ</param>
    public void PlayBGM(string tag)
    {
        AudioClip clip = audioList.GetBGM(tag);
        if (clip == null)
        {
            Debug.LogWarning("設定されたBGMがありません");
            return;
        }

        source_BGM.PlayOneShot(audioList.GetBGM(tag));
    }
}