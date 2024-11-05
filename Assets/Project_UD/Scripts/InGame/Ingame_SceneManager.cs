using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//이 스크립트는 씬을 관리하기 위한 스크립트입니다.

public class Ingame_SceneManager : MonoBehaviour
{
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

    // 로비 씬으로 이동
    public void GoToLobby()
    {
        SceneManager.LoadSceneAsync(1);  // 비동기적으로 로비 씬 로드
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Ingame_WaveUIManager.instance.  winWaveRestartBtn != null)
        {
            Ingame_WaveUIManager.instance.winWaveRestartBtn.onClick.AddListener(RestartCurrentScene);
            Time.timeScale = 1.0f;
        }

        if (Ingame_WaveUIManager.instance.Winlobbybtn != null)
        {
            Ingame_WaveUIManager.instance.Winlobbybtn.onClick.AddListener(GoToLobby);
            Time.timeScale = 1.0f;
        }

        if (Ingame_WaveUIManager.instance.loseWaveRestartBtn != null)
        {
            Ingame_WaveUIManager.instance.loseWaveRestartBtn.onClick.AddListener(RestartCurrentScene);
            Time.timeScale = 1.0f;
        }

        if (Ingame_WaveUIManager.instance.loselobbybtn != null)
        {
            Ingame_WaveUIManager.instance.loselobbybtn.onClick.AddListener(GoToLobby);
            Time.timeScale = 1.0f;
        }

        if(Ingame_UIManager.instance.settingLobbyBtn != null)
        {
            Ingame_UIManager.instance.settingLobbyBtn.onClick.AddListener(GoToLobby);
            Time.timeScale = 1.0f;
        }

        if (Ingame_UIManager.instance.settingReStartBtn != null)
        {
            Ingame_UIManager.instance.settingReStartBtn.onClick.AddListener(RestartCurrentScene);
            Time.timeScale = 1.0f;
        }
    }
}