using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private Image progressImage;
    [SerializeField] private Text progressText;
    [SerializeField] private float loadingTime = 0.0f;

    // 로딩씬 다음 설정해주는 변수  a -> 로딩씬 -> b 
    private string nextSceneName;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadScene("IntroScene"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        
        while(!operation.isDone)
        {
            loadingTime += Time.deltaTime;

            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            if (operation.progress >= 0.9f) // 씬 로딩이 완료되었을 때
            {
                if (loadingTime >= 3.0f)
                {
                    operation.allowSceneActivation = true;
                }
                else
                {
                    progress = Mathf.Clamp01(loadingTime / 1.0f);
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
