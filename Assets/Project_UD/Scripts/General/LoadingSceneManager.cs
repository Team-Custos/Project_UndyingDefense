using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Localization.Settings;

public class LoadingSceneManager : MonoBehaviour
{
    // 로딩씬 다음 설정해주는 변수  씬 a -> 로딩씬 -> 씬 b 
    private static string nextScene;
    [SerializeField] private Image progressImage;
    [SerializeField] private Text progressText;
    [SerializeField] private float loadingTime = 3.0f;

    [SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private int tipCount = 20;
    //[SerializeField] private TipTextData[] tipTextData;




    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.StopBGM();

        SetTipText();
        //if (tipText != null) tipText.text = "";

        StartCoroutine(LoadSceneProcess());

        //StartCoroutine(SetTipTextRoutine());
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

    public void SetTipText()
    {
        //int randomIndex = Random.Range(0, tipTextData.Length);
        //string tip = tipTextData[randomIndex].TipText;
        //tipText.text = tip;

        int randomIndex = Random.Range(1, tipCount+1);  // 꿀팁 로컬ID 시작 인덱스 = 1
        string key = $"TIP_loadingTip{randomIndex}";
        //Debug.Log($"id : {key}");
        tipText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("LoadingUI", key, LocalizationSettings.SelectedLocale);
    }

    // 사용 X. Localization 시스템이 켜질 때까지 대기하고 보여주는 코루틴
    private IEnumerator SetTipTextRoutine()
    {
        // 로컬라이제이션 시스템 초기화가 끝날 때까지 대기 (첫 프레임 씹힘 방지)
        yield return LocalizationSettings.InitializationOperation;
        Debug.Log("로컬라이징 셋팅 초기화완료");

        int randomIndex = Random.Range(1, tipCount + 1);
        string key = $"TIP_loadingTip{randomIndex}";

        // 비동기로 안전하게 로드 요청
        var op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("LoadingUI", key);

        // 텍스트 로드가 완전히 완료될 때까지 이 코루틴 안에서 대기합니다.
        yield return op;
        Debug.Log("로컬라이징 셋팅 불러오기 완료");

        if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            // 완벽하게 글자를 가져온 그 '순간'에 딱 한 번만 텍스트를 집어넣습니다.
            // (이로 인해 로딩 중간에 텍스트가 툭 바뀌는 번쩍임 현상이 사라집니다)
            Debug.Log($"id : {key}");
            Debug.Log($"op 결과 : {op.Result}");
            tipText.text = op.Result;
        }
        else
        {
            Debug.LogError($"로컬라이징 팁 텍스트 로드 실패! Key: {key}");
            tipText.text = "기본 로딩 팁 문구..."; // 에러 발생 시 띄울 대체 문구
        }
    }
}
