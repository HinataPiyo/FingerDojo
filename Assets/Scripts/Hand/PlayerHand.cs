using UnityEngine;

/// <summary>
/// 敵の手を管理するクラス
/// </summary>
public class PlayerHand : MonoBehaviour, IDamageble
{
    [SerializeField] GameObject taleDamageUI;
    [SerializeField] Transform handPos;

    [Header("ステート管理"), SerializeField] PlayerStateController playerState;

    [Header("Comamnd")]
    [SerializeField] CommandCounter cmd_Counter;

    [Header("データ"), SerializeField] PlayerDataSO playerData;
    [Header("アニメーション"), SerializeField] Animator handAnim;
    [Header("UI")]
    [SerializeField] HealthSlider healthSlider;
    float health;
    public StatusConst Status => playerData.Status;

    void Awake()
    {
        health = playerData.Status.Health;
        healthSlider.SetHealthUI(health, playerData.Status.Health);
    }

    /// <summary>
    /// ダメージの受け取り処理
    /// </summary>
    public void TakeDamage(float damage)
    {
        // カウンター中だった場合
        if (cmd_Counter.IsCounter)
        {
            CombatCalculator.instance.PlayerToEnemyCounter();
            playerState.StateChange(BattleState.Counter_Success);
            return;
        }

        health -= damage;
        healthSlider.SetHealthUI(health, playerData.Status.Health);

        GameObject obj = Instantiate(taleDamageUI, handPos.position, transform.rotation, GameUIManager.instance.WorldCanvas);
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
        playerState.StateChange(BattleState.Hit);
    }

    public void Die()
    {

    }
}