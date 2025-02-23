using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroScene : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        // 영상이 끝났을 때 이벤트 등록
        videoPlayer.loopPointReached += (VideoPlayer vp) =>
        {
            FadeOut();
        };
    }

    void Update()
    {
        // 스페이스바를 누르면 영상 중지 및 페이드아웃 시작
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoPlayer.Stop();

            FadeOut();


            //Ingame_SceneManager.inst.GoToTitle(); // 로비 씬으로 이동
        }

    }

    private void FadeOut()
    {
        animator.SetFloat("animationSpeed", 1f);
    }

    private void LoadScene()
    {
        Ingame_SceneManager.inst.LoadScene("TitleScene");
    }
}