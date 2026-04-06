using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    [Header("사운드 슬라이더")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;

    [Header("퍼센트 텍스트")]
    [SerializeField] private TextMeshProUGUI masterText;
    [SerializeField] private TextMeshProUGUI bgmText;
    [SerializeField] private TextMeshProUGUI combatText;
    [SerializeField] private TextMeshProUGUI uiText;

    [Header("음소거 토글")]
    [SerializeField] private Toggle muteToggle;

    // 슬라이더 초기화 중 OnValueChanged 이벤트 무시 플래그
    private bool isInitializing = false;

    private void OnEnable()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        isInitializing = true;

        SettingManager sm = SettingManager.Instance;

        masterSlider.value = sm.MasterVolume;
        bgmSlider.value = sm.BGMVolume;
        sfxSlider.value = sm.SFXVolume;
        uiSlider.value = sm.UIVolume;
        muteToggle.isOn = sm.IsMuted;

        RefreshAllTexts();

        isInitializing = false;
    }

    // ── Inspector에서 각 Slider의 OnValueChanged에 아래 메서드 연결

    public void OnMasterVolumeChanged(float value)
    {
        if (isInitializing) return;
        SettingManager.Instance.SetMasterVolume(value);
        masterText.text = ToPercent(value);
    }

    public void OnBGMVolumeChanged(float value)
    {
        if (isInitializing) return;
        SettingManager.Instance.SetBGMVolume(value);
        bgmText.text = ToPercent(value);
    }

    public void OnCombatVolumeChanged(float value)
    {
        if (isInitializing) return;
        SettingManager.Instance.SetCombatVolume(value);
        combatText.text = ToPercent(value);
    }

    public void OnUIVolumeChanged(float value)
    {
        if (isInitializing) return;
        SettingManager.Instance.SetUIVolume(value);
        uiText.text = ToPercent(value);
    }

    // ── Toggle OnValueChanged 에 연결

    public void OnMuteToggleChanged(bool isMute)
    {
        if (isInitializing) return;
        SettingManager.Instance.SetMute(isMute);
    }

    // ── 버튼 OnClick 에 연결 

    /// 초기값 버튼. 모든 슬라이더를 50%로 리셋.
    public void OnResetButtonClicked()
    {
        SettingManager.Instance.ResetToDefault();
        InitializeUI(); // 슬라이더 UI도 동기화
    }

    /// 저장 & 닫기 버튼
    public void OnCloseButtonClicked()
    {
        SettingManager.Instance.SaveSettings();
        gameObject.SetActive(false);
    }

    private void RefreshAllTexts()
    {
        masterText.text = ToPercent(masterSlider.value);
        bgmText.text = ToPercent(bgmSlider.value);
        combatText.text = ToPercent(sfxSlider.value);
        uiText.text = ToPercent(uiSlider.value);
    }

    // 0.5f → "50%"
    private string ToPercent(float value) => $"{Mathf.RoundToInt(value * 100)}%";
}
