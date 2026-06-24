using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CommanderSkillUI : MonoBehaviour, IInputESC
{
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private CommandSkillRepository cSkillRepository;
    [SerializeField] private GameObject[] pageBtnArray;
    [SerializeField] private CommandSkillBtnUI[] cSkillBtnArray;
    [SerializeField] private SelectedCommanderSkillUI selectedSkillUI;
    [SerializeField] private RankSystem rankSystem;

    [Header("지휘관 스킬 저장 확인창")]
    [SerializeField] private GameObject confirmSavePanel;

    [Header("보상 알림")]
    [SerializeField] private MessageUI rewardAlarm;

    [Header("버튼이미지")]
    [SerializeField] private Sprite pageSelectedSprite;
    [SerializeField] private Sprite pageNormalSprite;

    private CommandSkillData[] datas = new CommandSkillData[] { };
    private CommandSkillData[] currentSelected = new CommandSkillData[3];
    private List<CommandSkillData> canUse = new List<CommandSkillData>();
    private int skillCount = 0;
    private int pageNum = 1;

    private void ShowAlarm()
    {
        IReadOnlyList<string> alarms = rankSystem.GetRewardAlarms();

        if (alarms == null || alarms.Count == 0)
            return;

        for (int i = 0; i < alarms.Count; i++)
        {
            rewardAlarm.AddMessage($"[ {alarms[i]} ] 스킬을 배웠습니다!");
        }
        rankSystem.ResetAlarmList();
    }

    private void LoadSelectedSkill()
    {
        Debug.Log($"리소스에서 로드한 지휘관 스킬Id : {datas[0].Id}, {datas[1].Id}, {datas[2].Id}");
        for (int i = 0; i < currentSelected.Length; i++)
            currentSelected[i] = null;

        List<string> selectedSkillList = PlayerPrefsData.instance.GetSelectedCommanderSkill();
        Debug.Log($"★★불러온 선택 지휘관 스킬갯수 {selectedSkillList.Count}");
        for (int i = 0; i < selectedSkillList.Count; i++)
        {
            Debug.Log($"★★불러온 선택 지휘관스킬 : {selectedSkillList[i]}");
        }

        //--
        for (int i = 0; i < selectedSkillList.Count; i++)
        {
            
            // To do: 빈칸이 넘어오면 빈칸으로 처리
            if (string.IsNullOrEmpty(selectedSkillList[i]))
            {
                currentSelected[i] = null;
                continue;
            }

            for (int j = 0; j < datas.Length; j++)
            {
                //Debug.Log($"{datas.Length}");
                //Debug.Log($"리소스 로드 {datas[j].Id}, Length: {datas[j].Id.Length}");
                //Debug.Log($"프랩스 로드 {selectedSkillList[i]}, Length: {selectedSkillList[i].Length}");
                //Debug.Log($"결과 : {datas[j].Id == selectedSkillList[i]}");

                if (string.Compare(datas[j].Id, selectedSkillList[i]) == 0)
                {
                    Debug.Log("일치");
                    currentSelected[i] = datas[j];
                    //currentSelected.Add(datas[j]);
                    if (currentSelected[i] != null)
                    {
                        Debug.Log("지휘관스킬로드 성공");
                    }
                }
            }
        }
        selectedSkillUI.SetCSkillList(currentSelected);
        //Debug.Log($"프랩스에서 가져온 스킬Id : {selectedSkillList[0]}, {selectedSkillList[1]}, {selectedSkillList[2]}");
        //Debug.Log($"선택 지휘관 스킬로 셋팅된 ID : {currentSelected[0].Id}, {currentSelected[1].Id}, {currentSelected[2].Id}");


    }

    private void LoadCanUseSkill()
    {
        Debug.Log($"리소스에서 로드한 지휘관 스킬Id : {datas[0].Id}, {datas[1].Id}, {datas[2].Id}");

        List<string> casUseCSkillList = PlayerPrefsData.instance.GetHaveCommanderSkill();
        for (int i = 0; i < casUseCSkillList.Count; i++)
        {
            //Debug.Log($"{casUseCSkillList.Count}");
            Debug.Log($"{casUseCSkillList[i]}");

            for (int j = 0; j < datas.Length; j++)
            {
                //Debug.Log($"{datas.Length}");
                //Debug.Log($"리소스 로드 {datas[j].Id}, Length: {datas[j].Id.Length}");
                //Debug.Log($"프랩스 로드 {casUseCSkillList[i]}, Length: {casUseCSkillList[i].Length}");
                //Debug.Log($"결과 : {datas[j].Id == casUseCSkillList[i]}");

                if (string.Compare(datas[j].Id, casUseCSkillList[i]) == 0)
                {
                    Debug.Log("일치");
                    canUse.Add(datas[j]);
                }
            }
        }
        Debug.Log($"프랩스에서 가져온 사용가능스킬Id : {casUseCSkillList[0]}, {casUseCSkillList[1]}, {casUseCSkillList[2]}");
        Debug.Log($"사용가능 지휘관 스킬로 셋팅된 ID : {canUse[0].Id}, {canUse[1].Id}, {canUse[2].Id}");


    }

    // 버튼 클릭용
    public void SelectCommandSkill(int index)
    {
        if (!cSkillBtnArray[index].IsSelected())
        {
            if(selectedSkillUI.AddSkill(datas[((pageNum - 1) * 10) + index]))
            {
                cSkillBtnArray[index].ToggleSelected(true);
            }
        }
        else
        {
            selectedSkillUI.RemoveSkill(datas[((pageNum - 1) * 10) + index]);
            cSkillBtnArray[index].ToggleSelected(false);
        }
    }

    public void DeSelectCommanderSkill(CommandSkillData data)
    {
        for (int i = 0; i < cSkillBtnArray.Length; i++)
        {
            if(!cSkillBtnArray[i].gameObject.activeSelf)
                continue;

            if (datas[((pageNum - 1) * 10) + i] == data)
            {
                cSkillBtnArray[i].ToggleSelected(false);
            }
        }
    }

    private void SetPage()
    {
        ResetPageBtn();
        ResetCSkillBtn();

        datas = cSkillRepository.GetCommandSkills();
        skillCount = datas.Length;

        // 페이지 버튼 갯수
        int pageCount = (skillCount % 9 == 0) ? skillCount / 9 : (skillCount / 9) + 1;

        
        // 만들어 놓은 버튼 활성화
        for (int i = 0; i < pageCount; i++)
        {
            pageBtnArray[i].gameObject.SetActive(true);
        }

    }

    public void OnPageBtnClick(int num)     // 버튼 클릭 이벤트용 함수
    {
        pageNum = num;
        ShowCommandSkill();


        // LoPol 추가
        for (int i = 0; i < pageBtnArray.Length; i++)
        {
            Image img = pageBtnArray[i].GetComponent<Image>();

            if (img != null)
                img.sprite = ((i + 1) == pageNum) ? pageSelectedSprite : pageNormalSprite;
        }
    }

    public void SetCSkillBtn(int i, bool canUse, Sprite image, string name, string desc, string effect, string coolTime)
    {
        cSkillBtnArray[i].SetBtn(i, canUse, image, name, desc, effect, coolTime);
        cSkillBtnArray[i].gameObject.SetActive(true);

        // To do: 해금여부에 따른 잠금이미지 & 업적 비활성화
    }

    private void ShowCommandSkill()
    {
        //ResetPageBtn();
        ResetCSkillBtn();

        int toShow = skillCount - ((pageNum - 1) * 10);
        int temp = Mathf.Min(toShow, 10);
        bool canUseSkill = false;

        for (int i = 0; i < temp; i++)
        {
            CommandSkillData cData = datas[((pageNum - 1) * 10) + i];     // 보여줘야할 데이터 순번
            
            // 해금된 스킬인지 확인
            canUseSkill = canUse.Contains(cData) ? true : false;

            // 선택된 스킬인지 확인
            if (currentSelected.Contains(cData))
                cSkillBtnArray[i].ToggleSelected(true);

            string skillNameId = cData.Id + "_name";
            string skillDescId = cData.Id + "_desc";
            string skillEffectId = cData.Id + "_effect";

            var sCooltime = LocalizationSettings.StringDatabase.
           GetLocalizedString("CommonUI", "CON_skillCooltime",
           new object[] { new { num = cData.CoolTime } });

            SetCSkillBtn(i, canUseSkill, cData.Icon,
                LocalizationSettings.StringDatabase.
                GetLocalizedString("CommanderSkill", $"{skillNameId}", LocalizationSettings.SelectedLocale),
                LocalizationSettings.StringDatabase.
                GetLocalizedString("CommanderSkill", $"{skillDescId}", LocalizationSettings.SelectedLocale),
                LocalizationSettings.StringDatabase.
                GetLocalizedString("CommanderSkill", $"{skillEffectId}", LocalizationSettings.SelectedLocale),
                sCooltime
                );  //unit.Name);

            cSkillBtnArray[i].gameObject.SetActive(true);
        }
    }
    private void ResetPageBtn()
    {
        for (int i = 0; i < pageBtnArray.Length; i++)
        {
            pageBtnArray[i].gameObject.SetActive(false);
        }
    }

    private void ResetCSkillBtn()
    {
        for (int i = 0; i < cSkillBtnArray.Length; i++)
        {
            cSkillBtnArray[i].gameObject.SetActive(false);
            cSkillBtnArray[i].ResetButton();
        }
    }
    public void ShowCommandSkillUI()   // 로비 버튼용
    {
        SetPage();
        LoadCanUseSkill();
        LoadSelectedSkill();
        OnPageBtnClick(1);
        //ShowCommandSkill();
        gameObject.SetActive(true );
        ShowAlarm();

        inputEventManager.OnESCTarget = this;
    }

    public void HideUI()    // 뒤로가기
    {
        bool isSaved = selectedSkillUI.IsSaved();
        if (!isSaved)
        {
            confirmSavePanel.SetActive(true);

            inputEventManager.OnESCTarget = null;

            return;
        }
        gameObject.SetActive(false);

        inputEventManager.OnESCTarget = null;

    }

    public void ConfirmPanelCancleBtn()
    {
        confirmSavePanel.SetActive(false);

        inputEventManager.OnESCTarget = this;
    }

    public void ConfirmPanelOKBtn()
    {
        confirmSavePanel.SetActive(false);
        gameObject.SetActive(false);

        inputEventManager.OnESCTarget = null;
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HideUI();
        }
    }
}
