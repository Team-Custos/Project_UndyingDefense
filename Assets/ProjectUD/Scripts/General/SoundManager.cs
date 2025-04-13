using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioSource sfxAudio;

    public void PlayBGM(AudioClip clip)
    {
        if (bgmAudio.clip == clip)
            return;

        bgmAudio.clip = clip;
        bgmAudio.Play();
    }

    public void StopBGM()
    {
        bgmAudio.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxAudio.PlayOneShot(clip);
    }
}
