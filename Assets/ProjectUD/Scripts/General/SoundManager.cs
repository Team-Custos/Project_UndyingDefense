
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioSource sfxAudio;
    [SerializeField] private AudioSource sfxLoopAudio;

    [SerializeField] private AudioClip uiClickClip;
    [SerializeField] private AudioClip cancleClip;
    [SerializeField] private AudioClip unableClickClip;

    [SerializeField] AudioClip[] unitSfxClip;
    [SerializeField] AudioClip[] uiSfxClip;

    private Dictionary<AudioClip, AudioSource> loopSfxDic = new Dictionary<AudioClip, AudioSource>();

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


    public void PlaySFX(AudioClip clip)
    {
        sfxAudio.PlayOneShot(clip);
    }

    public void PlaySFX(params AudioClip[] clips)
    {
        if (clips == null)
            return;

        if (clips.Length > 0)
        {
            AudioClip clip = clips[Random.Range(0, clips.Length)];
            PlaySFX(clip);
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
        sfxAudio.PlayOneShot(uiClickClip);
    }

    public void PlayUnableUIClickSFX()
    {
        sfxAudio.PlayOneShot(unableClickClip);
    }

    public void PlayCancelUISFX()
    {
        sfxAudio.PlayOneShot(cancleClip);
    }

    //public void playCancleSFX()
    //{
    //    sfxAudio.PlayOneShot(cancleClip);
    //}
}
