using UnityEngine;

[CreateAssetMenu(fileName = "AudioListSO", menuName = "AudioListSO")]
public class AudioListSO : ScriptableObject
{
    [SerializeField] AudioTag[] se;
    [SerializeField] AudioTag[] bgm;

    /// <summary>
    /// 設定したタグのSEを返す
    /// </summary>
    public AudioClip GetSE(string tag)
    {
        for (int i = 0; i < se.Length; i++)
        {
            if (se[i].tag == tag)
            {
                return se[i].clip;
            }
        }
        return null;
    }

    /// <summary>
    /// 設定したタグのBGMを返す
    /// </summary>
    public AudioClip GetBGM(string tag)
    {
        for (int i = 0; i < bgm.Length; i++)
        {
            if (bgm[i].tag == tag)
            {
                return bgm[i].clip;
            }
        }
        return null;
    }
}