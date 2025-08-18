using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class CommandButton : MonoBehaviour
{
    [SerializeField] CommandSO commandToExecute;

    Button button;
    PlayerStateController playerStateController;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        // Find the PlayerStateController in the scene
        playerStateController = FindAnyObjectByType<PlayerStateController>();
    }

     void OnClick()
    {
        if (commandToExecute != null && playerStateController != null)
        {
            playerStateController.ExecuteCommand(commandToExecute);
        }
    }
}
