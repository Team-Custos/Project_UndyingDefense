using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadSceneAsync("LobbyScene_LoPol");  // 비동기적으로 로비 씬 로드
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Ingame_UIManager.instance.waveRestartBtn != null)
        {
            Time.timeScale = 1.0f;
            Ingame_UIManager.instance.waveRestartBtn.onClick.AddListener(RestartCurrentScene);
        }

        if (Ingame_UIManager.instance.lobbybtn != null)
        {
            Time.timeScale = 1.0f;
            Ingame_UIManager.instance.lobbybtn.onClick.AddListener(GoToLobby);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}