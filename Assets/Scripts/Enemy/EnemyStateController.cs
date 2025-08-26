using UnityEngine;

/// <summary>
/// 敵のステート全体を管理するクラス
/// </summary>
public class EnemyStateController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator handAnim;

    [Header("Effect")]
    [SerializeField] GameObject effect_Counter_Success;
    [SerializeField] GameObject effect_Refresh;

    float _stateTime;
    bool stateEnter;
    EnemyHand enemyHand;
    EffectEvents refresh_Event;     // リフレッシュのエフェクトのスクリプト
    public BattleState State { get; private set; } = BattleState.Idle;
    public Animator EnemyAnimator => handAnim;

    void Awake()
    {
        enemyHand = GetComponent<EnemyHand>();
        Initialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Initialize()
    {

    }

    void LateUpdate()
    {
        if (_stateTime != 0)
        {
            stateEnter = false;
        }
    }

    /// <summary>
    /// ステートの切り替え
    /// </summary>
    public void StateChange(BattleState nextState)
    {
        State = nextState;
        _stateTime = 0;
        stateEnter = true;
    }

    void Update()
    {
        _stateTime += Time.deltaTime;

        switch (State)
        {
            case BattleState.BattleIntro:
                break;
            case BattleState.Idle:
                if (stateEnter)
                {
                    handAnim.SetBool("Counter", false);
                    handAnim.SetBool("Break", false);
                    SetHandAnimation(BattleState.Refresh_End);
                }

                break;
            case BattleState.Attack:
                if (stateEnter)
                {
                    // アニメーション(アニメーションイベントでに攻撃発火)
                    SetHandAnimation(BattleState.Attack);
                }
                if (_stateTime > enemyHand.E_Profile.AttackStateDuration) StateChange(BattleState.Idle);
                break;
            case BattleState.Counter:
                if (stateEnter)
                {
                    SetHandAnimation(BattleState.Counter);
                }
                // カウンターの持続時間が過ぎればIdleに戻る
                if (_stateTime > enemyHand.E_Profile.CounterStateDuration)
                {
                    StateChange(BattleState.Idle);
                }
                break;
            case BattleState.Counter_Success:
                if (stateEnter)
                {
                    SoundManager.instance.PlaySE("counter_success");
                    GameManager.instance.StartMomentTime();     // 一瞬止める
                    Instantiate(effect_Counter_Success, enemyHand.CounterPos.position, Quaternion.identity);      // エフェクトを生成
                    SetHandAnimation(BattleState.Counter_Success);
                }
                if (_stateTime > enemyHand.E_Profile.CounterStateDuration) StateChange(BattleState.Idle);
                break;
            case BattleState.Refresh:
                if (stateEnter)
                {
                    refresh_Event = Instantiate(effect_Refresh, enemyHand.RefreshPos.position, Quaternion.identity).GetComponent<EffectEvents>();
                    SetHandAnimation(BattleState.Refresh);
                    enemyHand.Refresh();
                }
                break;
            case BattleState.Refresh_End:
                if (stateEnter)
                {
                    refresh_Event?.EventDestroy();       // 任意のタイミングでエフェクトを削除
                    StateChange(BattleState.Idle);
                }
                break;
            case BattleState.Break:
                if (stateEnter)
                {
                    SetHandAnimation(BattleState.Break);
                }
                // 設定したブレイク時間が過ぎればIdleに戻る
                if (_stateTime > enemyHand.E_Profile.BreakTime)
                {
                    enemyHand.ResetBreakCountUI.Invoke();
                    StateChange(BattleState.Idle);
                }
                break;
            case BattleState.Skill:
                break;
            case BattleState.Goad:
                break;
            case BattleState.BattleClear:
                break;
            case BattleState.GameOver:
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// アニメーションを再生する
    /// </summary>
    void SetHandAnimation(BattleState state)
    {
        switch (state)
        {
            case BattleState.Idle:
                break;
            case BattleState.Attack:
                handAnim.SetTrigger("Attack");
                break;
            case BattleState.Counter:
                handAnim.SetBool("Counter", true);
                break;
            case BattleState.Counter_Success:
                handAnim.SetTrigger("Counter_Success");
                break;
            case BattleState.Refresh:
                handAnim.SetBool("Refresh", true);
                break;
            case BattleState.Refresh_End:
                handAnim.SetBool("Refresh", false);
                break;
            case BattleState.Break:
                handAnim.SetBool("Break", true);
                break;
        }
    }
}