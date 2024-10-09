using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LobbyUIManager : MonoBehaviour
{
    [Header("===== Locla Situation UI ====")]
    public GameObject localSituationPanel = null;
    public Button localSituationPanelCloseBtn = null;
    public Image battleFieldImage = null;
    public Text battleFieldScriptTxt = null;
    public GameObject commandSkillDeckPaenl = null;
    public Image[] commandSkillDeckList = null;
    public Button commandSkillResetBtn = null;
    public Button stageStartBtn = null;

    [Header("==== CommandSkill UI ====")]
    public GameObject commandSkillPanel = null;
    public Button commandSkillPanelCloseBtn = null;
    public Image[] commandSkillList = null;
    public Button[] commandSkillEquipBtn = null;
    public Image commandSkillInfoImage = null;
    public Text commandSkillInfoScriptTxt = null; 

    public Button[] commandSkillClearBtn = null;

    private Sprite emptyDeckImageSprite = null;
    private Color emptyDeckImageColor;

    private bool[] isUnitDeckEmpty;
    private bool[] isUnitSelect;

    private int[] unitDeckIndex;


    private List<CommandSkillManager.SkillData> commandskillList = new List<CommandSkillManager.SkillData>();
    private CommandSkillManager commandskillDataManager;



    // Start is called before the first frame update
    void Start()
    {
        commandskillDataManager = FindObjectOfType<CommandSkillManager>();

        emptyDeckImageSprite = commandSkillDeckList[0].sprite;
        emptyDeckImageColor = commandSkillDeckList[0].color;

        isUnitDeckEmpty = new bool[commandSkillDeckList.Length];

        unitDeckIndex = new int[commandSkillDeckList.Length];

        // UI 판넬 On/Off
        if (stageStartBtn != null)
        {
            stageStartBtn.onClick.AddListener(() =>
            {
                localSituationPanel.SetActive(true);
            });
        }

        if(localSituationPanelCloseBtn != null)
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

        for (int i = 0; i < isUnitDeckEmpty.Length; i++)
        {
            isUnitDeckEmpty[i] = true;
            unitDeckIndex[i] = -1;
        }

        isUnitSelect = new bool[commandSkillList.Length];

        for (int i = 0; i < isUnitSelect.Length; i++)
        {
            isUnitSelect[i] = false;
        }

        for (int i = 0; i < commandSkillEquipBtn.Length; i++)
        {
            
            if (commandSkillEquipBtn[i] != null)
            {
                int unitIndex = i;
                commandSkillEquipBtn[i].onClick.AddListener(() => _addUnitToDeck(unitIndex));
            }
        }

        for (int i = 0; i < commandSkillClearBtn.Length; i++)
        {

            if (commandSkillClearBtn[i] != null)
            {
                int deckIndex = i;
                commandSkillClearBtn[i].onClick.AddListener(() => _clearUnitDeck(deckIndex));
            }
        }

        
        for (int i = 0; i < commandSkillClearBtn.Length; i++)
        {
            int index = i;
            commandSkillClearBtn[index].onClick.AddListener(() => AddCommandSkillToList(index));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void _addUnitToDeck(int unitIndex)
    {
        if (isUnitSelect[unitIndex])
        {
            Debug.Log("컨맨더 스킬 중복");
            return;
        }


        for (int i = 0; i < isUnitDeckEmpty.Length; i++)
        {
            if (isUnitDeckEmpty[i])
            {
                commandSkillDeckList[i].sprite = commandSkillList[unitIndex].sprite;
                commandSkillDeckList[i].color = commandSkillList[unitIndex].color;
                isUnitDeckEmpty[i] = false;
                isUnitSelect[unitIndex] = true;
                unitDeckIndex[i] = unitIndex;
                break;
            }
        }
    }

    void _clearUnitDeck(int deckIndex)
    {
        int unitIndex = unitDeckIndex[deckIndex];
        if (unitIndex != -1)
        {
            commandSkillDeckList[deckIndex].sprite = emptyDeckImageSprite;
            commandSkillDeckList[deckIndex].color = emptyDeckImageColor;
            isUnitDeckEmpty[deckIndex] = true;
            isUnitSelect[unitIndex] = false;
            unitDeckIndex[deckIndex] = -1;
        }
    }

    private void AddCommandSkillToList(int index)
    {
        string stringIndex = index.ToString();

        // CommandSkillManager 딕셔너리에서 특정 CommandSkill 가져오기
        if (commandskillDataManager.skillDataDictionary.TryGetValue(stringIndex, out CommandSkillManager.SkillData skill))
        {
            commandskillList.Add(skill); // 리스트에 추가
            Debug.Log($"CommandSkill {index} 추가됨");
        }
        else
        {
            Debug.LogWarning("해당 인덱스에 대한 CommandSkill을 찾을 수 없습니다.");
        }
    }

    public void ClearUnitDeck(int deckIndex)
    {
        if (deckIndex >= 0 && deckIndex < commandskillList.Count)
        {
            commandskillList.RemoveAt(deckIndex); // 리스트에서 해당 인덱스 제거
            Debug.Log($"CommandSkill {deckIndex} 삭제됨");
        }
        else
        {
            Debug.LogWarning("유효하지 않은 인덱스입니다.");
        }
    }
}
