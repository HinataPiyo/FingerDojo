using Command;
using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    [Header("アニメーション"), SerializeField] Animator handAnim;
    PlayerHealth p_Health;
    Break cmd_break;

    float _stateTime;
    bool stateEnter;
    
    public Animator P_Animator => handAnim;
    public BattleState State { get; private set; } = BattleState.Idle;

    void Awake()
    {
        p_Health = GetComponent<PlayerHealth>();
        cmd_break = GetComponent<Break>();

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
                if (stateEnter)
                {
                    // コマンドボタンを押せる状態にする
                    GameUIManager.instance.SetCMDInteractable(true);
                }
                break;
            case BattleState.Attack:
                break;
            case BattleState.Counter:
                // counterDurationの間 CMD UI は非アクティブ
                if (_stateTime > p_Health.P_Data.Status.CounterStateDuration
                && State != BattleState.Counter_Success)
                {
                    // カウンターボタンは押したものの失敗してしまったらIdleになる
                    StateChange(BattleState.Idle, null);
                }
                break;
            case BattleState.Counter_Success:
                if (_stateTime > p_Health.P_Data.Status.CounterStateDuration)
                    StateChange(BattleState.Idle, null);
                break;
            case BattleState.Hit:
                if (_stateTime > p_Health.P_Data.Status.HitDuration)
                {
                    GameUIManager.instance.SetCMDInteractable(true);
                    StateChange(BattleState.Idle, null);
                }
                break;
            case BattleState.Refresh:
                break;
            case BattleState.Break:
                // 設定したブレイク時間が過ぎればIdleに戻る
                if (_stateTime > p_Health.P_Data.Status.BreakTime)
                {
                    cmd_break.DurationEvent();
                }
                break;
            case BattleState.Skill:
                if (stateEnter)
                {
                    GameUIManager.instance.SetCMDInteractable(false);
                }
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
    /// アニメーションイベントで設定
    /// </summary>
    public void AttackImpactAnimation()
    {
        handAnim.SetBool("Attack", false);
    }
}