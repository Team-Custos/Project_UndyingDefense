using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


public class LobbyUIManager : MonoBehaviour
{
    public Button stageStartBtn = null;

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

    public GameObject[] optionPanel;
    public Button[] optionBtn;

    private const int maxSkillDeckSize = 3;

    public ParticleSystem buttonParticleEffect;
    public Button particleBtn;

    private CommandSkillManager commandSkillManager;
    private List<CommandSkillManager.SkillData> commandSkillDeckList = new List<CommandSkillManager.SkillData>();



    // Start is called before the first frame update
    void Start()
    {
        commandSkillManager = CommandSkillManager.Instance;

        emptyDeckImageSprite = commandSkillDeckListImage[0].sprite;
        emptyDeckImageColor = commandSkillDeckListImage[0].color;

        isCommandSkillDeckEmpty = new bool[commandSkillDeckListImage.Length];
        CommandSkillDeckIndex = new int[commandSkillDeckListImage.Length];

        if (battleStartBtn != null)
        {
            if (battleStartBtn != null)
            {

                battleStartBtn.onClick.AddListener(() =>
                {
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_battleStart);
                    SceneManager.LoadSceneAsync(2);

                    //if (commandSkillDeckList.Count < maxSkillDeckSize)
                    //{
                    //    //StartCoroutine(FadeUI(saveErrorMessgePanel, true));
                    //    //saveErrorMessgePanel.SetActive(true);
                    //}
                    //else
                    //{
                    //    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_battleStart);
                    //    SceneManager.LoadSceneAsync("Stage1_Mege_LoPol 1");
                    //}
                });
            }
        }

        // UI 판넬 On / Off
        if (stageStartBtn != null)
        {
            stageStartBtn.onClick.AddListener(() =>
            {
                //GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                ShowUI(localSituationPanel);
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
                ShowUI(localSituationPanel);
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
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_commanderSkillEquip);
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
                    GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_commanderSkillUnequip);
                    RemoveCommandSkill(buttonIndex);
                });
            }
        }

        if (showCommandSkillListBtn != null)
        {
            showCommandSkillListBtn.onClick.AddListener(() =>
            {
                GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
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
                GlobalSoundManager.instance.PlayLobbySFX(GlobalSoundManager.lobbySfx.sfx_click);
                SaveCommandSkillList(skillData);
            });
        }


        for (int i = 0; i < optionBtn.Length; i++)
        {
            int index = i;  // 내부에서 사용하기 위해 로컬 변수로 인덱스를 저장
            optionBtn[i].onClick.AddListener(() => TogglePanel(index));
        }

        // 시작 시 모든 패널을 비활성화
    }

    // 하나의 패널만 켜고 나머지는 끄는 함수
    private void TogglePanel(int index)
    {
        // 만약 선택된 패널이 이미 켜져 있으면 끄기
        if (optionPanel[index].activeSelf)
        {
            optionPanel[index].SetActive(false);
        }
        else
        {
            // 그렇지 않으면 모든 패널을 끄고 선택한 패널만 켜기
            HideAllPanels();
            optionPanel[index].SetActive(true);
        }
    }

    // 모든 패널을 비활성화하는 함수
    private void HideAllPanels()
    {
        foreach (var panel in optionPanel)
        {
            panel.SetActive(false);
        }
    }

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
    public IEnumerator AnimateUI(RectTransform ui, bool isActive, float duration = 0.3f)
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
        StartCoroutine(AnimateUI(ui, true, 0.3f)); // 커지면서 나타나는 연출
    }

    // 패널을 비활성화하는 함수
    public void HideUI(RectTransform ui)
    {
        StartCoroutine(AnimateUI(ui, false, 0.3f)); // 작아지면서 사라지는 연출
    }

    private void PlayParticleEffect()
    {
        if (buttonParticleEffect != null)
        {
            buttonParticleEffect.Play();  // 파티클 효과 실행
        }
    }


    // 페이드 인/아웃을 위한 코루틴 함수
    public IEnumerator FadeUI(GameObject uiElement, bool isFadeIn, float duration = 0.5f)
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

}