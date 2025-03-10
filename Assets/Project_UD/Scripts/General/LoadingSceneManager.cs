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
    [SerializeField] private float loadingTime = 3.0f;

    


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
        operation.allowSceneActivation = false; // 씬 자동 활성화 방지

        float elapsedTime = 0f;

        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / loadingTime);

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

    }
}
