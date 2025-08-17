public enum BattleState
{
    BattleIntro,    // 開始演出
    Free,           // 常時（入力受付・CT更新）
    Attack,         // 指撃中（バー出現〜確定）
    Counter,        // ガード判定窓
    Refresh,        // 休む中（無防備）
    Skill,          // 技発動中
    Resolve,        // ダメージ反映/怯み等
    BattleClear,    // 次Wave準備
    GameOver
}