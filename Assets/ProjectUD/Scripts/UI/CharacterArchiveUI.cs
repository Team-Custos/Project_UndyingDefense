using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterArchiveUI : MonoBehaviour
{
    [SerializeField] private FactionCharacterRepository fRepository;
    [SerializeField] private CharacterButtonUI[] characterBtnArray;
    [SerializeField] private GameObject[] pageBtnArray;
    [SerializeField] private TextMeshProUGUI unitFactionName;
    private UnitData[] units = new UnitData[] { };
    private int unitCount = 0;
    private int unitIndex = 0;
    private int pageNum = 1;
    private string fName;


    private void Start()
    {
        pageNum = 1;
    }

    public void OnTabBtnClick(string name)  // 버튼 클릭 이벤트용 함수
    {
        fName = name;
        SetPage();
        ShowUnit();
    }

    public void SetFactionName(string kFactionName)
    {
        unitFactionName.text = kFactionName;
    }

    public void OnCharacterBtnClick(int buttonIndex)  // 버튼 클릭 이벤트용 함수
    {
        unitIndex = buttonIndex;
    }

    public void SetButtonData(int i, Sprite image, string cName)
    {
        characterBtnArray[i].SetButton(image, cName);
        characterBtnArray[i].gameObject.SetActive(true);
    }

    public void OnPageBtnClick(int num)
    {
        pageNum = num;
        ShowUnit();
    }

    public void ResetButton()
    {
        for (int i = 0; i < characterBtnArray.Length; i++)
        {
            characterBtnArray[i].gameObject.SetActive(false);
            characterBtnArray[i].ResetButton();
        }

        for(int i = 0; i < pageBtnArray.Length; i++)
        {
            pageBtnArray[i].gameObject.SetActive(false);
        }
    }

    public void SetPage()
    {
        ResetButton();

        units = fRepository.GetFactionArray(fName);
        unitCount = units.Length;

        int pageCount = (unitCount / 9) + 1;    // 표시될 페이지 수 
        // To do : 딱 나눠 떨어질때 버튼이 하나 더 생성됨. 에외처리 해주기

        for(int i = 0; i < pageCount; i++) 
        {
            // 미리 만들어 놓고 활성화
            pageBtnArray[i].gameObject.SetActive(true);
        }

    }

    public void ShowUnit()
    {
        int toShow = unitCount - ((pageNum - 1) * 9);
        int temp = (toShow > 9) ? 9 : toShow;
        //int temp2 = Mathf.Min(toShow, 9);

        
        for (int i = 0;i < temp;i++)
        {
            UnitData unit = units[((pageNum - 1) * 9) + i];     // 보여줘야할 데이터 순번
            SetButtonData(i, unit.Icon, unit.Name);
            characterBtnArray[i].gameObject.SetActive(true);
        }
        
    }
}
