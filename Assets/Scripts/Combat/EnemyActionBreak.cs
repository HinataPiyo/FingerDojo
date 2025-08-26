using UnityEngine;

public class EnemyActionBreak : MonoBehaviour
{
    [SerializeField] HealthSlider healthSlider;
    EnemyStateController enemyStateCtrl;
    EnemyHand enemyHand;
    int nowCounterCount;

    void Awake()
    {
        enemyStateCtrl = GetComponent<EnemyStateController>();
        enemyHand = GetComponent<EnemyHand>();
        ResetBreakCountUI();
    }

    public void CheckBreak()
    {
        // ブレイク中はカウントしない
        if (enemyStateCtrl.State == BattleState.Break) return;

        nowCounterCount++;
        healthSlider.SetBreakCount(enemyHand.E_Profile.Status.MaxBreakCount - nowCounterCount);

        // 最大カウント数を超えていたら
        if (enemyHand.E_Profile.Status.MaxBreakCount == nowCounterCount)
        {
            Break();        // Breakする
        }
    }

    /// <summary>
    /// ブレイク開始
    /// </summary>
    void Break()
    {
        // ステートをブレイク状態にする
        enemyStateCtrl.StateChange(BattleState.Break);
        nowCounterCount = 0;
        healthSlider.SetBreakCount(0);
    }

    /// <summary>
    /// ブレイク回数のUIを初期化
    /// </summary>
    public void ResetBreakCountUI()
    {
        healthSlider.SetBreakCount(enemyHand.E_Profile.Status.MaxBreakCount);
    }
}