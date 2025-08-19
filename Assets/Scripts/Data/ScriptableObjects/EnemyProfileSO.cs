using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 敵の個性を定義するScriptableObject。
/// ステータス、基本行動、そしてAIの「賢さ」を定義するパラメータを持つ。
/// </summary>
[CreateAssetMenu(fileName = "EnemyProfileSO", menuName = "FingerDojo/EnemyProfileSO")]
public class EnemyProfileSO : ScriptableObject
{
    [Header("基本情報")]
    [Tooltip("敵の基本的なステータス（体力、攻撃力など）")]
    [SerializeField] private StatusConst status;

    [Tooltip("行動を選択するまでの待機時間")]
    [SerializeField] private float idleTime = 2f;

    [Header("カウンター設定")]
    [Tooltip("プレイヤーの攻撃に反応してカウンターを試みる確率")]
    [SerializeField, Range(0f, 1f)] private float reactiveCounterChance = 0.5f;

    [Tooltip("カウンター状態の持続時間（秒）")]
    [SerializeField] private float counterStateDuration = 1f;
    [Tooltip("攻撃状態の持続時間（秒）")]
    [SerializeField] private float attackStateDuration = 1f;

    [Header("AIの賢さ（行動パターン）")]
    [Tooltip("HPがこの割合（0~1）以下になると、Refresh（回復）を優先的に選択する")]
    [SerializeField, Range(0f, 1f)] private float lowHealthRefreshThreshold = 0.3f;

    [Tooltip("プレイヤーがこれらの状態の時に、攻撃を選択肢に入れる")]
    [SerializeField] private List<BattleState> attackPlayerStates;

    [Header("アクションの選択重み")]
    [Tooltip("攻撃アクションの基本選択重み")]
    [SerializeField, Range(1, 100)] private int attackWeight = 50;

    [Tooltip("回復アクションの基本選択重み")]
    [SerializeField, Range(1, 100)] private int refreshWeight = 50;

    [Tooltip("カウンターアクションの基本選択重み")]
    [SerializeField, Range(1, 100)] private int counterWeight = 50;

    [Tooltip("挑発アクションの基本選択重み")]
    [SerializeField, Range(1, 100)] private int goadWeight = 20;

    [Tooltip("スキルアクションの基本選択重み")]
    [SerializeField, Range(1, 100)] private int skillWeight = 30;

    // --- public properties ---
    public StatusConst Status => status;
    public float IdleTime => idleTime;
    public float ReactiveCounterChance => reactiveCounterChance;
    public float CounterStateDuration => counterStateDuration;
    public float AttackStateDuration => attackStateDuration;
    public float LowHealthRefreshThreshold => lowHealthRefreshThreshold;
    public List<BattleState> AttackPlayerStates => attackPlayerStates;
    public int AttackWeight => attackWeight;
    public int RefreshWeight => refreshWeight;
    public int CounterWeight => counterWeight;
    public int GoadWeight => goadWeight;
    public int SkillWeight => skillWeight;
}