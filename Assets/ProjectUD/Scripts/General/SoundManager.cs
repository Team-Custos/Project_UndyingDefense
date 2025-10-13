
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{

    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioSource sfxAudio;
    [SerializeField] private AudioSource sfxLoopAudio;

    [SerializeField] private AudioClip uiClickClip;
    [SerializeField] private AudioClip cancleClip;

    [SerializeField] AudioClip[] unitSfxClip;
    [SerializeField] AudioClip[] uiSfxClip;

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

    public void PlaySFXLoop(AudioClip clip)
    {
        sfxLoopAudio.clip = clip;
        sfxLoopAudio.Play();
    }

    public void StopSFXLoop()
    {
        if (sfxLoopAudio != null)
            sfxLoopAudio.Stop();
    }

    public void PlayUIClickSFX()
    {
        sfxAudio.PlayOneShot(uiClickClip);
    }


    //public void playCancleSFX()
    //{
    //    sfxAudio.PlayOneShot(cancleClip);
    //}
}
