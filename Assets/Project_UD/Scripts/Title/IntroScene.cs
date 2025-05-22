using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroScene : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Animator animator;
    private bool isSkipped = false;

    [SerializeField] private AudioClip firstHalfBgm;
    [SerializeField] private AudioClip secondHalfBgm;
    [SerializeField] private AudioClip bgSfx;
    [SerializeField] private AudioClip startSfx;

    [SerializeField] private float duration = 1.0f;

    //[SerializeField] private Image statementImage;
    [SerializeField] private CanvasGroup statementCanvasGroup;
    [SerializeField] private CanvasGroup declarationTransformCanvasGroup;
    [SerializeField] private CanvasGroup videoCanvasGroup;
    [SerializeField] private RectTransform declarationTransform;

    // Start is called before the first frame update
    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isSkipped)
        {
            isSkipped = true;

            videoPlayer.Stop();
            OnVideoFinished(videoPlayer); // 강제로 처리
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // 1. 첫 BGM 재생
        SoundManager.Instance.PlaySFX(firstHalfBgm);

        // 2. statement 페이드 인
        FadeInStatementImage();

        // 3. firstHalfBgm 길이만큼 후에 다음 단계 실행
        Invoke(nameof(OnFirstBgmEnded), firstHalfBgm.length);
    }

    private void FadeInStatementImage()
    {
        if (statementCanvasGroup == null) return;

        statementCanvasGroup.alpha = 0;
        statementCanvasGroup.DOFade(1f, duration)
            .SetEase(Ease.OutQuad);
    }

    private void OnFirstBgmEnded()
    {
        // 4. 두 번째 BGM 재생
        SoundManager.Instance.PlayBGM(secondHalfBgm);

        // 5. 선언문 슬라이드 연출
        PlayDeclarationDropAnimation();
    }

    private void PlayDeclarationDropAnimation()
    {
        declarationTransform.DOAnchorPosY(0, duration).SetEase(Ease.InOutSine);
        SoundManager.Instance.PlaySFX(bgSfx);
    }

    public void LoadScene()
    {
        Sequence seq = DOTween.Sequence();
        seq.Join(statementCanvasGroup.DOFade(0f, duration));
        seq.Join(declarationTransformCanvasGroup.DOFade(0f, duration));
        seq.Join(videoCanvasGroup.DOFade(0f, duration));

        SoundManager.Instance.PlaySFX(startSfx);

        seq.OnComplete(() =>
        {
            LoadingSceneManager.LoadScene("LobbyScene_LoPol");
        });
    }

    private void FadeOut()
    {
        animator.SetFloat("animationSpeed", 1f);
    }

}