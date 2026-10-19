using EnemyAction;
using UnityEngine;

/// <summary>
/// 敵の手を管理するクラス
/// </summary>
public class EnemyHealth : MonoBehaviour, IDamageble
{
    [Header("Effect")]
    [SerializeField] GameObject takeDamageUI;
    [SerializeField] GameObject effect_Hit;

    [Header("Hand Position")]
    [SerializeField] Transform handPos;

    [Header("Data")]
    [SerializeField] EnemyProfileSO enemyProfile;

    [Header("UI")]
    [SerializeField] HealthSlider healthSlider;

    EnemyStateController e_StateCtrl;
    Break cmd_Break;
    Counter cmd_Counter;
    CounterSuccess cmd_CounterSuccess;
    Refresh cmd_Refresh;
    EnemyDecisionController decisionController;

    float health;
    
    public float GetCurrentHealth => health;
    public BattleState E_State => e_StateCtrl.State;
    public EnemyProfileSO E_Profile => enemyProfile;
    public Counter E_Counter => cmd_Counter;
    // Counter判定から難易度条件と連続攻撃履歴を参照するために公開する。
    public EnemyDecisionController DecisionController => decisionController;

    void Awake()
    {
        e_StateCtrl = GetComponent<EnemyStateController>();
        cmd_Break = GetComponent<Break>();
        cmd_Counter = GetComponent<Counter>();
        cmd_CounterSuccess = GetComponent<CounterSuccess>();
        cmd_Refresh = GetComponent<Refresh>();
        decisionController = GetComponent<EnemyDecisionController>();
    }

    void Start()
    {
        // Sceneの初期化後に、敵のBreak回数表示をプロフィールに合わせて初期化する。
        cmd_Break.ResetBreakCountUI();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize(EenmyDifficultyListSO.Data data)
    {
        if (data != null)
        {
            enemyProfile = data.profileSO;       // Titleで設定した難易度ごとのSOを設定
        }

        // 体力を設定
        health = enemyProfile.Status.Health;
        healthSlider.HitSetHealthUI(health, enemyProfile.Status.Health);
    }

    /// <summary>
    /// ダメージの受け取り処理
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (GameManager.instance.isGameEnd) return;

        cmd_Break.CheckBreak();             // ブレイク状態を更新する
        bool isCounter = cmd_CounterSuccess.CheckCounter();         // カウンター状態を更新する
        if (isCounter) return;              // カウンターが成功したら

        health -= damage;
        healthSlider.HitSetHealthUI(health, enemyProfile.Status.Health);

        GameObject obj = Instantiate(takeDamageUI, handPos.position, Quaternion.identity, GameUIManager.instance.WorldCanvas);
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
        if (e_StateCtrl.State == BattleState.Idle)
        {
            // ステートチェンジ
            e_StateCtrl.StateChange(BattleState.Hit, null);
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
        GameManager.instance.GameClear();
    }
    
    /// <summary>
    /// 回復
    /// </summary>
    public float Healing(float amount)
    {
        if (GameManager.instance.isGameEnd || amount <= 0f) return 0f;

        // 上限適用後の実回復量を返し、呼び出し側の回復予算と実HPの増分を一致させる。
        float previousHealth = health;
        health += amount;
        health = Mathf.Clamp(health, 0, E_Profile.Status.Health);
        healthSlider.RefreshSetHealthUI(health, E_Profile.Status.Health);
        return health - previousHealth;
    }
}