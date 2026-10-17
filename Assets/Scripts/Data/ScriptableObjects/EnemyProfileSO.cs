using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 敵の個性を定義するScriptableObject。
/// ステータス、基本行動、そしてAIの「賢さ」を定義するパラメータを持つ。
/// </summary>
[CreateAssetMenu(fileName = "EnemyProfileSO", menuName = "FingerDojo/EnemyProfileSO")]
public class EnemyProfileSO : ScriptableObject
{
    [Header("難易度")]
    // Titleで選択した難易度とこの設定データの対応を実行時に検証する。
    [SerializeField] DifficultyId difficultyId = DifficultyId.Normal;

    [Header("基本情報")]
    [SerializeField] StatusConst status;

    [Header("待機間隔の設定")]
    [SerializeField] float idleTime = 2f;
    [SerializeField] float idleTimeMultiplier = 0.5f;

    [Header("攻撃予兆とCounter")]
    // 攻撃開始前の表示時間と、反応Counterへ移るまでの待機範囲。
    [SerializeField, Min(0f)] float attackTelegraphDuration = 1.1f;
    [SerializeField, Min(0f)] float counterAttemptDelayMin;
    [SerializeField, Min(0f)] float counterAttemptDelayMax = 1f;

    [Header("カウンター設定")]
    [Tooltip("プレイヤーの攻撃に反応してカウンターを試みる確率")]
    [SerializeField, Range(0f, 1f)] float reactiveCounterChance = 0.5f;

    [Header("ギミック設定")]
    // 難易度ごとの行動条件。各フラグは共通AIの選択条件を切り替える。
    [SerializeField] bool refreshOnlyAtHalfHealth;
    [SerializeField] bool repeatedCommandCounter;
    [SerializeField] bool comboAttackEnabled;
    [SerializeField, Range(0f, 1f)] float comboAttackChance;

    [Header("攻撃設定")]
    [Tooltip("攻撃状態の持続時間（秒）")]
    // Animatorイベントを待つ時間でもあり、イベント欠落時の代替解決期限でもある。
    [SerializeField] float attackStateDuration = 1f;

    [Header("アクションの選択重み")]
    [Tooltip("攻撃アクションの基本選択重み")]
    [SerializeField, Range(1, 100)] int attackWeight = 50;

    [Tooltip("回復アクションの基本選択重み")]
    [SerializeField, Range(1, 100)] int refreshWeight = 50;

    [Tooltip("カウンターアクションの基本選択重み")]
    [SerializeField, Range(1, 100)] int counterWeight = 50;

    [Tooltip("挑発アクションの基本選択重み")]
    [SerializeField, Range(1, 100)] int goadWeight = 20;

    [Tooltip("スキルアクションの基本選択重み")]
    [SerializeField, Range(1, 100)] int skillWeight = 30;

    // --- public properties ---
    public DifficultyId DifficultyId => difficultyId;
    public StatusConst Status => status;
    public float IdleTime => idleTime;
    public float IdleTimeMultiplier => idleTimeMultiplier;
    public float AttackTelegraphDuration => attackTelegraphDuration;
    public float CounterAttemptDelayMin => counterAttemptDelayMin;
    public float CounterAttemptDelayMax => counterAttemptDelayMax;
    public float ReactiveCounterChance => reactiveCounterChance;
    public bool RefreshOnlyAtHalfHealth => refreshOnlyAtHalfHealth;
    public bool RepeatedCommandCounter => repeatedCommandCounter;
    public bool ComboAttackEnabled => comboAttackEnabled;
    public float ComboAttackChance => comboAttackChance;
    public float AttackStateDuration => attackStateDuration;
    public int AttackWeight => attackWeight;
    public int RefreshWeight => refreshWeight;
    public int CounterWeight => counterWeight;
    public int GoadWeight => goadWeight;
    public int SkillWeight => skillWeight;
}