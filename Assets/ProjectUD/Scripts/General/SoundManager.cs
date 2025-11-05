
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private Transform cameraPos;

    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioSource sfxAudio;
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
        sfxAudio.PlayOneShot(clip);
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
