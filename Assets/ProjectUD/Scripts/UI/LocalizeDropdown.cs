using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

[Serializable]
public class LocalizedDropdownOption
{
    public LocalizedString text; // Inspector에서 Table = CommonUI, Entry = 낮음/중간/높음 각각 선택
}

[RequireComponent(typeof(TMP_Dropdown))]
public class LocalizeDropdown : MonoBehaviour
{
    public List<LocalizedDropdownOption> options; // 순서대로 낮음, 중간, 높음

    TMP_Dropdown dropdown;

    void Awake() => dropdown = GetComponent<TMP_Dropdown>();

    void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        RefreshOptions();
    }

    void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(UnityEngine.Localization.Locale locale) => RefreshOptions();

    async void RefreshOptions()
    {
        int currentIndex = dropdown.value;

        var texts = new List<string>();
        foreach (var option in options)
            texts.Add(await option.text.GetLocalizedStringAsync().Task);

        dropdown.ClearOptions();
        dropdown.AddOptions(texts);
        dropdown.SetValueWithoutNotify(Mathf.Clamp(currentIndex, 0, texts.Count - 1));
        dropdown.RefreshShownValue();
    }
}
