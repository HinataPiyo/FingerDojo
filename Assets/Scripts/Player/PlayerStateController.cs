using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    [Header("アニメーション"), SerializeField] Animator handAnim;
    [Header("GameObjects"), SerializeField] TimingBarController timingBarCtrl;
    [Header("Effect")]
    [SerializeField] GameObject effect_Counter_Success;
    [SerializeField] GameObject effect_Refresh;

    [SerializeField] CommandTimingQTE cmd_TimingQTE;
    [SerializeField] EnemyActionCounter enemyCounter;

    [Header("Settings"), SerializeField] float counterDuration = 0.5f;

    PlayerHand playerHand;
    float _stateTime;
    bool stateEnter;
    EffectEvents refresh_Event;     // リフレッシュのエフェクトのスクリプト
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
    /// コマンドを実行する
    /// </summary>
    public void ExecuteCommand(CommandSO command)
    {
        command.Execute(gameObject);
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
                    // コマンドボタンを押せる状態にする
                    GameUIManager.instance.SetCMDInteractable(true);
                }

                break;
            case BattleState.Attack:
                if (stateEnter)
                {
                    GameUIManager.instance.SetCMDInteractable(false);
                    SetHandAnimation(BattleState.Attack);       // アニメーション
                    // タイミングバーを出現
                    timingBarCtrl.gameObject.SetActive(true);
                    cmd_TimingQTE.gameObject.SetActive(true);

                    enemyCounter.CheckCounterAction();      // 敵がカウンターをするか判定
                }

                break;
            case BattleState.Counter:
                if (stateEnter)
                {
                    SoundManager.instance.PlaySE("counter_befor");
                    GameUIManager.instance.SetCMDInteractable(false);
                    SetHandAnimation(BattleState.Counter);
                }
                // counterDurationの間 CMD UI は非アクティブ
                if (_stateTime > counterDuration) StateChange(BattleState.Idle);
                break;
            case BattleState.Counter_Success:
                if (stateEnter)
                {
                    SoundManager.instance.PlaySE("counter_success");
                    GameManager.instance.StartMomentTime();     // 一瞬止める
                    Instantiate(effect_Counter_Success, playerHand.CounterPos.position, Quaternion.identity);      // エフェクトを生成
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
                    refresh_Event = Instantiate(effect_Refresh, playerHand.RefreshPos.position, Quaternion.identity).GetComponent<EffectEvents>();

                    GameUIManager.instance.SetCMDInteractable(false);
                    playerHand.Refresh();
                    SetHandAnimation(BattleState.Refresh);
                }

                break;
            case BattleState.Refresh_End:
                if (stateEnter)
                {
                    refresh_Event?.EventDestroy();       // 任意のタイミングでエフェクトを削除
                    SetHandAnimation(BattleState.Refresh_End);
                    StateChange(BattleState.Idle);
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
            case BattleState.Refresh:
                handAnim.SetBool("Refresh", true);
                break;
            case BattleState.Refresh_End:
                handAnim.SetBool("Refresh", false);
                break;
        }
    }
}