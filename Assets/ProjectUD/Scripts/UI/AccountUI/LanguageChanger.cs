using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class LanguageChanger : MonoBehaviour
{
    [Header("한국어 버튼")]
    [SerializeField] private Image koreanButtonImage;
    [SerializeField] private TextMeshProUGUI koreanButtonText;

    [Header("영어 버튼")]
    [SerializeField] private Image englishButtonImage;
    [SerializeField] private TextMeshProUGUI englishButtonText;
    [Header("선택 상태")]
    [SerializeField] private Sprite selectedSprite;
    [SerializeField] private Color selectedTextColor;

    [Header("미선택 상태")]
    [SerializeField] private Sprite deselectedSprite;
    [SerializeField] private Color deselectedTextColor;
    private void OnEnable()
    {
        // 계정창이 열릴 때 현재 언어로 버튼 상태 초기화
        RefreshButtonStates();
    }
    public void ChangeToKorean()
    {
        ChangeLanguage("ko-KR");
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    public void ChangeToEnglish()
    {
        ChangeLanguage("en");
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    private void ChangeLanguage(string localeCode)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (locale != null)
            LocalizationSettings.SelectedLocale = locale;
        RefreshButtonStates();
    }

    private void RefreshButtonStates()
    {
        string currentCode = LocalizationSettings.SelectedLocale?.Identifier.Code;
        bool isKorean = currentCode == "ko-KR";

        ApplyState(koreanButtonImage, koreanButtonText, isKorean);
        ApplyState(englishButtonImage, englishButtonText, !isKorean);
    }

    private void ApplyState(Image image, TextMeshProUGUI text, bool isSelected)
    {
        image.sprite = isSelected ? selectedSprite : deselectedSprite;
        text.color = isSelected ? selectedTextColor : deselectedTextColor;
    }
}
