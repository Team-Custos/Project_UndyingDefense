using TMPro;
using UltEvents;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class LanguageChanger : MonoBehaviour
{
    private const string KEY_LANGUAGE = "LocaleCode";
    public const string DEFAULT_LOCALE = "ko-KR"; // 초기화 시 기본 언어

    [Header("인풋 이벤트 매니저")]
    [SerializeField] private PlayerInputEventManager inputEventManager;

    [Header("시스템 확인 창")]
    [SerializeField] private SystemConfirmUI systemConfirmUI;

    [Header("확인창 Localization")]
    [SerializeField] private LocalizedString languageChangeMessage;

    [Header("Change To Korean Event")]
    [SerializeField] private UltEvent ToKoreanEvent;

    [Header("Change To English Event")]
    [SerializeField] private UltEvent ToEnglishEvent;

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

    // 계정정보창 한국어 버튼용
    public void OnKoreanButtonClicked()
    {
        systemConfirmUI.SetConfirmUI(languageChangeMessage, ToKoreanEvent);
        inputEventManager.OnESCTarget = null;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    // 계정정보창 영어 버튼용
    public void OnEnglishButtonClicked()
    {
        systemConfirmUI.SetConfirmUI(languageChangeMessage, ToEnglishEvent);
        inputEventManager.OnESCTarget = null;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
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
        ReloadScene();
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

    // 씬을 다시 로드하여 언어 변경 사항을 적용하는 메서드
    public void ReloadScene()
    {
        // 로딩씬을 통해 씬을 다시 로드하여 언어 변경 사항을 적용
        LoadingSceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
