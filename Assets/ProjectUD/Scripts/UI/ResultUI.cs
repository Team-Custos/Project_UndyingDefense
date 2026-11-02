using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ResultUI : MonoBehaviour
{
    [Header("■ UI")]
    [SerializeField] private GameObject recordUI;
    [SerializeField] private TextMeshProUGUI rewardTextUI;
    [SerializeField] private TextMeshProUGUI timeRecordTextUI;
    [SerializeField] private Image windowUI;
    [SerializeField] private Image lightEffect;
    [SerializeField] private Image underlayEffect;
    [SerializeField] private TextMeshProUGUI resultCommentTextUI;
    [SerializeField] private GameObject newRecordUI;          // IngameScreenUI에서 이동


    [Header("■ Window Sprites")]
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite;

    [Header("■ Effect Colors")]
    [SerializeField] private Color winColor;
    [SerializeField] private Color loseColor;

    // 판단 중복 & Lose에서 record 매개 변수 표시 안 함 > 인게임에서 이미 판단, ShowWin() / ShowLose()로 나눠서 호출하는 방식으로 변경
    public void ShowWin(float reward, string record, bool isNewRecord)
    {
        ShowCommon(reward, winSprite, winColor, "TXT_resultWin01");

        lightEffect.gameObject.SetActive(true);
        recordUI.SetActive(true);
        timeRecordTextUI.gameObject.SetActive(true);
        timeRecordTextUI.text = record + '"';
        newRecordUI.SetActive(isNewRecord);
    }

    public void ShowLose(float reward)
    {
        ShowCommon(reward, loseSprite, loseColor, "TXT_resultLose01");

        lightEffect.gameObject.SetActive(false);
        recordUI.SetActive(false);
        timeRecordTextUI.gameObject.SetActive(false);
        newRecordUI.SetActive(false);
    }

    private void ShowCommon(float reward, Sprite window, Color effectColor, string commentKey)
    {
        gameObject.SetActive(true);
        windowUI.sprite = window;
        underlayEffect.color = effectColor;
        resultCommentTextUI.text = LocalizationSettings.StringDatabase
            .GetLocalizedString("IngameUI", commentKey, LocalizationSettings.SelectedLocale);
        rewardTextUI.text = ((int)reward).ToString();
    }

    public void Show(float reward, bool win, string record)
    {
        gameObject.SetActive(true);     // 공통 : rewardTextUI 텍스트, sprite, underlayEffect 색상, resultCommentTextUI 텍스트 설정
        if (win)         // 다른 점: lightEffect 활성화, timeRecordTextUI 활성화 및 텍스트 설정
        {
            windowUI.sprite = winSprite;
            lightEffect.gameObject.SetActive(true);
            underlayEffect.color = winColor;
            //--Localization
            //resultCommentTextUI.text = "지략이 빛을 발한 전투였습니다!";
            resultCommentTextUI.text = LocalizationSettings.StringDatabase.GetLocalizedString("IngameUI", "TXT_resultWin01",
                 LocalizationSettings.SelectedLocale);
            timeRecordTextUI.gameObject.SetActive(true);
            timeRecordTextUI.text = record + '"';
        }
        else  // 다른 점: lightEffect 비활성화, timeRecordTextUI 비활성화
        {
            recordUI.SetActive(false);

            windowUI.sprite = loseSprite;
            lightEffect.gameObject.SetActive(false);
            underlayEffect.color = loseColor;
            //--Localization
            //resultCommentTextUI.text = "한 걸음 물러나 지혜를 도모해봅시다.";
            resultCommentTextUI.text = LocalizationSettings.StringDatabase.GetLocalizedString("IngameUI", "TXT_resultLose01",
                 LocalizationSettings.SelectedLocale);
            timeRecordTextUI.gameObject.SetActive(false);
        }

        rewardTextUI.text = ((int)reward).ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
