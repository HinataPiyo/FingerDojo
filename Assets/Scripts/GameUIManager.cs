using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance { get; private set; }

    [Header("タイミングアクションバー"), SerializeField]
    GameObject timingBarUI;

    void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}