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
    public Button localSituationPanelCloseBtn = null;
    public Image battleFieldImage = null;
    public Text battleFieldScriptTxt = null;
    public Button commandSkillResetBtn = null;
    public Image[] commandSkillDeckEquipImage = null;
    public Button showCommandSkillListBtn = null;
    public Button battleStartBtn = null;

    [Header("==== CommandSkill UI ====")]
    public RectTransform commandSkillPanel = null;
    public Button commandSkillPanelCloseBtn = null;
    public Image[] commandSkillDeckListImage = null;
    public Image[] commandSkillList = null;
    public Button[] commandSkillEquipBtn = null;
    public Image commandSkillInfoImage = null;
    public Text commandSkillInfoScriptTxt = null;
    public Button[] commandSkillClearBtn = null;
    public Button commandkSkillSaveBtn = null;

    private Sprite emptyDeckImageSprite = null;
    private Color emptyDeckImageColor;

    private bool[] isCommandSkillDeckEmpty;
    private bool[] isCommandSkillSelect;

    private int[] CommandSkillDeckIndex;

    private string[] skillIDs = {
    "lead_order101", "lead_order102", "lead_order103",
    "lead_support101", "lead_support102", "lead_support103",
    "lead_morale101", "lead_morale102", "lead_morale103"
    };


    public GameObject HelpUsePanel;
    public GameObject HelpArrPanel;
    public Button HelpCloseBtn;
    public Button HelpUseBtn;
    public Button HelpArrBtn;


    private const int maxSkillDeckSize = 3;

    public ParticleSystem buttonParticleEffect;
    public Button particleBtn;

    private CommandSkillManager commandSkillManager;
    private List<CommandSkillManager.SkillData> commandSkillDeckList = new List<CommandSkillManager.SkillData>();


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


    public GameObject loadingPanel;
    public Image progressImage;
    public Text progressText;

    public float animationDuration = 0.5f; // 연출 지속 시간
    public float delayBetweenAnimations = 0.2f; // 각 ui delay 시간

    float elapsedTime = 0f;
    public float minLoadingTime = 3f; // 씬 로딩 시간 3초로 고정


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;

        commandSkillManager = CommandSkillManager.Instance;

        emptyDeckImageSprite = commandSkillDeckListImage[0].sprite;
        emptyDeckImageColor = commandSkillDeckListImage[0].color;

        isCommandSkillDeckEmpty = new bool[commandSkillDeckListImage.Length];
        CommandSkillDeckIndex = new int[commandSkillDeckListImage.Length];

        // 설정 창 on/off
        if(lobbySettingBtn != null)
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



        //if (battleStartBtn != null)
        //{
        //    if (battleStartBtn != null)
        //    {

        //        battleStartBtn.onClick.AddListener(() =>
        //        {
        //            if(GlobalSoundManager.instance != null)
        //            {
        //                GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_battleStart);
        //            }

        //            SceneManager.LoadSceneAsync(2);

        //            //if (commandSkillDeckList.Count < maxSkillDeckSize)
        //            //{
        //            //    //StartCoroutine(FadeUI(saveErrorMessgePanel, true));
        //            //    //saveErrorMessgePanel.SetActive(true);
        //            //}
        //            //else
        //            //{
        //            //    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_battleStart);
        //            //    SceneManager.LoadSceneAsync("Stage1_Mege_LoPol 1");
        //            //}
        //        });
        //    }
        //}

        // UI 판넬 On / Off
        if (stageStartBtn != null)
        {
            stageStartBtn.onClick.AddListener(() =>
            {
                //SceneManager.LoadSceneAsync(2);

                LoadScene(2);

                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_battleStart);
                }
                //ShowUI(localSituationPanel);
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

        if (commandSkillPanelCloseBtn != null)
        {
            commandSkillPanelCloseBtn.onClick.AddListener(() =>
            {
                //GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                HideUI(commandSkillPanel);
                //ShowUI(localSituationPanel);
            });
        }

        if (commandSkillResetBtn != null)
        {
            commandSkillResetBtn.onClick.AddListener(() =>
            {
                //GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                HideUI(localSituationPanel);
                ShowUI(commandSkillPanel);

            });
        }
        // UI 판넬 On/Off


        // 초기화: 덱 비우기
        for (int i = 0; i < isCommandSkillDeckEmpty.Length; i++)
        {
            isCommandSkillDeckEmpty[i] = true;
            CommandSkillDeckIndex[i] = -1;
        }

        isCommandSkillSelect = new bool[commandSkillList.Length];

        // 스킬 장착 버튼에 대한 리스너 추가
        for (int i = 0; i < commandSkillEquipBtn.Length; i++)
        {
            if (commandSkillEquipBtn[i] != null)
            {
                int buttonIndex = i;

                commandSkillEquipBtn[buttonIndex].onClick.AddListener(() =>
                {
                    if (GlobalSoundManager.instance != null)
                    {
                        GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_commanderSkillEquip);
                    }
                    AddCommandSkill(buttonIndex);
                });
            }
        }

        // 스킬 제거 버튼에 대한 리스너 추가
        for (int i = 0; i < commandSkillClearBtn.Length; i++)
        {
            if (commandSkillClearBtn[i] != null)
            {
                int buttonIndex = i;

                commandSkillClearBtn[buttonIndex].onClick.AddListener(() =>
                {
                    if (GlobalSoundManager.instance != null)
                    {
                        GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_commanderSkillUnequip);
                    }
                    RemoveCommandSkill(buttonIndex);
                });
            }
        }

        if (showCommandSkillListBtn != null)
        {
            showCommandSkillListBtn.onClick.AddListener(() =>
            {
                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }
                ShowCommandSkillList();
            });
        }

        if (commandkSkillSaveBtn != null)
        {
            commandkSkillSaveBtn.onClick.AddListener(() =>
            {
                Dictionary<string, string> skillData = new Dictionary<string, string>();

                foreach (var skill in commandSkillDeckList)
                {
                    skillData.Add(skill.SkillID, skill.SkillName);
                }

                if (GlobalSoundManager.instance != null)
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                }

                //SaveCommandSkillList(skillData);
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

    // 스킬 장착 기능 및 UI 업데이트
    private void AddCommandSkill(int buttonIndex)
    {
        if (commandSkillDeckList.Count >= maxSkillDeckSize)
        {
            Debug.Log("스킬 개수 초과");
            return;
        }

        if (buttonIndex >= 0 && buttonIndex < skillIDs.Length)
        {
            string skillID = skillIDs[buttonIndex];

            if (commandSkillDeckList.Exists(skill => skill.SkillID == skillID))
            {
                Debug.Log("스킬 중복");
                return;
            }

            if (commandSkillManager.skillDataDictionary.TryGetValue(skillID, out CommandSkillManager.SkillData skill))
            {
                commandSkillDeckList.Add(skill);
                Debug.Log($"Command Skill {skill.SkillName} 추가됨");

                for (int i = 0; i < commandSkillDeckListImage.Length; i++)
                {
                    if (isCommandSkillDeckEmpty[i])
                    {
                        commandSkillDeckListImage[i].sprite = commandSkillList[buttonIndex].sprite;
                        commandSkillDeckListImage[i].color = commandSkillList[buttonIndex].color;
                        isCommandSkillDeckEmpty[i] = false;
                        isCommandSkillSelect[buttonIndex] = true;
                        CommandSkillDeckIndex[i] = buttonIndex;
                        commandSkillDeckEquipImage[i].sprite = commandSkillDeckListImage[i].sprite;
                        commandSkillDeckEquipImage[i].color = commandSkillDeckListImage[i].color;

                        break;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"{skillID} 스킬 없음");
            }
        }
        else
        {
            Debug.LogWarning("잘못된 인덱스");
        }
    }


    // 스킬 제거 기능 및 UI 업데이트
    private void RemoveCommandSkill(int deckIndex)
    {
        // 인덱스 범위 검증
        if (deckIndex < 0 || deckIndex >= commandSkillDeckListImage.Length)
        {
            Debug.LogWarning("덱 인덱스가 잘못되었습니다.");
            return;
        }

        int skillIndex = CommandSkillDeckIndex[deckIndex];

        // 해당 덱이 비어 있는지 확인 (skillIndex == -1이 아니고, 덱 인덱스도 올바른지 확인)
        if (skillIndex != -1 && deckIndex >= 0 && deckIndex < commandSkillDeckList.Count)
        {
            Debug.Log($"{commandSkillDeckList[deckIndex].SkillName} 제거");
            commandSkillDeckList.RemoveAt(deckIndex);

            // UI 업데이트
            commandSkillDeckListImage[deckIndex].sprite = emptyDeckImageSprite;
            commandSkillDeckListImage[deckIndex].color = emptyDeckImageColor;
            isCommandSkillDeckEmpty[deckIndex] = true;
            isCommandSkillSelect[skillIndex] = false;
            CommandSkillDeckIndex[deckIndex] = -1;
            commandSkillDeckEquipImage[deckIndex].sprite = emptyDeckImageSprite;
            commandSkillDeckEquipImage[deckIndex].color = emptyDeckImageColor;
        }
        else
        {
            Debug.LogWarning("잘못된 버튼 인덱스 또는 덱이 비어 있습니다.");
        }
    }


    private void ShowCommandSkillList()
    {
        if (commandSkillDeckList.Count > 0)
        {
            Debug.Log("현재 장착된 Command Skills:");
            for (int i = 0; i < commandSkillDeckList.Count; i++)
            {
                var skill = commandSkillDeckList[i];
                Debug.Log($"Skill {i + 1}: {skill.SkillName}");
            }
        }
        else
        {
            Debug.Log("Command Skill 없음");
        }
    }

    // 커맨더 스킬 저장
    public void SaveCommandSkillList(Dictionary<string, string> skillData)
    {
        UserDataModel.instance.skillIDs.Clear();
        List<string> skillIDs = new List<string>();

        foreach (var skill in skillData)
        {
            skillIDs.Add(skill.Key);
        }


        //foreach (var entry in skillData)
        //{
        //    // 스킬 ID와 이름을 각각 PlayerPrefs에 저장
        //    PlayerPrefs.SetString(entry.Key, entry.Value);
        //    skillIDs.Add(entry.Key);
        //    UserDataModel.instance.skillIDs.Add(entry.Key);
        //}

        //// 모든 스킬 ID 리스트를 저장 (나중에 불러올 때 사용)
        //PlayerPrefs.SetString("SkillIDList", string.Join(",", skillIDs));

        //PlayerPrefs.Save(); // 저장

        Debug.Log("커맨더 스킬 저장");
    }



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

    public void LoadScene(int sceneNumber)
    {
        StartCoroutine(LobbyLoadSceneAsyncCorutine(sceneNumber));
    }

    private IEnumerator LobbyLoadSceneAsyncCorutine(int sceneIdx)
    {

        // 로딩 패널 활성화
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            Debug.Log("로딩 패널 활성화됨");
        }

        // 씬 로딩 시작
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIdx);
        operation.allowSceneActivation = false; // 씬 자동 전환 방지
        Debug.Log("씬 로딩 시작");

        while (!operation.isDone)
        {
            // 로딩 진행도 (0.0 ~ 1.0)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressImage != null)
                progressImage.fillAmount = progress;

            if (progressText != null)
                progressText.text = (progress * 100f).ToString("F0") + "%";




            // 로딩이 90% 이상일 때 (Unity는 실제 로딩 완료 시점이 0.9임)
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // 씬 로딩 완료 후 로딩 패널 비활성화
        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
            Debug.Log("로딩 패널 비활성화됨");
        }

        Debug.Log("씬 로드 완료");
    }


}