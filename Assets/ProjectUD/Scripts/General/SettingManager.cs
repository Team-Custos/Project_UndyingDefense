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

    // 현재 볼륨 상태 (0~1 정규화 값, UI와 동기화됨)
    public float MasterVolume { get; private set; } = 0.5f;
    public float BGMVolume { get; private set; } = 0.5f;
    public float SFXVolume { get; private set; } = 0.5f;
    public float UIVolume { get; private set; } = 0.5f;
    public bool IsMuted { get; private set; } = false;

    // 그래픽 설정 (추후 확장)
    public bool IsFullScreen { get; private set; } = true;

    protected override void Awake()
    {
        base.Awake();
        // SoundManager.Start()보다 먼저 값을 로드
        LoadSettings();
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

    public void SetResolution(int width, int height)
    {
        Screen.SetResolution(width, height, IsFullScreen);
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
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat(KEY_MASTER, 0.5f);
        BGMVolume = PlayerPrefs.GetFloat(KEY_BGM, 0.5f);
        SFXVolume = PlayerPrefs.GetFloat(KEY_SFX, 0.5f);
        UIVolume = PlayerPrefs.GetFloat(KEY_UI, 0.5f);
        IsMuted = PlayerPrefs.GetInt(KEY_MUTE, 0) == 1;
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
}
