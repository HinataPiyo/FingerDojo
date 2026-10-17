using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI関連全てを管理するクラス
/// </summary>
public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance { get; private set; }
    [Header("WorldCanvas"), SerializeField] Transform worldCanvas;

    [SerializeField] Button[] cmd_Buttons;
    [SerializeField] GameObject counterText;

    [Header("Win or Lose Panel")]
    [SerializeField] Canvas win_Lose_Canvas;
    [SerializeField] GameObject winPanel;
    [SerializeField] GameObject losePanel;
    public Transform WorldCanvas => worldCanvas;

    TMP_Text counterTextLabel;
    string defaultCounterText;
    // 攻撃予兆と、その間に発生した通知を別々に保持して表示競合を防ぐ。
    string currentEnemyTelegraph;
    string queuedCommandMessage;
    bool showingEnemyTelegraph;
    // 連続攻撃を読まれた状態は予兆とは別管理し、予兆更新後も表示に残す。
    bool repeatedAttackWarning;

    /// <summary>
    /// 戦闘画面の勝敗表示と通知テキストを初期化する。
    /// </summary>
    void Awake()
    {
        // シーン内でUI管理者を一つに保ち、破棄された重複オブジェクトからの更新を防ぐ。
        if (instance == null) instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        win_Lose_Canvas.enabled = false;
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        // CounterTextを予兆・コマンド通知の共通表示欄として使う。
        counterTextLabel = counterText.GetComponent<TMP_Text>();
        if (counterTextLabel == null)
        {
            Debug.LogError("CounterText requires a TextMeshPro component.", counterText);
        }
        else
        {
            defaultCounterText = counterTextLabel.text;
        }
    }

    /// <summary>
    /// プレイヤーの行動中にコマンドボタンをまとめて有効／無効にする。
    /// </summary>
    public void SetCMDInteractable(bool interactable)
    {
        foreach (Button b in cmd_Buttons)
        {
            b.interactable = interactable;
        }
    }

    /// <summary>
    /// Counter入力可能表示に戻し、直前の一時通知を消す。
    /// </summary>
    public void ActiveCounterText()
    {
        // もしすでにAcriveなら一度非アクティブにする
        if (counterText.activeSelf == true)
        {
            counterText.SetActive(false);
        }

        // 新しいコマンド表示を始める前に、前回の予兆と予約メッセージを破棄する。
        showingEnemyTelegraph = false;
        currentEnemyTelegraph = null;
        queuedCommandMessage = null;
        if (counterTextLabel != null)
        {
            counterTextLabel.text = repeatedAttackWarning
                ? defaultCounterText + "\nNEXT ATTACK WILL BE COUNTERED"
                : defaultCounterText;
        }
        counterText.SetActive(true);
    }

    /// <summary>
    /// 敵予兆が表示中なら通知を予約し、そうでなければ直ちに表示する。
    /// </summary>
    public void ShowCommandMessage(string message)
    {
        if (counterTextLabel == null) return;
        if (showingEnemyTelegraph)
        {
            // 予兆の視認性を保ち、予兆終了後に最新の通知を表示する。
            queuedCommandMessage = message;
            return;
        }

        counterTextLabel.text = message;
        counterText.SetActive(true);
    }

    /// <summary>
    /// 敵攻撃の強度と連携段数を通知欄へ表示する。
    /// </summary>
    public void ShowEnemyTelegraph(string strengthBand)
    {
        if (counterTextLabel == null) return;

        // 攻撃強度と連携段階はAttack側で確定し、ここでは表示状態だけを保持する。
        showingEnemyTelegraph = true;
        currentEnemyTelegraph = strengthBand;
        RenderEnemyTelegraph();
    }

    /// <summary>
    /// 予兆を消し、優先度の高い連続攻撃警告または予約通知を表示する。
    /// </summary>
    public void ClearEnemyTelegraph()
    {
        if (!showingEnemyTelegraph) return;

        showingEnemyTelegraph = false;
        currentEnemyTelegraph = null;
        // 予兆が終わった時点で、残っている警告または最後に予約された通知を表示する。
        if (repeatedAttackWarning)
        {
            queuedCommandMessage = null;
            ShowCommandMessage("ENEMY READS YOUR ATTACK");
        }
        else if (!string.IsNullOrEmpty(queuedCommandMessage))
        {
            counterTextLabel.text = queuedCommandMessage;
            queuedCommandMessage = null;
            counterText.SetActive(true);
        }
        else
        {
            counterText.SetActive(false);
        }
    }

    /// <summary>
    /// 連続攻撃へのCounter予告を更新し、表示中の予兆にも反映する。
    /// </summary>
    public void SetRepeatedAttackWarning(bool active)
    {
        repeatedAttackWarning = active;
        if (showingEnemyTelegraph)
        {
            RenderEnemyTelegraph();
        }
        else if (active)
        {
            ShowCommandMessage("ENEMY READS YOUR ATTACK");
        }
        else if (counterTextLabel != null && counterTextLabel.text == "ENEMY READS YOUR ATTACK")
        {
            counterText.SetActive(false);
        }
        else if (counterTextLabel != null
            && counterTextLabel.text == defaultCounterText + "\nNEXT ATTACK WILL BE COUNTERED")
        {
            counterTextLabel.text = defaultCounterText;
        }
    }

    /// <summary>
    /// 現在の予兆情報と、必要なら連続攻撃警告を組み合わせて描画する。
    /// </summary>
    void RenderEnemyTelegraph()
    {
        if (counterTextLabel == null) return;

        // 連続攻撃警告を予兆に重ね、プレイヤーが次の攻撃の危険を判断できるようにする。
        counterTextLabel.text = "ENEMY ATTACK: " + currentEnemyTelegraph
            + (repeatedAttackWarning ? "\nNEXT ATTACK WILL BE COUNTERED" : string.Empty);
        counterText.SetActive(true);
    }

    /// <summary>
    /// 勝利パネルを表示し、敗北パネルと戦闘中の通知を閉じる。
    /// </summary>
    public void GameClear()
    {
        // 終了後に古い戦闘通知が残らないよう片付けてから結果画面を確定する。
        repeatedAttackWarning = false;
        queuedCommandMessage = null;
        ClearEnemyTelegraph();
        counterText.SetActive(false);
        SetCMDInteractable(false);
        win_Lose_Canvas.enabled = true;
        winPanel.SetActive(true);
        losePanel.SetActive(false);
    }

    /// <summary>
    /// 敗北パネルを表示し、勝利パネルと戦闘中の通知を閉じる。
    /// </summary>
    public void GameOver()
    {
        // 終了後に古い戦闘通知が残らないよう片付けてから結果画面を確定する。
        repeatedAttackWarning = false;
        queuedCommandMessage = null;
        ClearEnemyTelegraph();
        counterText.SetActive(false);
        SetCMDInteractable(false);
        win_Lose_Canvas.enabled = true;
        winPanel.SetActive(false);
        losePanel.SetActive(true);
    }

}