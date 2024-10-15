using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame_BGMManager : MonoBehaviour
{
    public AudioClip[] BGM;
    public AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        if (audioSource != null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.clip = BGM[0];
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
