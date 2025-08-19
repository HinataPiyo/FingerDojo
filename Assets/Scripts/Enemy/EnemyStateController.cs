using UnityEngine;

/// <summary>
/// 敵のステート全体を管理するクラス
/// </summary>
public class EnemyStateController : MonoBehaviour
{
    [Header("アニメーション"), SerializeField] Animator handAnim;
    [Header("参照コンポーネント"), SerializeField] EnemyHand enemyHand;
    [SerializeField] EnemyProfileSO enemyProfile;

    float _stateTime;
    bool stateEnter;
    public BattleState State { get; private set; } = BattleState.Idle;

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
        Debug.Log("enemy : " + State);
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
                }

                break;
            case BattleState.Attack:
                if (stateEnter)
                {
                    // アニメーション(アニメーションイベントでに攻撃発火)
                    SetHandAnimation(BattleState.Attack);
                }
                if (_stateTime > enemyProfile.AttackStateDuration) StateChange(BattleState.Idle);
                break;
            case BattleState.Counter:
                if (stateEnter)
                {
                    // SetHandAnimation(BattleState.Counter);
                }
                break;
            case BattleState.Counter_Success:
                if (_stateTime > enemyProfile.CounterStateDuration) StateChange(BattleState.Idle);
                break;
            case BattleState.Refresh:
                if (stateEnter)
                {
                    enemyHand.Refresh();
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
        }
    }
}