using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Animator animator;
    [SerializeField] private RawImage videoRawImage;


    // Start is called before the first frame update
    void Start()
    {
        videoRawImage.gameObject.SetActive(true);

        // 영상 재생
        videoPlayer.Play();


        // 영상이 끝났을 때 이벤트 등록
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        // 스페이스바를 누르면 영상 중지 및 페이드아웃 시작
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Stop();

            OnVideoEnd(videoPlayer);


            //Ingame_SceneManager.inst.GoToTitle(); // 로비 씬으로 이동
        }

    }

    void OnVideoEnd(VideoPlayer vp)
    {
        animator.SetBool("isFadeOut", true);

    }
}