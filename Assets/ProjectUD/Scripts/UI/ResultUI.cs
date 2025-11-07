using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultUI : MonoBehaviour
{
    [Header("■ UI")]
    [SerializeField] private TextMeshProUGUI rewardTitleUI;
    [SerializeField] private TextMeshProUGUI rewardTextUI;
    [SerializeField] private TextMeshProUGUI timeRecordTextUI;
    [SerializeField] private Image windowUI;
    [SerializeField] private Image lightEffect;
    [SerializeField] private Image underlayEffect;
    [SerializeField] private Text resultCommentTextUI;

    [Header("■ Window Sprites")]
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;

    [Header("■ Effect Colors")]
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    public void Show(float reward, bool win, string record)
    {
        gameObject.SetActive(true);
        if(win)
        {
            windowUI.sprite = winSprite;
            lightEffect.gameObject.SetActive(true);
            underlayEffect.color = winColor;
            resultCommentTextUI.text = "지략이 빛을 발한 전투였습니다!";
            timeRecordTextUI.gameObject.SetActive(true);
            timeRecordTextUI.text = record;
        }
        else
        {
            windowUI.sprite = loseSprite;
            lightEffect.gameObject.SetActive(false);
            underlayEffect.color = loseColor;
            resultCommentTextUI.text = "한 걸음 물러나 지혜를 도모해봅시다.";
            timeRecordTextUI.gameObject.SetActive(false);
        }

        rewardTextUI.text = ((int)reward).ToString("0,0");
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
