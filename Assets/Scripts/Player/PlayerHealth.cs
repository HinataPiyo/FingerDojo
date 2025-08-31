using System.Collections;
using Command;
using TMPro;
using UnityEngine;

/// <summary>
/// プレイヤーの体力を管理するクラス
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageble
{
    [Header("Effect")]
    [SerializeField] GameObject taleDamageUI;
    [SerializeField] GameObject effect_Hit;

    [Header("Hand Position")]
    [SerializeField] Transform handPos;
    [SerializeField] Transform counterPos;
    

    [Header("State Control")]
    [SerializeField] PlayerStateController playerState;

    [Header("Data")]
    [SerializeField] PlayerDataSO playerData;

    [Header("UI")]
    [SerializeField] HealthSlider healthSlider;

    Refresh cmd_Refresh;
    Break cmd_break;
    CounterSuccess cmd_CounterSuccess;


    public PlayerDataSO P_Data => playerData;
    public Transform CounterPos => counterPos;
    public BattleState P_State => playerState.State;

    float health;

    void Awake()
    {
        cmd_Refresh = GetComponent<Refresh>();
        cmd_CounterSuccess = GetComponent<CounterSuccess>();
        cmd_break = GetComponent<Break>();

        health = playerData.Status.Health;
        healthSlider.HitSetHealthUI(health, playerData.Status.Health);
    }

    /// <summary>
    /// ダメージの受け取り処理
    /// </summary>
    public void TakeDamage(float damage)
    {
        cmd_break.CheckBreak();     // Breakできるか判定
        bool isCounter = cmd_CounterSuccess.CheckCounter();         // カウンター状態を更新する
        if (isCounter) return;

        health -= damage;
        healthSlider.HitSetHealthUI(health, playerData.Status.Health);

        GameObject obj = Instantiate(taleDamageUI, handPos.position, Quaternion.identity, GameUIManager.instance.WorldCanvas);
        obj.GetComponent<TakeDamageUI>().SetDamageUI(damage);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            Hit();
        }
    }

    public void Hit()
    {
        cmd_Refresh.CancelRefresh();        // もしリフレッシュ中ならキャンルする
        // ブレイク中はアニメーションを再生しない & Attack中はアニメーションを再生しない
        if (P_State == BattleState.Idle)
        {
            // ステートチェンジ
            playerState.StateChange(BattleState.Hit, null);
            GameUIManager.instance.SetCMDInteractable(false);
            playerState.P_Animator.SetTrigger("Hit");
        }

        Instantiate(effect_Hit, handPos.position, Quaternion.identity);     // Hitエフェクト
        SoundManager.instance.PlaySE("hit");
        CameraShake.instance.Shake(0.2f, 1f, 5f);       // カメラシェイク
    }

    /// <summary>
    /// 死亡時の処理
    /// </summary>
    public void Die()
    {
        GameManager.instance.GameOver();
    }

    /// <summary>
    /// 回復
    /// </summary>
    public void Healing(float healPerSecond)
    {
        health += healPerSecond * Time.deltaTime;
        health = Mathf.Clamp(health, 0, P_Data.Status.Health);
        healthSlider.RefreshSetHealthUI(health, P_Data.Status.Health);
    }
}