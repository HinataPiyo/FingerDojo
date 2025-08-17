using TMPro;
using UnityEngine;

/// <summary>
/// ダメージを受けたときに表示されるUIを管理するクラス
/// </summary>
public class TakeDamageUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damage_Text;

    /// <summary>
    /// ダメージ量を表示
    /// </summary>
    public void SetDamageUI(float damage)
    {
        damage_Text.text = damage.ToString("F0");
    }
}