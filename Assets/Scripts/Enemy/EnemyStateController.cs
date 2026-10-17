using EnemyAction;
using UnityEngine;

/// <summary>
/// 敵のステート全体を管理するクラス
/// </summary>
public class EnemyStateController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator handAnim;

    float _stateTime;
    bool stateEnter;
    Break cmd_Break;
    Attack cmd_Attack;
    EnemyHealth e_Health;
    public BattleState State { get; private set; } = BattleState.Idle;
    public Animator E_Animator => handAnim;

    void Awake()
    {
        e_Health = GetComponent<EnemyHealth>();
        cmd_Break = GetComponent<Break>();
        cmd_Attack = GetComponent<Attack>();
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
    public void StateChange(BattleState nextState, ICommand _cmd)
    {
        if (GameManager.instance.isGameEnd) return;

        State = nextState;
        _stateTime = 0;
        stateEnter = true;
        _cmd?.Execute();
    }

    void Update()
    {
        if (GameManager.instance.isGameEnd) return;
        
        _stateTime += Time.deltaTime;

        switch (State)
        {
            case BattleState.BattleIntro:
                break;
            case BattleState.Idle:
                break;
            case BattleState.Attack:
                if (_stateTime > cmd_Attack.StateDuration)
                {
                    // AnimationEvent未着時の保険として、Attack側に残ったダメージ解決を依頼する。
                    cmd_Attack.ResolveAtStateEnd();
                    StateChange(BattleState.Idle, null);
                }
                break;
            case BattleState.Counter:
                // カウンターの持続時間が過ぎればIdleに戻る
                if (_stateTime > e_Health.E_Profile.Status.CounterStateDuration
                && State != BattleState.Counter_Success)
                {
                    StateChange(BattleState.Idle, null);
                }
                break;
            case BattleState.Counter_Success:
                // カウンター持続時間が過ぎたらIdleに戻る
                if (_stateTime > e_Health.E_Profile.Status.CounterStateDuration)
                {
                    StateChange(BattleState.Idle, null);
                }
                break;
            case BattleState.Hit:
                if (_stateTime > e_Health.E_Profile.Status.HitDuration)
                {
                    StateChange(BattleState.Idle, null);
                }
                break;
            case BattleState.Break:
                // 設定したブレイク時間が過ぎればIdleに戻る
                if (_stateTime > e_Health.E_Profile.Status.BreakTime)
                {
                    cmd_Break.DurationEvent();
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
}