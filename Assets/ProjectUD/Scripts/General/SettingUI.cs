using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltEvents;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Localization;

public class SettingUI : MonoBehaviour, IInputESC
{
    [Header("인풋 이벤트 매니저")]
    [SerializeField] private PlayerInputEventManager inputEventManager;

    [Header("인게임 매니저")]
    [SerializeField] private InGameManager inGameManager;

    [Header("시스템 확인 창")]
    [SerializeField] private SystemConfirmUI systemConfirmUI;

    [Header("확인창 Localization - Close")]
    [SerializeField] private LocalizedString closeConfirmMessage;
    [SerializeField] private UltEvent BackEvent;

    [Header("확인창 Localization - Reset")]
    [SerializeField] private LocalizedString resetConfirmMessage;
    [SerializeField] private UltEvent ResetEvent;

    [Header("그래픽해상도")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle windowedModeToggle;

    [Header("그래픽품질")]
    [SerializeField] private TMP_Dropdown qualityDropdown;

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
        inputEventManager.OnESCTarget = this;
    }

    private void InitializeUI()
    {
        isInitializing = true;

        SettingManager sm = SettingManager.Instance;

        // 사운드 옵션 동기화
        masterSlider.value = sm.MasterVolume;
        bgmSlider.value = sm.BGMVolume;
        sfxSlider.value = sm.SFXVolume;
        uiSlider.value = sm.UIVolume;
        muteToggle.isOn = sm.IsMuted;

        // 그래픽 옵션 동기화
        //resolutionDropdown.value = sm.ResolutionIndex; => 추후 해상도 옵션 추가 시 구현
        //resolutionDropdown.value = 0;   //현재 1920x1080 옵션 하나뿐이므로 항상 0으로 초기화
        resolutionDropdown.value = sm.ApplyResolutionDropdownIndex(); // 저장된 해상도 인덱스 적용
        windowedModeToggle.isOn = !sm.IsFullScreen;

        // 품질 옵션 동기화
        qualityDropdown.value = QualitySettings.GetQualityLevel();        //sm.ApplyQualityDropdownIndex();

        RefreshAllTexts();

        isInitializing = false;
    }

    // 해상도 Dropdown 셋팅(아직미사용)
    private void SetSettingUI()
    {
        string[] options = SettingManager.Instance.GetResolutionString();
        for (int i = 0; i < options.Length; i++) 
        {
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(options[i]));         //TMP_Dropdown.OptionData)
        } 
    }

    // Dropdown OnValueChanged에 연결_해상도
    public void OnResolutionChanged(int index)
    {
        if (isInitializing) return;

        // 현재는 index 0 = 1920x1080 하나만 존재
        switch (index)
        {
            case 0: SettingManager.Instance.SetResolution(1920, 1080); break;
            case 1: SettingManager.Instance.SetResolution(1600, 900); break;
            case 2: SettingManager.Instance.SetResolution(1280, 720); break;
        }
    }

    // Dropdown OnValueChanged에 연결_그래픽 품질
    public void OnQualityChanged(int index)
    {
        if (isInitializing) return;

        // Unity의 QualitySettings에 맞춰 인덱스 전달
        switch (index)
        {
            case 0: SettingManager.Instance.SetQualityLevel(0); break; // Low
            case 1: SettingManager.Instance.SetQualityLevel(1); break; // Medium
            case 2: SettingManager.Instance.SetQualityLevel(2); break; // High
        }
    }

    // Toggle OnValueChanged에 연결
    public void OnWindowedModeToggleChanged(bool isWindowedMode)
    {
        if (isInitializing) return;
        SettingManager.Instance.SetFullScreen(!isWindowedMode);   // 토글이 창모드이므로 반대로 전달
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
    public void OnResetCorfirmButtonClicked()
    {
        SettingManager.Instance.ResetToDefault();
        InitializeUI(); // 슬라이더 UI도 동기화
    }

    /// 저장 & 닫기 버튼
    public void OnCloseButtonClicked()
    {
        if(systemConfirmUI.gameObject.activeSelf)
        {
            systemConfirmUI.gameObject.SetActive(false);
            return;
        }

        if (GlobalSoundManager.instance != null)
        {
            GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
        }

        SettingManager.Instance.SaveSettings();
        gameObject.SetActive(false);

        inputEventManager.OnESCTarget = null;

        if (inGameManager != null)
        {
            inputEventManager.OnESCTarget = inGameManager;
            Time.timeScale = 1.0f;
            //SoundManager.Instance.PlayCancelUISFX();
        }
            
    }


    private void OnDisable()
    {
        SettingManager.Instance.SaveSettings(); 
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

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnCloseButtonClicked();
        }
    }

    public void OnResetButtonClicked()
    {
        systemConfirmUI.SetConfirmUI(resetConfirmMessage, ResetEvent);
    }

    public void OnBackButtonClicked()
    {
        systemConfirmUI.SetConfirmUI(closeConfirmMessage, BackEvent);
    }
}
