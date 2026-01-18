using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class CommanderSkillUI : MonoBehaviour
{
    [SerializeField] private CommandSkillRepository cSkillRepository;
    [SerializeField] private GameObject[] pageBtnArray;
    [SerializeField] private CommandSkillBtnUI[] cSkillBtnArray;
    [SerializeField] private SelectedCommanderSkillUI selectedSkillUI;

    private CommandSkillData[] datas = new CommandSkillData[] { };
    private CommandSkillData[] currentSelected = new CommandSkillData[3];
    private List<CommandSkillData> canUse = new List<CommandSkillData>();
    private int skillCount = 0;
    private int pageNum = 1;


    private void LoadSelectedSkill()
    {
        Debug.Log($"리소스에서 로드한 지휘관 스킬Id : {datas[0].Id}, {datas[1].Id}, {datas[2].Id}");

        List<string> selectedSkillList = PlayerPrefsData.instance.GetSelectedCommanderSkill();
        for (int i = 0; i < selectedSkillList.Count; i++)
        {
            Debug.Log($"{selectedSkillList.Count}");

            for (int j = 0; j < datas.Length; j++)
            {
                Debug.Log($"{datas.Length}");
                Debug.Log($"리소스 로드 {datas[j].Id}, Length: {datas[j].Id.Length}");
                Debug.Log($"프랩스 로드 {selectedSkillList[i]}, Length: {selectedSkillList[i].Length}");
                Debug.Log($"결과 : {datas[j].Id == selectedSkillList[i]}");

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
        Debug.Log($"프랩스에서 가져온 스킬Id : {selectedSkillList[0]}, {selectedSkillList[1]}, {selectedSkillList[2]}");
        Debug.Log($"선택 지휘관 스킬로 셋팅된 ID : {currentSelected[0].Id}, {currentSelected[1].Id}, {currentSelected[2].Id}");


    }

    private void LoadCanUseSkill()
    {
        Debug.Log($"리소스에서 로드한 지휘관 스킬Id : {datas[0].Id}, {datas[1].Id}, {datas[2].Id}");

        List<string> casUseCSkillList = PlayerPrefsData.instance.GetHaveCommanderSkill();
        for (int i = 0; i < casUseCSkillList.Count; i++)
        {
            Debug.Log($"{casUseCSkillList.Count}");

            for (int j = 0; j < datas.Length; j++)
            {
                Debug.Log($"{datas.Length}");
                Debug.Log($"리소스 로드 {datas[j].Id}, Length: {datas[j].Id.Length}");
                Debug.Log($"프랩스 로드 {casUseCSkillList[i]}, Length: {casUseCSkillList[i].Length}");
                Debug.Log($"결과 : {datas[j].Id == casUseCSkillList[i]}");

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

    public void SetCSkillBtn(int i, bool canUse, Sprite image, string name, string desc, string effect)
    {
        cSkillBtnArray[i].SetBtn(i, canUse, image, name, desc, effect);
        cSkillBtnArray[i].gameObject.SetActive(true);

        // To do: 해금여부에 따른 잠금이미지 & 업적 비활성화
    }

    private void ShowCommandSkill()
    {
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

            SetCSkillBtn(i, canUseSkill, cData.Icon,
                LocalizationSettings.StringDatabase.
                GetLocalizedString("CommanderSkill", $"{skillNameId}", LocalizationSettings.SelectedLocale),
                LocalizationSettings.StringDatabase.
                GetLocalizedString("CommanderSkill", $"{skillDescId}", LocalizationSettings.SelectedLocale),
                LocalizationSettings.StringDatabase.
                GetLocalizedString("CommanderSkill", $"{skillEffectId}", LocalizationSettings.SelectedLocale));  //unit.Name);

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
        ShowCommandSkill();
        gameObject.SetActive(true );
    }

    public void HideUI()    // 뒤로가기
    {
        gameObject.SetActive(false);
    }
}
