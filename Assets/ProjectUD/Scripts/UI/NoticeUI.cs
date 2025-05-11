using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoticeUI : MonoBehaviour
{
    [Header("■ UI")]
    [SerializeField] private Image panelUI;
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private GameObject skipBtn;
    [SerializeField] private GameObject warningIcon;

    [Header("■ Panel Sprites")]
    [SerializeField] private Sprite panelSprite;
    [SerializeField] private Sprite warningPanelSprite;

    public void SetText(string text, bool isWarning = false)
    {
        gameObject.SetActive(true);


        if (isWarning)
        {
            textUI.color = Color.red;
            panelUI.sprite = warningPanelSprite;
            warningIcon.SetActive(true);
        }
        else
        {
            textUI.color = Color.white;
            panelUI.sprite = panelSprite;
            warningIcon.SetActive(false);
        }

        textUI.text = text;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetText(string text)
    {
        textUI.text = text;
    }

}
