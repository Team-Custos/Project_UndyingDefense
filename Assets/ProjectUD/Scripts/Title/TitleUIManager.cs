using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine.Utility;
using DG.Tweening;

public class TitleUIManager : MonoBehaviour
{
    public Button gameStartBtn;
    public Button gameEndBtn;
    public GameObject loadingPanel;
    public Image progressImage;
    public Text progressText;
    [SerializeField] private Button creditBtn;
    [SerializeField] private Button creditCloseBtn;
    [SerializeField] private GameObject creditPanel;

    public RectTransform backgroundPanel;
    public RectTransform titleText;
    public RectTransform gameBtnPanel;

    public float animationDuration = 0.5f; // 연출 지속 시간
    public float delayBetweenAnimations = 0.2f; // 각 ui delay 시간

    public float minLoadingTime = 3f; // 씬 로딩 시간 3초로 고정

    [SerializeField] private AudioClip titleBgm;
    [SerializeField] private AudioClip startSfx;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(titleBgm);


        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

        if (gameStartBtn != null)
        {
            gameStartBtn.onClick.AddListener(() =>
            {
                if (PlayerPrefs.GetInt("IntroVideo") == 0)
                {
                    SoundManager.Instance.PlaySFX(startSfx);

                    LoadingSceneManager.LoadScene("IntroScene");

                }
                else
                {
                    SoundManager.Instance.PlaySFX(startSfx);

                    LoadingSceneManager.LoadScene("LobbyScene_LoPol");
                }

                //SoundManager.Instance.PlaySFX(startSfx);

                //LoadingSceneManager.LoadScene("IntroScene");

                // --- 저장데이터 초기화


                //DOTween.Sequence()
                //.AppendInterval(startSfx.length)
                //.OnComplete(() =>
                //    {
                //         LoadingSceneManager.LoadScene("IntroScene");
                //     });

                //SoundManager.Instance.StopBGM();
            });

        }

        

        if (gameEndBtn != null)
        {
            gameEndBtn.onClick.AddListener(() =>
            {
                EndGame();
            });
        }

        if (creditBtn != null)
        {
            creditBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                SoundManager.Instance.PlayUIClickSFX();
                creditPanel.SetActive(true);
            });
        }

        if (creditCloseBtn != null)
        {
            creditCloseBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                //SoundManager.Instance.playCancleSFX();
                creditPanel.SetActive(false);
            });
        }

        StartCoroutine(PlayTitleSceneAnimation());
    }

    public void LoadScene(int sceneNumber)
    {
        StartCoroutine(LoadSceneAsync(sceneNumber));
    }

    private IEnumerator LoadSceneAsync(int sceneNumber)
    {
        loadingPanel.SetActive(true);

        progressImage.fillAmount = 0f;
        progressText.text = "0%";

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneNumber);
        operation.allowSceneActivation = false; // 씬 자동 활성화 방지

        float elapsedTime = 0f;
        float totalTime = 3f; // 프로그래스바 연출 시간

        while (elapsedTime < totalTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / totalTime);

            progressImage.fillAmount = progress;
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        // 프로그래스바 연출이 끝난 후 실제 씬 로드 완료 여부 체크
        while (!operation.isDone)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true; // 씬 활성화
            }
            yield return null;
        }

        loadingPanel.SetActive(false);
    }


    IEnumerator PlayTitleSceneAnimation()
    {
        backgroundPanel.localScale = Vector3.zero;
        titleText.localScale = Vector3.zero;
        gameBtnPanel.localScale = Vector3.zero;

        // 1번 배경 이미지
        yield return StartCoroutine(ScaleUp(backgroundPanel));

        // 2번 타이틀 텍스트
        //yield return new WaitForSeconds(delayBetweenAnimations);
        //yield return StartCoroutine(ScaleUp(titleText));

        // 3번 게임 버튼
        yield return new WaitForSeconds(delayBetweenAnimations);
        yield return StartCoroutine(ScaleUp(gameBtnPanel));
    }

    IEnumerator ScaleUp(RectTransform uiElement)
    {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;

        float time = 0.0f;

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            uiElement.localScale = Vector3.Lerp(startScale, endScale, time / animationDuration);
            yield return null;
        }

        uiElement.localScale = endScale;
    }

    void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

    }


    public void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
        SettingManager.Instance.ResetToDefault();
    }
}