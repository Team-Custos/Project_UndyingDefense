using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    // 로딩씬 다음 설정해주는 변수  씬 a -> 로딩씬 -> 씬 b 
    private static string nextScene;
    [SerializeField] private Image progressImage;
    [SerializeField] private Text progressText;
    [SerializeField] private float loadingTime = 0.0f;

    


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }


    private IEnumerator LoadSceneProcess()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nextScene);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            loadingTime += Time.deltaTime;

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress >= 0.9f) // 씬 로딩이 완료되었을 때
            {
                if (loadingTime >= 3.0f)
                {
                    progress = 1.0f;
                    operation.allowSceneActivation = true;
                }
                else
                {
                    progress = Mathf.Lerp(0.9f, 1.0f, loadingTime / 3.0f); // 90%에서 100%까지 서서히 증가
                }
            }

            if (progressImage != null)
                progressImage.fillAmount = progress;

            if (progressText != null)
                progressText.text = (progress * 100f).ToString("F0") + "%";

            yield return null;
        }
    }
}
