using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolAudioSource : MonoBehaviour
{
    private SoundManager soundManager;
    [SerializeField] private AudioSource audioSource;

    public AudioSource Audio => audioSource;

    public void Initialize(SoundManager soundManager)
    {
        this.soundManager = soundManager;
    }

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            soundManager.ReturnAudioSource(audioSource);
            audioSource.clip = null;
            enabled = false;
            
        }
    }

    public void Activate()
    {
        enabled = true;
    }
}
