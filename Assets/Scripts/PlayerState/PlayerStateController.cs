using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    [Header("アニメーション"), SerializeField] Animator handAnim;

    float _stateTime;
    bool stateEnter;
    public BattleState State { get; private set; } = BattleState.BattleIntro;

    void Awake()
    {
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
                    SetHandAnimation(BattleState.Idle);
                }

                break;
            case BattleState.Attack:
                if (stateEnter)
                {
                    // アニメーション
                    SetHandAnimation(BattleState.Attack);
                }

                break;
            case BattleState.Counter:
                if (stateEnter)
                {
                    SetHandAnimation(BattleState.Counter);
                }
                break;
            case BattleState.Counter_Success:
                if (stateEnter)
                {
                    SetHandAnimation(BattleState.Counter_Success);
                }
                break;
            case BattleState.Hit:
                if (stateEnter)
                {
                    SetHandAnimation(BattleState.Hit);
                }
                break;
            case BattleState.Refresh:
                break;
            case BattleState.Skill:
                break;
            case BattleState.Resolve:
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
            case BattleState.Attack:
                handAnim.SetTrigger("Attack");
                break;
            case BattleState.Counter:
                handAnim.SetTrigger("Counter");
                break;
            case BattleState.Counter_Success:
                handAnim.SetTrigger("Counter_Success");
                break;
            case BattleState.Hit:
                handAnim.SetTrigger("Hit");
                break;
        }
    }
}