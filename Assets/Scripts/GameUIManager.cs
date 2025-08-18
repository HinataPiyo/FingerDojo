using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI関連全てを管理するクラス
/// </summary>
public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance { get; private set; }
    [Header("WorldCanvas"), SerializeField] Transform worldCanvas;

    [SerializeField] Button[] cmd_Buttons;

    public Transform WorldCanvas => worldCanvas;

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
    /// コマンドボタンを押せないようにする
    /// </summary>
    public void SetCMDInteractable(bool interactable)
    {
        foreach (Button b in cmd_Buttons)
        {
            b.interactable = interactable;
        }
    }


}