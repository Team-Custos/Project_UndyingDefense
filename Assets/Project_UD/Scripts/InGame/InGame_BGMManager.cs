using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//이 스크립트는 인게임의 배경음악을 관리하기 위한 스크립트입니다.
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
