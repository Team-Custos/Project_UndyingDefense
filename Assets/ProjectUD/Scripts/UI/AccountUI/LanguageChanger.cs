using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;

public class LanguageChanger : MonoBehaviour
{
    private const string KEY_LANGUAGE = "LocaleCode";
    public const string DEFAULT_LOCALE = "ko-KR"; // 초기화 시 기본 언어

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
        //var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        //if (locale != null)
        //{
        //    LocalizationSettings.SelectedLocale = locale;
        //    SaveLanguage(localeCode);
        //}
        SettingManager.Instance.ChangeLanguage(localeCode);
        RefreshButtonStates();
    }

    private void SaveLanguage(string localeCode)
    {
        PlayerPrefs.SetString(KEY_LANGUAGE, localeCode);
        PlayerPrefs.Save();
    }

    // 초기화 버튼용 — 한국어로 되돌리고 저장
    public void ResetToDefaultLanguage()
    {
        ChangeLanguage(DEFAULT_LOCALE);
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
