using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class CommanderSkillUI : MonoBehaviour
{
    [SerializeField] private ResourcesRepository cSkillRepository;
    [SerializeField] private GameObject[] pageBtnArray;
    [SerializeField] private CommandSkillBtnUI[] cSkillBtnArray;

    private CommandSkillData[] datas = new CommandSkillData[] { };
    private int skillCount = 0;
    private int pageNum = 1;

    private void Start()
    {
        ShowCommandSkillUI();
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

    public void SetCSkillBtn(int i, Sprite image, string text)
    {
        cSkillBtnArray[i].SetBtn(image, text);
        cSkillBtnArray[i].gameObject.SetActive(true);

        // To do: 해금여부에 따른 잠금이미지 & 업적 비활성화
    }

    private void ShowCommandSkill()
    {
        int toShow = skillCount - ((pageNum - 1) * 10);
        int temp = Mathf.Min(toShow, 10);

        for (int i = 0; i < temp; i++)
        {
            CommandSkillData cData = datas[((pageNum - 1) * 10) + i];     // 보여줘야할 데이터 순번

            string skillId = cData.Id + "_name";

            SetCSkillBtn(i, cData.Icon,
                LocalizationSettings.StringDatabase.
                GetLocalizedString("CommanderSkill", $"{skillId}", LocalizationSettings.SelectedLocale));  //unit.Name);
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
        ShowCommandSkill();
    }

    public void HideUI()    // 뒤로가기
    {

    }
}
