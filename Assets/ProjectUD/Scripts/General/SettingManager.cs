using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingManager : Singleton<SettingManager>
{
    // PlayerPrefs 키
    private const string KEY_MASTER = "Vol_Master";
    private const string KEY_BGM = "Vol_BGM";
    private const string KEY_SFX = "Vol_SFX";
    private const string KEY_UI = "Vol_UI";
    private const string KEY_MUTE = "Vol_Mute";
    private const string KEY_QUALITY = "QualityLevel";
    private const string KEY_RESOLUTION = "ResolutionIndex";

    // 현재 볼륨 상태 (0~1 정규화 값, UI와 동기화됨)
    public float MasterVolume { get; private set; } = 0.5f;
    public float BGMVolume { get; private set; } = 0.5f;
    public float SFXVolume { get; private set; } = 0.5f;
    public float UIVolume { get; private set; } = 0.5f;
    public bool IsMuted { get; private set; } = false;
    //public int ResolutionIndex { get; private set; } = 0;   // 현재 해상도 인덱스 (추후 확장)
    //public int QualityLevel => QualitySettings.GetQualityLevel();

    // 그래픽 설정 (추후 확장)
    public bool IsFullScreen { get; private set; } = true;
    private Resolution[] resolutions;

    protected override void Awake()
    {
        base.Awake();
        // SoundManager.Start()보다 먼저 값을 로드
        LoadSettings();

        resolutions = Screen.resolutions;
        for(int i = 0; i < resolutions.Length; i++) {
            Debug.Log($"지원 해상도 {i+1} : {resolutions[i].width} X {resolutions[i].height}");
        }
        
    }

    private void Start()
    {
        // SoundManager가 초기화된 이후 시점에 저장된 볼륨 일괄 적용
        ApplyAllToSoundManager();
    }

    // ── 그래픽 설정 변경 (추후 확장)
    public void SetFullScreen(bool isFullScreen)
    {
        IsFullScreen = isFullScreen;
        Screen.fullScreen = isFullScreen;
    }

    // ── 해상도 종류 반환
    public Resolution[] GetResolutions()
    {
        return resolutions;
    }

    public string[] GetResolutionString()
    {
        string[] options = new string[resolutions.Length];
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].ToString();
            options[i] = option;
        }

        return options;
    }

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, IsFullScreen);
    }

    // ── 그래픽 품질 변경
    public void SetQualityLevel(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex, true);
    }

    // ── 볼륨 변경

    public void SetMasterVolume(float value)
    {
        MasterVolume = value;
        SoundManager.Instance.SetMasterVolume(value);
    }

    public void SetBGMVolume(float value)
    {
        BGMVolume = value;
        SoundManager.Instance.SetBGMVolume(value);
    }

    public void SetCombatVolume(float value)
    {
        SFXVolume = value;
        SoundManager.Instance.SetCombatVolume(value);
    }

    public void SetUIVolume(float value)
    {
        UIVolume = value;
        SoundManager.Instance.SetUIVolume(value);
    }

    /// 음소거 상태 변경.
    public void SetMute(bool isMute)
    {
        IsMuted = isMute;
        SoundManager.Instance.SetMute(isMute);
    }

    // ── 저장 / 로드 

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(KEY_MASTER, MasterVolume);
        PlayerPrefs.SetFloat(KEY_BGM, BGMVolume);
        PlayerPrefs.SetFloat(KEY_SFX, SFXVolume);
        PlayerPrefs.SetFloat(KEY_UI, UIVolume);
        PlayerPrefs.SetInt(KEY_MUTE, IsMuted ? 1 : 0);
        PlayerPrefs.SetInt(KEY_QUALITY, QualitySettings.GetQualityLevel());
        PlayerPrefs.SetInt(KEY_RESOLUTION, 0); // 현재 해상도 옵션 하나뿐이므로 항상 0 저장
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        if (PlayerPrefs.GetInt("IntroVideo") == 0)
        {
            ResetToDefault();
            return;
        }

        MasterVolume = PlayerPrefs.GetFloat(KEY_MASTER);
        BGMVolume = PlayerPrefs.GetFloat(KEY_BGM);
        SFXVolume = PlayerPrefs.GetFloat(KEY_SFX);
        UIVolume = PlayerPrefs.GetFloat(KEY_UI);
        IsMuted = PlayerPrefs.GetInt(KEY_MUTE) == 1;
        ApplyQualityDropdownIndex(); // 저장된 품질 인덱스 적용
    }

    /// 초기값 버튼 — 모든 볼륨을 50%로 초기화하고 즉시 저장.
    public void ResetToDefault()
    {
        SetMasterVolume(0.5f);
        SetBGMVolume(0.5f);
        SetCombatVolume(0.5f);
        SetUIVolume(0.5f);
        SetMute(false);
        SaveSettings();
    }

    // ── 내부 
    private void ApplyAllToSoundManager()
    {
        SoundManager.Instance.SetMasterVolume(MasterVolume);
        SoundManager.Instance.SetBGMVolume(BGMVolume);
        SoundManager.Instance.SetCombatVolume(SFXVolume);
        SoundManager.Instance.SetUIVolume(UIVolume);
        SoundManager.Instance.SetMute(IsMuted);
    }

    public void ApplyQualityDropdownIndex()
    {
        // 그래픽 품질 Dropdown과 동기화
        int qualityIndex = PlayerPrefs.GetInt(KEY_QUALITY);
        QualitySettings.SetQualityLevel(qualityIndex, true);    // 저장된 품질 인덱스 적용
        //qualityDropdown.value = qualityIndex; => 추후 그래픽 품질 옵션 추가 시 구현
        //return qualityIndex;
    }

    public int ApplyResolutionDropdownIndex()
    {
        // 해상도 Dropdown과 동기화
        int resolutionIndex = PlayerPrefs.GetInt(KEY_RESOLUTION);
        return resolutionIndex;
    }
}
