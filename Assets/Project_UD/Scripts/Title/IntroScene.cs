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

    [Header("2배속 기능")]
    [SerializeField] private GameObject speedBtn;           // 2배속 버튼 (선택)
    [SerializeField] private Animator speedBtnAnim;
    private bool isFastForward = false;
    private double fadeOutVideoTime = -1; // 페이드아웃이 시작되어야 하는 "영상 재생 시간" 기준점


    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log($"인트로영상{PlayerPrefs.GetInt("IntroVideo")}");

        if(PlayerPrefs.GetInt("IntroVideo") == 0)
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += vp => OnVideoFinished();
            // 영상 소리 페이드 아웃 예약
            //ScheduleVideoAudioFadeOut();

            // 영상 소리 페이드 아웃 시점 계산 (영상 길이 - 3초 지점) _ 260701 2배속 구현중 코드 변경
            fadeOutVideoTime = videoPlayer.length - 3.0;

            // 매 프레임 영상 재생 시간을 체크해서 페이드아웃 트리거
            videoPlayer.frame = 0; // 안전하게 초기화
        }
        else
        {
            OnVideoFinished();
        }

            
    }

    // 260701 2배속 기능 구현중
    void Update()
    {
        // 영상 재생 중일 때만 체크
        if (videoPlayer.gameObject.activeSelf && videoPlayer.isPlaying && fadeOutVideoTime > 0)
        {
            if (videoPlayer.time >= fadeOutVideoTime)
            {
                FadeOutVideoAudio();
                fadeOutVideoTime = -1; // 한 번만 실행
            }
        }
    }

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

    // 2배속 버튼 메서드
    public void ToggleVideoSpeed()
    {
        isFastForward = !isFastForward;
        videoPlayer.playbackSpeed = isFastForward ? 2f : 1f;

        if(isFastForward)
        {
            speedBtnAnim.SetBool("VideoSpeedDouble", true);
        }
        else
        {
            speedBtnAnim.SetBool("VideoSpeedDouble", false);
        }
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
    }

    public void PlayDeclarationDropAnimation()
    {
        declarationTransform.DOAnchorPosY(0, duration).SetEase(Ease.InOutSine);
        SoundManager.Instance.PlaySFX(bgSfx);
    }

    //private void ScheduleVideoAudioFadeOut()
    //{
    //    double videoDuration = videoPlayer.length;

    //    // 끝나기 1.5초 전에 페이드아웃 시작
    //    double fadeOutStartTime = videoDuration - 3.0f;

    //    if (fadeOutStartTime > 0)
    //        Invoke(nameof(FadeOutVideoAudio), (float)fadeOutStartTime);
    //}

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