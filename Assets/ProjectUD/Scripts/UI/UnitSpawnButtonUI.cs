using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitSpawnButtonUI : MonoBehaviour
{
    [Header("■ UI")]
    [SerializeField] private Image portraitUI;
    [SerializeField] private Image backgroundUI;
    [SerializeField] private TextMeshProUGUI priceUI;
    [SerializeField] private GameObject lockUI;

    [Header("■ Tier Colors")]
    [SerializeField] private Color lockedColor;
    [SerializeField] private Color[] tierColors;

    public void Set(Sprite icon, int tier, int price)
    {
        portraitUI.sprite = icon;
        backgroundUI.color = tierColors[tier - 1];
        priceUI.text = price.ToString();
    }

    public void Lock()
    {
        portraitUI.color = Color.gray;
        backgroundUI.color = lockedColor;
        priceUI.text = string.Empty;
    }
}
