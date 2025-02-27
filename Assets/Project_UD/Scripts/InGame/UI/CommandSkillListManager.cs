using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CommandSkillListManager : MonoBehaviour, IPointerClickHandler
{
    public CommandSkillData commandSkillData;

    public CommandSkillDeckManager[] commandSkillDeckManagers;

    public Button commandSkillListBtn;
    public Image commandSkillListImage;

    public Image commandSkillInfoImage;
    public Text commandSkillInfoName;
    public Text commandSkillInfoDescription;

    public Image[] commandSkillSlotImage;
    public Button[] commandSkillSlotBtn;

    private int index;


    // Start is called before the first frame update
    void Start()
    {
        commandSkillListImage.sprite = commandSkillData.commandSkillImage;

        commandSkillListBtn.onClick.AddListener(() =>
        {
            // 리스트의 크기가 3 이상인 경우
            if (UserDataModel.instance.skillDatas.Count >= 3)
            {
                // 중복된 스킬이 있는지 확인
                if (UserDataModel.instance.skillDatas.Contains(commandSkillData))
                {
                    Debug.Log("이미 추가된 스킬입니다.");
                    return;
                }

                // 리스트를 순회하며 null 값을 찾음
                bool isNullFound = false;
                for (int i = 0; i < UserDataModel.instance.skillDatas.Count; i++)
                {
                    if (UserDataModel.instance.skillDatas[i] == null)
                    {
                        // null 값을 새로운 데이터로 교체
                        UserDataModel.instance.skillDatas[i] = commandSkillData;
                        commandSkillDeckManagers[i].commandSkillDeckImage.sprite = commandSkillData.commandSkillImage;
                        isNullFound = true;
                        Debug.Log("null 값을 새로운 데이터로 교체했습니다.");
                        break; // null 값을 찾았으므로 반복문 종료
                    }
                }

                // null 값이 없으면 리턴
                if (!isNullFound)
                {
                    Debug.Log("이미 추가된 스킬이거나 스킬이 3개 이상입니다.");
                    return;
                }
            }
            // 리스트의 크기가 3 미만인 경우
            else
            {
                // 중복된 스킬이 있는지 확인
                if (UserDataModel.instance.skillDatas.Contains(commandSkillData))
                {
                    Debug.Log("이미 추가된 스킬입니다.");
                    return;
                }

                // 새로운 스킬 추가
                UserDataModel.instance.skillDatas.Add(commandSkillData);
                Debug.Log(UserDataModel.instance.skillDatas.Count);

                // 덱의 이미지 업데이트
                for (int i = 0; i < UserDataModel.instance.skillDatas.Count; i++)
                {
                    commandSkillDeckManagers[i].commandSkillDeckImage.sprite = UserDataModel.instance.skillDatas[i].commandSkillImage;
                }
            }
        });


    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(commandSkillData != null)
        {
            commandSkillInfoImage.sprite = commandSkillData.commandSkillImage;
            commandSkillInfoName.text = commandSkillData.commandSkillName;
            commandSkillInfoDescription.text = commandSkillData.commandSkillDescription;
        }
    }
}
