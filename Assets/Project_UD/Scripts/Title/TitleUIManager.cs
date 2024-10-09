using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TitleUIManager : MonoBehaviour
{
    public Button GameStartBtn;
    public GameObject LoadingPanel;
    public Slider progressBar;
    public Text progressText;

    // Start is called before the first frame update
    void Start()
    {
        if (LoadingPanel != null)
        {
            LoadingPanel.SetActive(false);
        }

        if (GameStartBtn != null)
        {
            GameStartBtn.onClick.AddListener(() => LoadScene("LobbyScene"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        LoadingPanel.SetActive(true);

        progressBar.value = 0f;
        progressText.text = "0%";

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            yield return null;
        }

        LoadingPanel.SetActive(false);
    }

}
