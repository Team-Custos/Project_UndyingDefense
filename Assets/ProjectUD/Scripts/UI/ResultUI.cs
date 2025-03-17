using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [Header("■ UI")]
    [SerializeField] private TextMeshProUGUI rewardTitleUI;
    [SerializeField] private TextMeshProUGUI rewardTextUI;
    [SerializeField] private Image windowUI;
    [SerializeField] private Image lightEffect;
    [SerializeField] private Image underlayEffect;

    [Header("■ Window Sprites")]
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;

    [Header("■ Effect Colors")]
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    public void Show(float reward, bool win)
    {
        gameObject.SetActive(true);
        if(win)
        {
            windowUI.sprite = winSprite;
            rewardTitleUI.text = "보상";
            lightEffect.gameObject.SetActive(true);
            underlayEffect.color = winColor;
        }
        else
        {
            windowUI.sprite = loseSprite;
            rewardTitleUI.text = "위로금";
            lightEffect.gameObject.SetActive(false);
            underlayEffect.color = loseColor;
        }

        rewardTextUI.text = ((int)reward).ToString("0,0");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
