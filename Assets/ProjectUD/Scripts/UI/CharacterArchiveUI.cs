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
    [SerializeField] private FactionNameTextTable fNameTextTable;

    [Header("UnitInfoPanel")]
    [SerializeField] private UnitInfoPanelUI unitInfoPanel;

    [Header("UnitDetail")]
    [SerializeField] private GameObject unitDetailPanel;

    [Header("Indicator")]
    [SerializeField] private RectTransform indicator;

    private UnitData[] units = new UnitData[] { };
    private int unitCount = 0;
    private int unitIndex = 0;
    private int pageNum = 1;
    private string fName;
    private bool isDetailOn = false;

    private void Start()
    {
        //ShowCharacterArchive();
    }


    public void OnTabBtnClick(string name)  // 버튼 클릭 이벤트용 함수
    {
        fName = name;
        pageNum = 1;
        unitFactionName.text = fNameTextTable.GetName(name);
        SetPage();
        ShowUnit();
        OnCharacterBtnClick(0);   // 첫번째 캐릭터 정보 보여주기

        unitDetailPanel.SetActive(false);
        isDetailOn = false;
    }

    public void OnCharacterBtnClick(int buttonIndex)  // 버튼 클릭 이벤트용 함수
    {
        unitIndex = ((pageNum - 1) * 9) + buttonIndex;
        unitInfoPanel.SetUnitData(units[unitIndex]);
        indicator.transform.position = characterBtnArray[buttonIndex].transform.position;
        indicator.gameObject.SetActive(true);

        unitDetailPanel.SetActive(false);
        isDetailOn = false;
    }

    public void ToggleUnitDetailBtn()
    {
        if (isDetailOn)
        {
            unitDetailPanel.SetActive(false);
            isDetailOn = false;
        }
        else
        {
            unitDetailPanel.SetActive(true);
            isDetailOn = true;
        }
    }


    public void SetCharacterBtn(int i, Sprite image, string text)
    {
        characterBtnArray[i].SetButton(image, text);
        characterBtnArray[i].gameObject.SetActive(true);
    }

    public void OnPageBtnClick(int num)     // 버튼 클릭 이벤트용 함수
    {
        pageNum = num;
        ShowUnit();
        OnCharacterBtnClick(0);   // 첫번째 캐릭터 정보 보여주기
    }

    public void ResetCharacterBtn()
    {
        for (int i = 0; i < characterBtnArray.Length; i++)
        {
            characterBtnArray[i].gameObject.SetActive(false);
            characterBtnArray[i].ResetButton();
        }
    }

    public void ResetPageBtn()
    {
        for (int i = 0; i < pageBtnArray.Length; i++)
        {
            pageBtnArray[i].gameObject.SetActive(false);
        }
    }

    public void SetPage()
    {
        ResetPageBtn();
        ResetCharacterBtn();

        units = fRepository.GetFactionArray(fName);
        unitCount = units.Length;

        // 페이지 버튼 개수
        int pageCount = (unitCount % 9 == 0) ? unitCount / 9 : (unitCount / 9) + 1;

        for (int i = 0; i < pageCount; i++) 
        {
            // 미리 만들어 놓고 활성화
            pageBtnArray[i].gameObject.SetActive(true);
        }

    }

    public void ShowUnit()
    {
        ResetCharacterBtn();

        int toShow = unitCount - ((pageNum - 1) * 9);
        //int temp = (toShow > 9) ? 9 : toShow;
        int temp = Mathf.Min(toShow, 9);

        
        for (int i = 0;i < temp;i++)
        {
            UnitData unit = units[((pageNum - 1) * 9) + i];     // 보여줘야할 데이터 순번
            SetCharacterBtn(i, unit.Icon, unit.Name);
            characterBtnArray[i].gameObject.SetActive(true);
        }
        
    }

    public void ShowCharacterArchive()  // 로비 버튼용
    {
        gameObject.SetActive(true);
        string name = "ally";
        OnTabBtnClick(name);    // 도감창 열면 보여질 첫 페이지
    }

    public void HideCharacterArchive()  // 도감창 뒤로가기 버튼용
    {
        gameObject.SetActive(false);
    }
}
