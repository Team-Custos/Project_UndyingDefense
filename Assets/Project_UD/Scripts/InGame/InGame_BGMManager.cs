using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//이 스크립트는 인게임의 배경음악을 관리하기 위한 스크립트입니다.
public class InGame_BGMManager : MonoBehaviour
{
    public AudioClip[] BGM; //BGM들
    public AudioSource audioSource; //재생할 컴포넌트.


    // Start is called before the first frame update
    void Start()
    {
        if (audioSource != null)
        {
            audioSource = GetComponent<AudioSource>();
        }


        audioSource.clip = BGM[0];

        audioSource.Play();

        //if (!audioSource.isPlaying) //재생중이 아닐경우 재생.
        //{
        //    Debug.Log("BGM 재생");
        //    audioSource.Play();
        //}
    }

    // Update is called once per frame
    void Update()
    {

    }


    // BGM 일시 정지 
    public void PauseBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
}
