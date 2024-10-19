using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ingame_WaveUIManager : MonoBehaviour
{
    public static Ingame_WaveUIManager instance;

    public float waveCount = 20;
    public Text waveCountText;
    public Text waveStartText;
    public GameObject waveStartPanel;
    public GameObject waveResultPanel;
    public Image waveResultImage;
    public Text waveResultText;
    public bool isCurrentWaveSucceed;
    public Button nextWavBtn;
    public Button waveCountSkipBtn;
    public bool isCountDownIng = false;
    public Button waveRestartBtn = null;
    public Button lobbybtn = null;
    public GameObject waveStepSuccessPanel;
    public Sprite waveWinImage;
    public Sprite waveLoseImage;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 웨이브 카운트 스킵
        if (waveCountSkipBtn != null)
        {
            waveCountSkipBtn.onClick.AddListener(() =>
            {
                waveCount = 0; // 버튼 클릭 시 카운트를 0으로 설정
                waveCountText.text = "적군 침공까지 0초"; // 즉시 0으로 카운트 표시
                //StartCoroutine(EnemySpawner.inst.StartWaveWithDelay(1f)); // 바로 1초 후 웨이브 시작
                waveCountText.gameObject.SetActive(false);
            });

        }
    }

    // Update is called once per frame
    void Update()
    {
        // 웨이브 카운트 다운 텍스트
        if (waveCountText != null && isCountDownIng) // 웨이브가 진행중이 아닐 때
        {
            waveCount -= Time.deltaTime;
            if (waveCount >= 0 && isCountDownIng)
            {
                waveCountText.gameObject.SetActive(true);
                waveCountText.text = "적군 침공까지 " + Mathf.Ceil(waveCount).ToString() + "초";

            }
            else if (waveCount < 0)
            {
                waveStartText.text = EnemySpawner.inst.currentWave.ToString() + "차 침공 시작";
                isCountDownIng = false;
                EnemySpawner.inst.isWaveing = true;
                waveCountText.gameObject.SetActive(false);
                StartCoroutine(EnemySpawner.inst.StartWaveWithDelay(1f)); // 1초의 지연 후 웨이브 시작
                waveCount = 20;
            }
        }

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
        if (BaseStatus.instance.BaseHPCur <= 0)
        {
            waveResultImage.sprite = waveLoseImage;
        }

        // 웨이브 시작 표시 
        if (waveStartText.gameObject.activeSelf == true)
        {
            float coolTime = 3.0f;

            coolTime -= Time.deltaTime;

            if (coolTime < 0)
            {
                waveStartText.gameObject.SetActive(false);
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
}
