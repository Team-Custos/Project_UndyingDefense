using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class LobbyUIManager : MonoBehaviour
{
    public Button stageStartBtn = null;
    public Button stageStart2Btn = null;

    [Header("===== Local Situation UI ====")]
    public RectTransform localSituationPanel = null;
    public Button commandSkillPanelOpenBtn;
    public Button localSituationPanelCloseBtn = null;
    public Button battleStartBtn = null;
    public Image[] localCommandSkillImage;

    public GameObject commandSkillPanel;
    public Button commandSkillPanelCloseBtn;


    public GameObject HelpUsePanel;
    public GameObject HelpArrPanel;
    public Button HelpCloseBtn;
    public Button HelpUseBtn;
    public Button HelpArrBtn;

    public ParticleSystem buttonParticleEffect;
    public Button particleBtn;


    [Header("==================")]


    // 도움말 ui
    public Button lobbyHelpBtn;
    public Button lobbyHelpCloseBtn;
    public GameObject lobbyHelpPanel;

    public GameObject[] operatePanel;

    public Button operateRightBtn;
    public Button operateLeftBtn;

    private int currentIndex = 0;     // 현재 활성화된 패널 인덱스
    public float slideDuration = 0.5f; // 슬라이드 애니메이션 지속 시간

    public Button lobbySettingBtn;
    public Button settingCloseBtn;
    public Button endGameBtn;
    public Button creditBtn;
    public GameObject lobbySettingPanel;

    public GameObject creditPanel;
    public Button creditCloseBtn;

    public Sprite currentTabImage;
    public Sprite noneTabImage;

    public Button helpManulBtn;
    public Button helpAttributeBtn;
    public GameObject helpManulPanel;
    public GameObject helpAttributePanel;

    public GameObject helpManulPanel1;
    public GameObject helpAttributePanel1;


    private Image helpManulBtnImage;
    private Image helpAttributeBtnImage;

    private bool isManulTrue;

    public float animationDuration = 0.5f; // 연출 지속 시간
    public float delayBetweenAnimations = 0.2f; // 각 ui delay 시간

    float elapsedTime = 0f;
    public float minLoadingTime = 3f; // 씬 로딩 시간 3초로 고정


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;

        if(battleStartBtn != null)
        {
            battleStartBtn.onClick.AddListener(() =>
            {
                if(UserDataModel.instance.skillDatas.Count < 3)
                {
                    Debug.Log("스킬을 3개 선택해주세요.");
                    return;
                }
                else
                {
                    LoadingSceneManager.LoadScene("Stage1_MergeScene  25.0224");
                }
            });
        }

        // 설정 창 on/off
        if (lobbySettingBtn != null)
        {

            lobbySettingBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                lobbySettingPanel.SetActive(true);
            });
        }

        if(settingCloseBtn != null)
        {

            settingCloseBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                lobbySettingPanel.SetActive(false);
            });
        }

        if(endGameBtn != null)
        {

            endGameBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }


                EndGame();
            });
        }


        if (lobbyHelpBtn != null)
        {
            lobbyHelpBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                lobbyHelpPanel.SetActive(true);
            });
        }

        if (HelpCloseBtn != null)
        {
            
            HelpCloseBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                lobbyHelpPanel.SetActive(false);
            });
        }

        helpManulBtnImage = helpManulBtn.GetComponent<Image>();
        helpAttributeBtnImage = helpAttributeBtn.GetComponent<Image>();


        isManulTrue = true;

        if (helpManulBtn != null)
        {
            helpManulBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                isManulTrue = true;
                helpManulBtnImage.sprite = currentTabImage;
                helpAttributeBtnImage.sprite = noneTabImage;
                helpManulPanel.SetActive(true);
                helpAttributePanel.SetActive(false);
            });
        }
        
        
        if (helpAttributeBtn != null)
        {

            helpAttributeBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                isManulTrue = false;
                helpManulBtnImage.sprite = noneTabImage;
                helpAttributeBtnImage.sprite = currentTabImage;
                helpManulPanel.SetActive(false);
                helpAttributePanel.SetActive(true);
            });
        }


        commandSkillPanelOpenBtn.onClick.AddListener(() =>
        {
            commandSkillPanel.SetActive(true);
        });

        // 지역상황창 열기
        if (stageStartBtn != null)
        {
            stageStartBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_battleStart);
                }

                ShowUI(localSituationPanel);
            });

        }

        if(commandSkillPanelCloseBtn != null)
        {
            commandSkillPanelCloseBtn.onClick.AddListener(() =>
            {
                commandSkillPanel.SetActive(false);

                localCommandSkillImage[0].sprite = UserDataModel.instance.skillDatas[0].commandSkillImage;
                localCommandSkillImage[1].sprite = UserDataModel.instance.skillDatas[1].commandSkillImage;
                localCommandSkillImage[2].sprite = UserDataModel.instance.skillDatas[2].commandSkillImage;
            });
        }

        if (particleBtn != null)
        {
            particleBtn.onClick.AddListener(() =>
            {
                //GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                PlayParticleEffect();
            });

        }

        if (localSituationPanelCloseBtn != null)
        {
            localSituationPanelCloseBtn.onClick.AddListener(() =>
            {
                //GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                HideUI(localSituationPanel);
            });
        }


        //for (int i = 0; i < optionBtn.Length; i++)
        //{
        //    int index = i;  // 내부에서 사용하기 위해 로컬 변수로 인덱스를 저장
        //    optionBtn[i].onClick.AddListener(() => TogglePanel(index));
        //}

        //// 시작 시 모든 패널을 비활성화

        //HelpCloseBtn.onClick.AddListener(CloseHelpPanel);
        //HelpUseBtn.onClick.AddListener(OpenHelpUsePanel);
        //HelpArrBtn.onClick.AddListener(OpenHelpArrPanel);



        // 초기 패널 설정
        for (int i = 0; i < operatePanel.Length; i++)
        {
            if (i == currentIndex)
                operatePanel[i].SetActive(true);
            else
                operatePanel[i].SetActive(false);
        }

        // 버튼 클릭 이벤트 연결
        operateRightBtn.onClick.AddListener(() =>
        {
            if (GlobalSoundManager.instance != null)
            {
                GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
            }

            SlideRight();
        });

        operateLeftBtn.onClick.AddListener(() =>
        {
            if (GlobalSoundManager.instance != null)
            {
                GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
            }

            SlideLeft();
        });

        UpdateButtonInteractivity();

        if(creditBtn != null)
        {
            creditBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                creditPanel.SetActive(true);
            });
        }

        if (creditCloseBtn != null)
        {
            creditCloseBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                creditPanel.SetActive(false);
            });
        }
    }


    private void Update()
    {
        if(isManulTrue)
        {
            helpManulBtnImage.sprite = currentTabImage;
            helpAttributeBtnImage.sprite = noneTabImage;

        }
        else
        {
            helpManulBtnImage.sprite = noneTabImage;
            helpAttributeBtnImage.sprite = currentTabImage;
        }
    }

    void SlideRight()
    {
        if (currentIndex < operatePanel.Length - 1)
        {
            StartCoroutine(SlidePanels(currentIndex, currentIndex + 1, Vector2.right));
            currentIndex++;
            UpdateButtonInteractivity();
        }
    }

    void SlideLeft()
    {
        if (currentIndex > 0)
        {
            StartCoroutine(SlidePanels(currentIndex, currentIndex - 1, Vector2.left));
            currentIndex--;
            UpdateButtonInteractivity();
        }
    }

    void UpdateButtonInteractivity()
    {
        operateLeftBtn.interactable = currentIndex > 0;
        operateRightBtn.interactable = currentIndex < operatePanel.Length - 1;
    }

    IEnumerator SlidePanels(int fromIndex, int toIndex, Vector2 direction)
    {
        GameObject fromPanel = operatePanel[fromIndex];
        GameObject toPanel = operatePanel[toIndex];

        // 현재 y 위치 저장
        float yPositionFrom = fromPanel.transform.localPosition.y;
        float yPositionTo = toPanel.transform.localPosition.y;

        // 활성화할 패널을 슬라이드 방향으로 화면 밖에 위치시킴
        Vector2 offScreenPosition = new Vector2(direction.x * Screen.width, 0);
        toPanel.transform.localPosition = new Vector2(direction.x * Screen.width, yPositionTo);
        toPanel.SetActive(true);

        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            fromPanel.transform.localPosition = Vector2.Lerp(
                new Vector2(0, yPositionFrom),
                new Vector2(-offScreenPosition.x, yPositionFrom),
                t
            );
            toPanel.transform.localPosition = Vector2.Lerp(
                new Vector2(offScreenPosition.x, yPositionTo),
                new Vector2(0, yPositionTo),
                t
            );
            elapsed += Time.deltaTime;
            yield return null;
        }

        fromPanel.transform.localPosition = new Vector2(-offScreenPosition.x, yPositionFrom);
        toPanel.transform.localPosition = new Vector2(0, yPositionTo);
        fromPanel.SetActive(false);
    }

    // 하나의 패널만 켜고 나머지는 끄는 함수
    //private void TogglePanel(int index)
    //{
    //    // 만약 선택된 패널이 이미 켜져 있으면 끄기
    //    if (optionPanel[index].activeSelf)
    //    {
    //        optionPanel[index].SetActive(false);
    //    }
    //    else
    //    {
    //        // 그렇지 않으면 모든 패널을 끄고 선택한 패널만 켜기
    //        HideAllPanels();
    //        optionPanel[index].SetActive(true);
    //    }
    //}

    //void CloseHelpPanel()
    //{
    //    optionPanel[0].SetActive(false);
    //}

    // HelpUsePanel을 열고 HelpArrPanel을 닫는 함수
    void OpenHelpUsePanel()
    {
        HelpUsePanel.SetActive(true);
        HelpArrPanel.SetActive(false);
    }

    // HelpArrPanel을 열고 HelpUsePanel을 닫는 함수
    void OpenHelpArrPanel()
    {
        HelpArrPanel.SetActive(true);
        HelpUsePanel.SetActive(false);
    }

    // 모든 패널을 비활성화하는 함수
    //private void HideAllPanels()
    //{
    //    foreach (var panel in optionPanel)
    //    {
    //        panel.SetActive(false);
    //    }
    //}


    // 패널의 크기를 조절하는 코루틴
    public IEnumerator AnimateUI(RectTransform ui, bool isActive, float duration = 0.2f)
    {
        // 패널의 중심을 화면 중앙으로 설정 (피벗과 앵커를 중앙으로)
        ui.pivot = new Vector2(0.5f, 0.5f);
        ui.anchorMin = new Vector2(0.5f, 0.5f);
        ui.anchorMax = new Vector2(0.5f, 0.5f);

        // 시작 크기와 끝 크기 설정
        Vector3 startScale = ui.localScale;
        Vector3 endScale;

        // isActive에 따라 크기 설정
        if (isActive)
        {
            endScale = Vector3.one;  // (1, 1, 1) 크기로 활성화
        }
        else
        {
            endScale = Vector3.zero; // (0, 0, 0) 크기로 비활성화
        }

        // 애니메이션 타이머
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            // 크기를 서서히 변화시킴
            ui.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            yield return null;
        }

        // 최종 크기 설정
        ui.localScale = endScale;
        // 끝났을 때 패널의 활성/비활성 설정
        ui.gameObject.SetActive(isActive);
    }

    // 패널을 활성화하는 함수
    public void ShowUI(RectTransform ui)
    {
        ui.gameObject.SetActive(true); // 먼저 활성화
        StartCoroutine(AnimateUI(ui, true, 0.2f)); // 커지면서 나타나는 연출
    }

    // 패널을 비활성화하는 함수
    public void HideUI(RectTransform ui)
    {
        StartCoroutine(AnimateUI(ui, false, 0.2f)); // 작아지면서 사라지는 연출
    }

    private void PlayParticleEffect()
    {
        if (buttonParticleEffect != null)
        {
            buttonParticleEffect.Play();  // 파티클 효과 실행
        }
    }


    // 페이드 인/아웃을 위한 코루틴 함수
    public IEnumerator FadeUI(GameObject uiElement, bool isFadeIn, float duration = 0.2f)
    {
        CanvasGroup canvasGroup = uiElement.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = uiElement.AddComponent<CanvasGroup>();
        }

        float startAlpha;
        float endAlpha;

        if (isFadeIn)
        {
            startAlpha = 0;
            endAlpha = 1;
        }
        else
        {
            startAlpha = 1;
            endAlpha = 0;
        }

        float time = 0f;

        // 애니메이션 진행 시간에 따라 alpha 값을 변화
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
            yield return null;
        }

        canvasGroup.alpha = endAlpha;

        if (!isFadeIn)
        {
            canvasGroup.blocksRaycasts = true;
            uiElement.SetActive(false);
        }
        else
        {
            canvasGroup.blocksRaycasts = false;
            uiElement.SetActive(true);
        }
    }


    void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

}