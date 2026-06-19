using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class SelectedCommanderSkillUI : MonoBehaviour
{
    [SerializeField] private SelectedCSkillBtnUI[] selectedCSkillBtns;
    [SerializeField] private CommanderSkillUI commanderSkillUI;
    [SerializeField] private MessageUI warningMessage;
    [SerializeField] private Button saveButton; // 저장 버튼 연결

    private CommandSkillData[] selectedCSkills = new CommandSkillData[3];
    private List<string> selectCSkillID = new List<string>();

    private int index;
    private bool canAdd = false;
    private bool isSaved = true;

    private void SetSaveButtonState(bool isChanged)
    {
        if (saveButton != null)
            saveButton.interactable = isChanged;
    }

    public void SetCSkillList(CommandSkillData[] datas)
    {
        selectedCSkills = datas;
        for (int i = 0; i < selectedCSkillBtns.Length; i++)
        {
            SetCSkill(i);
        }
        //--UI 열릴 때 항상 비활성화로 초기화
        isSaved = true;
        SetSaveButtonState(false); // 추가
    }

    public void SetCSkill(int index)
    {
        if (selectedCSkills[index] == null)
        {
            //selectedCSkillBtns[index].ClearSelectedCSkillUI();
            selectedCSkillBtns[index].SetSelectedCSkillUI(index, null, string.Empty, string.Empty, string.Empty);
            return;
        }
        string skillNameId = selectedCSkills[index].Id + "_name";
        string skillDescId = selectedCSkills[index].Id + "_desc";
        string skillEffectId = selectedCSkills[index].Id + "_effect";

        selectedCSkillBtns[index].SetSelectedCSkillUI(index, selectedCSkills[index],
            LocalizationSettings.StringDatabase.
            GetLocalizedString("CommanderSkill", $"{skillNameId}", LocalizationSettings.SelectedLocale),
            LocalizationSettings.StringDatabase.
            GetLocalizedString("CommanderSkill", $"{skillDescId}", LocalizationSettings.SelectedLocale),
            LocalizationSettings.StringDatabase.
            GetLocalizedString("CommanderSkill", $"{skillEffectId}", LocalizationSettings.SelectedLocale));
    }

    public bool AddSkill(CommandSkillData data)
    {
        for (int i = 0; i < selectedCSkills.Length; i++)
        {
            if(selectedCSkills[i] == null)
            {
                //canAdd = true;
                selectedCSkills[i] = data;
                SetCSkill(i);
                //--선택 스킬 변경
                isSaved = false;
                //--저장 버튼 활성화
                SetSaveButtonState(true); // 추가
                return true;
            }
        }
        warningMessage.AddMessage("모든 슬롯이 채워져 있습니다!");
        return false;
    }

    // 선택스킬 버튼용
    public void RemoveSkill(int index)
    {
        commanderSkillUI.DeSelectCommanderSkill(selectedCSkills[index]);
        selectedCSkills[index] = null;
        //selectedCSkillBtns[index].ClearSelectedCSkillUI();
        selectedCSkillBtns[index].SetSelectedCSkillUI(index, null, string.Empty, string.Empty, string.Empty);
        //--선택 스킬 변경
        isSaved = false;
        //--저장 버튼 활성화
        SetSaveButtonState(true); // 추가
    }

    public void RemoveSkill(CommandSkillData data)
    {
        //if(data != null && selectedCSkills.Contains(data))
        
        if(data == null)
            return;
        for (int i = 0; i < selectedCSkills.Length; i++)
        {
            if (selectedCSkills[i] == data)
            {
                RemoveSkill(i);
            }
        }
    }

    // 저장 버튼용
    public void SaveChoiceCommanderSkill()
    {
        selectCSkillID.Clear();
        //Debug.Log(selectedCSkills);

        for (int i = 0; i < selectedCSkills.Length; i++)
        {
            if (selectedCSkills[i] == null)
            {
                Debug.Log($"★★★선택된 지휘관 스킬 {i}번째 : 비어있음");
            }
            else
                Debug.Log($"★★★선택된 지휘관 스킬 {i}번째 : {selectedCSkills[i].Name}");
            // 비어있으면 빈문자열 저장
            if (selectedCSkills[i] == null)
            {
                selectCSkillID.Add(string.Empty);
                continue;
            }
            selectCSkillID.Add(selectedCSkills[i].Id);
        }
        for (int i = 0; i < selectCSkillID.Count; i++)
        {
            Debug.Log($"★★★저장할 지휘관 스킬 ID {i}번째 ID : {selectCSkillID[i]}");
        }
        PlayerPrefsData.instance.SetSelectCSkill(selectCSkillID);
        for (int i = 0; i < selectCSkillID.Count; i++)
        {

            Debug.Log($"★★★저장된 지휘관 스킬 : {selectCSkillID[i]}");
        }

        isSaved = true;
        // 저장 후 버튼 비활성화
        SetSaveButtonState(false); // 추가
    }
    public bool IsSaved()
    {
        return isSaved;
    }
}
