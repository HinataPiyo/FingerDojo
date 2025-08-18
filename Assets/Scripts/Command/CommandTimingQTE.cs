using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// QTE用のボタンを管理するスクリプト
/// </summary>
public class CommandTimingQTE : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] PlayerStateController playerState;
    [SerializeField] TimingBarController timingBarCtrl;
    Button button;

    void Awake()
    {
        button = GetComponent<Button>();

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Activeになるたびに初期化
    /// </summary>
    void OnEnable()
    {
        button.interactable = true;
    }

    /// <summary>
    /// ボタンがDownされた瞬間の処理
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        // ボタンが押せない状態なら何もしない
        if (button.interactable == false) return;

        CombatCalculator.instance.PlayerToEnemyAttack(timingBarCtrl.CheckTiming());
        // タイミングアクション決定時、コルーチン開始
        timingBarCtrl.CorDecideTimingQTE(() => EndTimingQTE());
        button.interactable = false;
    }

    /// <summary>
    /// タイミングアクションが終了したときの処理
    /// </summary>
    void EndTimingQTE()
    {
        gameObject.SetActive(false);
        playerState.StateChange(BattleState.Idle);
    }

}