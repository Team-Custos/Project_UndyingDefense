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
    public GameObject localSituationPanel = null;
    public Button localSituationPanelCloseBtn = null;
    public Image battleFieldImage = null;
    public Text battleFieldScriptTxt = null;
    public Button commandSkillResetBtn = null;
    public Image[] commandSkillDeckEquipImage = null;
    public Button showCommandSkillListBtn = null;
    public Button battleStartBtn = null;

    [Header("==== CommandSkill UI ====")]
    public GameObject commandSkillPanel = null;
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

    private const int maxSkillDeckSize = 3;


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

        if(battleStartBtn != null)
        {
            if (battleStartBtn != null)
            {
                battleStartBtn.onClick.AddListener(() => SceneManager.LoadSceneAsync("Stage1_Mege_LoPol 1"));
            }
        }

        //UI 판넬 On / Off
        if (stageStartBtn != null)
        {
            stageStartBtn.onClick.AddListener(() =>
            {
                localSituationPanel.SetActive(true);
            });
        }

        if (localSituationPanelCloseBtn != null)
        {
            localSituationPanelCloseBtn.onClick.AddListener(() =>
            {
                localSituationPanel.SetActive(false);
            });
        }

        if(commandSkillPanelCloseBtn != null)
        {
            commandSkillPanelCloseBtn.onClick.AddListener(() =>
            {
                commandSkillPanel.SetActive(false);
                localSituationPanel.SetActive(true);
            });
        }

        if(commandSkillResetBtn != null)
        {
            commandSkillResetBtn.onClick.AddListener(() =>
            {
                localSituationPanel.SetActive(false);
                commandSkillPanel.SetActive(true);

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
                commandSkillEquipBtn[buttonIndex].onClick.AddListener(() => AddCommandSkill(buttonIndex));
            }
        }

        // 스킬 제거 버튼에 대한 리스너 추가
        for (int i = 0; i < commandSkillClearBtn.Length; i++)
        {
            if (commandSkillClearBtn[i] != null)
            {
                int buttonIndex = i;
                commandSkillClearBtn[buttonIndex].onClick.AddListener(() => RemoveCommandSkill(buttonIndex));
            }
        }

        if(showCommandSkillListBtn != null)
        {
            showCommandSkillListBtn.onClick.AddListener(() =>
            {
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

                SaveCommandSkillList(skillData); 
            });
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
        int skillIndex = CommandSkillDeckIndex[deckIndex];
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
            Debug.LogWarning("잘못된 버튼 인덱스");
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
        List<string> skillIDs = new List<string>();

        foreach (var entry in skillData)
        {
            // 스킬 ID와 이름을 각각 PlayerPrefs에 저장
            PlayerPrefs.SetString(entry.Key, entry.Value);
            skillIDs.Add(entry.Key);
        }

        // 모든 스킬 ID 리스트를 저장 (나중에 불러올 때 사용)
        PlayerPrefs.SetString("SkillIDList", string.Join(",", skillIDs));

        PlayerPrefs.Save(); // 저장

        Debug.Log("커맨더 스킬 저장");
    }


}
