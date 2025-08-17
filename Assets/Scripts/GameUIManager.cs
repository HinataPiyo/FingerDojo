using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance { get; private set; }
    [Header("WorldCanvas"), SerializeField] Transform worldCanvas;


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
}