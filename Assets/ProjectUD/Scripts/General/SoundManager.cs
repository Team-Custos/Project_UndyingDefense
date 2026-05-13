
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private Transform cameraPos;

    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioSource bgmOneShotAudio;  // sfx->bgm
    [SerializeField] private AudioSource uiAudio;
    [SerializeField] private AudioSource sfxLoopAudio;

    [SerializeField] private AudioClip uiClickClip;
    [SerializeField] private AudioClip cancleClip;
    [SerializeField] private AudioClip unableClickClip;

    [SerializeField] AudioClip[] unitSfxClip;
    [SerializeField] AudioClip[] uiSfxClip;

    [SerializeField] private GameObject audioSourcePrefab;
    [SerializeField] float maxDistance = 50f;
    [SerializeField] float minDistance = 15f;

    private ObjectPoolWithList<AudioSource> audioSourcePool;
    private Dictionary<AudioClip, AudioSource> loopSfxDic = new Dictionary<AudioClip, AudioSource>();

    // ── 볼륨 상태 (0~1 정규화) 
    // SettingManager > Awake > LoadSettings > Start
    private float masterVolume = 1f;
    private float bgmVolume = 1f;
    private float combatVolume = 1f;
    private float uiVolume = 1f;

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        ApplyBGMVolume();
        ApplyUIVolume();
        ApplyCombatVolume();
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        ApplyBGMVolume();
    }

    public void SetCombatVolume(float value)
    {
        combatVolume = value;
        ApplyCombatVolume();
    }

    public void SetUIVolume(float value)
    {
        uiVolume = value;
        ApplyUIVolume();
    }

    public void SetMute(bool isMute)
    {
        bgmAudio.mute = isMute;
        bgmOneShotAudio.mute = isMute;

        if (audioSourcePool != null)
            foreach (var src in audioSourcePool.List)
                src.mute = isMute;
    }

    // ── 볼륨 내부 _ 마스터 × 개별 = 실제 볼륨
    private void ApplyBGMVolume()
    {
        bgmAudio.volume = masterVolume * bgmVolume;
        bgmOneShotAudio.volume = masterVolume * bgmVolume;
    }

    private void ApplyUIVolume()
    {
        //bgmOneShotAudio.volume = masterVolume * uiVolume; //기존 sfxAudio
        uiAudio.volume = masterVolume * uiVolume;
    }

    private void ApplyCombatVolume()
    {
        if (audioSourcePool == null) return;
        foreach (var src in audioSourcePool.List)
            src.volume = masterVolume * combatVolume;
    }

    //---------------------------------------------------
    public void PlayBGM(AudioClip clip)
    {
        //if (bgmAudio.clip == clip)
        //    return;

        bgmAudio.clip = clip;
        bgmAudio.Play();
    }

    public void StopBGM()
    {
        if(bgmAudio != null)
            bgmAudio.Stop();
    }


    public void PlaySFX(AudioClip clip, Vector3 pos)
    {
        AudioSource audioSource = GetAudioSource();

        if(audioSource != null)
        {
            audioSource.transform.position = pos;

            audioSource.minDistance = minDistance;
            audioSource.maxDistance = maxDistance;

            audioSource.PlayOneShot(clip);
            audioSource.GetComponent<PoolAudioSource>().Activate();
        }


        //sfxAudio.PlayOneShot(clip);
    }

    public void PlaySFX(AudioClip clip) // UI 용
    {
        bgmOneShotAudio.PlayOneShot(clip);
    }

    public void PlaySFX(Vector3 pos, params AudioClip[] clips)
    {
        if (clips == null)
            return;

        if (clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            PlaySFX(clip, pos);
        }
    }

    public void PlayLoopSFX(AudioClip clip)
    {
        if(!loopSfxDic.ContainsKey(clip))
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            loopSfxDic.Add(clip, audioSource);
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            loopSfxDic.TryGetValue(clip, out AudioSource audioSource);
            audioSource.clip = clip;
            audioSource.loop = true;
            Debug.Log("재생");
            audioSource.Play();
        }
    }

    public void StopLoopSFX(AudioClip clip)
    {
        if (!loopSfxDic.ContainsKey(clip))
            return;

        loopSfxDic.TryGetValue(clip, out AudioSource audioSource);
        audioSource.Stop();
    }

    //public void PlaySFXLoop(AudioClip clip) 
    //{
    //    sfxLoopAudio.clip = clip;  
    //    sfxLoopAudio.Play(); 
    //}

    //public void StopSFXLoop() 
    //{
    //    if (sfxLoopAudio != null) 
    //        sfxLoopAudio.Stop(); 
    //}

    public void PlayUIClickSFX()
    {
        //bgmOneShotAudio.PlayOneShot(uiClickClip);
        //uiAudio.PlayOneShot(uiClickClip);
    }

    public void PlayUnableUIClickSFX()
    {
        //bgmOneShotAudio.PlayOneShot(unableClickClip);
        uiAudio.PlayOneShot(unableClickClip);
    }

    public void PlayCancelUISFX()
    {
        //bgmOneShotAudio.PlayOneShot(cancleClip);
        uiAudio.PlayOneShot(cancleClip);
    }


    public AudioSource GetAudioSource()
    {
        if (audioSourcePool == null)
            audioSourcePool = new ObjectPoolWithList<AudioSource>(() => CreateAudioSource());

        AudioSource audioSource = audioSourcePool.Pool.Get();
        audioSourcePool.List.Add(audioSource);
        return audioSource;
    }


    public AudioSource CreateAudioSource()
    {
        GameObject obj = Instantiate(audioSourcePrefab);
        obj.transform.SetParent(transform);

        PoolAudioSource poolAudioSource = obj.GetComponent<PoolAudioSource>();
        AudioSource audioSource = poolAudioSource.Audio;

        poolAudioSource.Initialize(this);

        return audioSource;
    }

    public void ReturnAudioSource(AudioSource source)
    {
        audioSourcePool.List.Remove(source);
        audioSourcePool.Pool.Release(source);
    }
}
