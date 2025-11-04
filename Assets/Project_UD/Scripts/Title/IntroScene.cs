using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Video;
using UltEvents;

public class IntroScene : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;       // 비디오 플레이어 (영상 재생)
    private bool isVideoSkipped = false;                         // 영상이 스킵되었는지 확인
    private bool isStatementSkipped = false;
    

    [SerializeField] private AudioClip firstHalfBgm;        
    [SerializeField] private AudioClip bgSfx;               
    [SerializeField] private AudioClip startSfx;            

    [SerializeField] private float duration = 1.0f;         

    //[SerializeField] private Image statementImage;
    [SerializeField] private CanvasGroup statementCanvasGroup;              
    [SerializeField] private CanvasGroup declarationTransformCanvasGroup;
    [SerializeField] private CanvasGroup videoCanvasGroup;
    [SerializeField] private RectTransform declarationTransform;
    [SerializeField] private CanvasGroup dialogueCanvasGroup;
    [SerializeField] private UltEvent nextDialogue;
    [SerializeField] private GameObject skipBtn;

    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log($"인트로영상{PlayerPrefs.GetInt("IntroVideo")}");

        if(PlayerPrefs.GetInt("IntroVideo") == 0)
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += vp => OnVideoFinished();
            // 영상 소리 페이드 아웃 예약
            ScheduleVideoAudioFadeOut();
        }
        else
        {
            OnVideoFinished();
        }

            
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space) && !isSkipped)
    //    {
    //        isSkipped = true;

    //        videoPlayer.Stop();
    //        OnVideoFinished(videoPlayer); // 강제로 처리
    //    }
    //}

    public void SkipVideo() // 인트로 
    {
        if(!isVideoSkipped)
        {
            isVideoSkipped = true;

            videoPlayer.Stop();
            OnVideoFinished();
            skipBtn.SetActive(false);
        }
        //else
        //{
        //    if(isStatementSkipped) return;

        //    ShowDialogue();
        //    isStatementSkipped = true;
        //}
    }

    private void OnVideoFinished()
    {
        videoPlayer.gameObject.SetActive(false);

        // 1. 첫 BGM 재생
        SoundManager.Instance.PlayBGM(firstHalfBgm);

        // 2. statement 페이드 인
        //FadeInStatementImage();
        skipBtn.SetActive(false);
        statementCanvasGroup.gameObject.SetActive(true);

        // 3. firstHalfBgm 길이만큼 후에 다음 단계 실행
        //Invoke(nameof(OnFirstBgmEnded), firstHalfBgm.length);

        PlayerPrefs.SetInt("IntroVideo", 1);
    }

    private void FadeInStatementImage()
    {
        if (statementCanvasGroup == null) return;

        //statementCanvasGroup.gameObject.SetActive(true);
        //statementCanvasGroup.alpha = 0;
        //statementCanvasGroup.DOFade(1f, duration)
        //    .SetEase(Ease.OutQuad);
        OnVideoFinished();
    }

    private void OnFirstBgmEnded()
    {
        if(isStatementSkipped) return;


        // 대화창 
        ShowDialogue();
    }

    private void ShowDialogue()
    {
        nextDialogue.Invoke();
        dialogueCanvasGroup.gameObject.SetActive(true);

        //dialogueCanvasGroup.alpha = 0;
        //dialogueCanvasGroup.DOFade(1f, duration)
        //    .SetEase(Ease.OutQuad);
    }

    public void PlayDeclarationDropAnimation()
    {
        declarationTransform.DOAnchorPosY(0, duration).SetEase(Ease.InOutSine);
        SoundManager.Instance.PlaySFX(bgSfx);
    }

    private void ScheduleVideoAudioFadeOut()
    {
        double videoDuration = videoPlayer.length;

        // 끝나기 1.5초 전에 페이드아웃 시작
        double fadeOutStartTime = videoDuration - 3.0f;

        if (fadeOutStartTime > 0)
            Invoke(nameof(FadeOutVideoAudio), (float)fadeOutStartTime);
    }

    private void FadeOutVideoAudio()
    {
        float currentVolume = videoPlayer.GetDirectAudioVolume(0); // 첫 번째 오디오 트랙
        DOTween.To(() => currentVolume, x =>
        {
            videoPlayer.SetDirectAudioVolume(0, x);
        }, 0f, 1f); // 1.5초 동안 0까지 감소
    }

    public void LoadScene()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(videoCanvasGroup.DOFade(0f, 0.5f));
        seq.Join(statementCanvasGroup.DOFade(0f, 0.5f));
        seq.Join(dialogueCanvasGroup.DOFade(0f, 0.5f));

        seq.Append(declarationTransformCanvasGroup.DOFade(0f, duration));


        //SoundManager.Instance.PlaySFX(startSfx);

        seq.OnComplete(() =>
        {
            LoadingSceneManager.LoadScene("LobbyScene_LoPol");
        });
    }

    public void LoadLobbyScene()
    {
        LoadingSceneManager.LoadScene("LobbyScene_LoPol");
    }


}