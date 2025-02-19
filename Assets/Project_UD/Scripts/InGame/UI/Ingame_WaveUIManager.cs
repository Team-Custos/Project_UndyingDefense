using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ingame_WaveUIManager : MonoBehaviour
{
    public static Ingame_WaveUIManager instance;

    [SerializeField] private  WaveCanvasController waveCanvasController;


    public float fadeDuration = 0.3f;

    private void Awake()
    {
        instance = this;

        // Resources 폴더에서 프리팹 로드
        GameObject canvasWavePrefab = Resources.Load<GameObject>("Canvas_wave");
        if (canvasWavePrefab != null)
        {
            // 프리팹을 인스턴스화하고 현재 오브젝트의 자식으로 추가
            GameObject canvasInstance = Instantiate(canvasWavePrefab, transform);
            Ingame_WaveUIManager instance = canvasInstance.GetComponent<Ingame_WaveUIManager>();


        }
        else
        {
            Debug.Log("프리팹을 찾을 수 없습니다: Canvas_wave");
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        // 웨이브 카운트 스킵
        if (waveCanvasController.waveCountSkipBtn != null)
        {
            waveCanvasController.waveCountSkipBtn.onClick.AddListener(() =>
            {
                SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_click);
                waveCanvasController.waveCount = 0; // 버튼 클릭 시 카운트를 0으로 설정
                waveCanvasController.waveCountText.text = "적군 침공까지 0초"; // 즉시 0으로 카운트 표시

                waveCanvasController.waveCountText.gameObject.SetActive(false);
                //waveCountText.gameObject.SetActive(false);
            });

        }
    }

    //public void StartNextWaveCountdown(float startTimeFromData)
    //{
    //    // WaveData에서 가져온 waveStartTime을 런타임용 waveCount에 복사
    //    waveCanvasController.waveCount = startTimeFromData;
    //    isCountDownIng = true;
    //    waveCountTextPanel.SetActive(true);
    //}

    // Update is called once per frame
    void Update()
    {
        //// 웨이브 카운트 다운 텍스트
        //if (waveCountText != null && isCountDownIng) // 웨이브가 진행중이 아닐 때
        //{
        //    waveCount -= Time.deltaTime;
        //    if (waveCount >= 0 && isCountDownIng)
        //    {
        //        waveCountTextPanel.SetActive(true);
        //        //waveCountText.gameObject.SetActive(true);
        //        waveCountText.text = "적군 침공까지 " + Mathf.Ceil(waveCount).ToString() + "초";

        //    }
        //    else if (waveCount < 0)
        //    {
        //        waveStartText.text = EnemySpawner.inst.currentWave.ToString() + "차 침공 시작";
        //        isCountDownIng = false;
        //        EnemySpawner.inst.isWaveing = true;
        //        waveCountTextPanel.SetActive(false);
               

        //        //StartCoroutine(EnemySpawner.inst.RunWave());
        //        waveCount = 20;
        //    }
        //}

        // 다음 웨이브로 강제 이동
        //if (nextWavBtn != null)
        //{
        //    nextWavBtn.onClick.AddListener(() =>
        //    {
        //        Time.timeScale = 0.0f;
        //        waveResultPanel.gameObject.SetActive(true);
        //        //EnemySpawner.inst.NextWave();
        //    });
        //}

        // 웨이브 실패시 이미지 설정
        

        // 웨이브 시작 표시 
        if (waveCanvasController.waveStartText.gameObject.activeSelf == true)
        {
            float coolTime = 3.0f;

            coolTime -= Time.deltaTime;

            if (coolTime < 0)
            {
                waveCanvasController.waveStartText.gameObject.SetActive(false);
            }
        }
    }


    public void ShowUI(GameObject uiElement, float duration)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(true);  // UI 요소 활성화
            StartCoroutine(HideUIAfterDelay(uiElement, duration));  // 일정 시간 후 비활성화
        }
    }

    // 일정 시간이 지나면 UI 요소를 비활성화하는 코루틴
    IEnumerator HideUIAfterDelay(GameObject uiElement, float delay)
    {
        yield return new WaitForSeconds(delay);  // 지정된 시간만큼 대기
        uiElement.SetActive(false);  // UI 요소 비활성화
    }



    // UI를 켜는 함수 (Fade In)
    public void FadeInUI(CanvasGroup canvasGroup)
    {
        StartCoroutine(FadeUI(canvasGroup, 0, 1)); // alpha를 0에서 1로 전환 (Fade In)
    }

    // UI를 끄는 함수 (Fade Out)
    public void FadeOutUI(CanvasGroup canvasGroup)
    {
        StartCoroutine(FadeUI(canvasGroup, 1, 0)); // alpha를 1에서 0으로 전환 (Fade Out)
    }

    // 페이드 애니메이션 코루틴
    private IEnumerator FadeUI(CanvasGroup canvasGroup, float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        // 초기 alpha 설정
        canvasGroup.alpha = startAlpha;

        // alpha 값을 서서히 변화시킴
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            yield return null;
        }

        // 최종 alpha 값 설정
        canvasGroup.alpha = endAlpha;

        // 페이드 아웃 시 상호작용 차단 (선택 사항)
        if (endAlpha == 0)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

}
