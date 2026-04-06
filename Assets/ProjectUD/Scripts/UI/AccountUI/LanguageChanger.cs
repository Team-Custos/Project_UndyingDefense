using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageChanger : MonoBehaviour
{
    public void ChangeToKorean()
    {
        ChangeLanguage("ko-KR");
    }

    public void ChangeToEnglish()
    {
        ChangeLanguage("en");
    }

    private void ChangeLanguage(string localeCode)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeCode);
        if (locale != null)
            LocalizationSettings.SelectedLocale = locale;
    }
}
