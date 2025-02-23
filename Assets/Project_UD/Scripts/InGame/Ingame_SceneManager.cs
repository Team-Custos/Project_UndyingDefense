using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//이 스크립트는 씬을 관리하기 위한 스크립트입니다.

public class Ingame_SceneManager : MonoBehaviour
{
    public static Ingame_SceneManager inst;

    public GameObject loadingPanel;
    public Image progressImage;
    public Text progressText;

    private void Awake()
    {
        inst = this;

        DontDestroyOnLoad(this.gameObject);
    }

    public void _nextScene(int sceneIdx)
    {
        if (sceneIdx >= 0)
        {
            SceneManager.LoadSceneAsync(sceneIdx);
        }
    }

    public void RestartCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();  // 현재 활성화된 씬 가져오기
        SceneManager.LoadSceneAsync(currentScene.buildIndex);  // 비동기적으로 현재 씬 다시 로드
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void GoToTitle()
    {
        SceneManager.LoadSceneAsync(0);  // 비동기적으로 타이틀 씬 로드
    }

    // 로비 씬으로 이동
    public void GoToLobby()
    {
        Ingame_UIManager.instance.settingPanel.gameObject.SetActive(false);
        StartCoroutine(LoadSceneAsyncCorutine(1)); 

        //SceneManager.LoadSceneAsync(1);  // 비동기적으로 로비 씬 로드
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (Ingame_WaveUIManager.instance.  winWaveRestartBtn != null)
        //{
        //    Ingame_WaveUIManager.instance.winWaveRestartBtn.onClick.AddListener(() =>
        //    {
        //        Time.timeScale = 1.0f;
        //        SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_click);
        //        RestartCurrentScene();
                
        //    });
        //}

        //if (Ingame_WaveUIManager.instance.winLobbyBtn != null)
        //{
        //    Ingame_WaveUIManager.instance.winLobbyBtn.onClick.AddListener(() =>
        //    {
        //        Time.timeScale = 1.0f;
        //        SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_exit);
        //        GoToLobby();
        //    });
        //}

        //if (Ingame_WaveUIManager.instance.loseWaveRestartBtn != null)
        //{
        //    Ingame_WaveUIManager.instance.loseWaveRestartBtn.onClick.AddListener(() =>
        //    {
        //        Time.timeScale = 1.0f;
        //        SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_click);
        //        RestartCurrentScene();
        //    });
                
        //}

        //if (Ingame_WaveUIManager.instance.loseLobbyBtn != null)
        //{
        //    Ingame_WaveUIManager.instance.loseLobbyBtn.onClick.AddListener(() =>
        //    {
        //        Time.timeScale = 1.0f;
        //        SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_exit);
        //        GoToLobby();
        //    });
            
        //}

        //if (Ingame_UIManager.instance.settingLobbyBtn != null)
        //{
        //    Ingame_UIManager.instance.settingLobbyBtn.onClick.AddListener(() =>
        //    {
        //        Time.timeScale = 1.0f;
        //        SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_exit);
        //        GoToLobby();
        //    });


        //}

        //if (Ingame_UIManager.instance.settingReStartBtn != null)
        //{
        //    Ingame_UIManager.instance.settingReStartBtn.onClick.AddListener(() =>
        //    {
        //        Time.timeScale = 1.0f;
        //        SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_click);
        //        RestartCurrentScene();
        //    });

        //}
    }


    private IEnumerator LoadSceneAsyncCorutine(int sceneIdx)
    {

        // 로딩 패널 활성화
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
        }

        // 씬 로딩 시작
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIdx);
        operation.allowSceneActivation = false; // 씬 자동 전환 방지

        while (!operation.isDone)
        {
            // 로딩 진행도 (0.0 ~ 1.0)
            float progress = Mathf.Clamp01(operation.progress / 1f);
            if (progressImage != null)
                progressImage.fillAmount = progress;
           
            if (progressText != null)
                progressText.text = (progress * 100f).ToString("F0") + "%";
           

           

            // 로딩이 90% 이상일 때 (Unity는 실제 로딩 완료 시점이 0.9임)
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // 씬 로딩 완료 후 로딩 패널 비활성화
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
        }

    }


}