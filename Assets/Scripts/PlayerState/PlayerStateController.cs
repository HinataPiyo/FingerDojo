using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    [Header("アニメーション"), SerializeField] Animator handAnim;

    PlayerHand playerHand;
    float _stateTime;
    bool stateEnter;
    public BattleState State { get; private set; } = BattleState.Idle;

    void Awake()
    {
        playerHand = GetComponent<PlayerHand>();
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
        Debug.Log("player : " + State);
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
                    // コマンドボタンを押せる状態にする
                    GameUIManager.instance.SetCMDInteractable(true);
                }

                break;
            case BattleState.Attack:
                if (stateEnter)
                {
                    GameUIManager.instance.SetCMDInteractable(false);
                    SetHandAnimation(BattleState.Attack);       // アニメーション
                }

                break;
            case BattleState.Counter:
                if (stateEnter)
                {
                    GameUIManager.instance.SetCMDInteractable(false);
                    SetHandAnimation(BattleState.Counter);
                }
                break;
            case BattleState.Counter_Success:
                if (stateEnter)
                {
                    SetHandAnimation(BattleState.Counter_Success);
                }

                if(_stateTime > 0.5) StateChange(BattleState.Idle);
                break;
            case BattleState.Hit:
                if (stateEnter)
                {
                    SetHandAnimation(BattleState.Hit);
                }
                if(_stateTime > 0.25) StateChange(BattleState.Idle);
                break;
            case BattleState.Refresh:
                if (stateEnter)
                {
                    GameUIManager.instance.SetCMDInteractable(false);
                    playerHand.Refresh();
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