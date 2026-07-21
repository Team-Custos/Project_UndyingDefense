using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CharacterArchiveUI : MonoBehaviour, IInputESC
{
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private ResourcesRepository fRepository;
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

    // LoPol 추가
    [SerializeField] private Button[] tabButtons;  // 탭 버튼들
    [SerializeField] private TextMeshProUGUI[] tabButtonTexts; // 버튼 텍스트
    [SerializeField] private Image[] tabButtonImages; // 탭 버튼 이미지들
    [SerializeField] private Sprite selectedSprite; // 선택 이미지
    [SerializeField] private Color selectedColor; // 선택 색상
    [SerializeField] private Sprite normalSprite;   // 기본 이미지
    [SerializeField] private Color normalColor;   // 기본 색상
    [SerializeField] private string[] buttonKeys;

    [SerializeField] private Image[] pageBtnImages;
    [SerializeField] private Sprite pageSelectedSprite;
    [SerializeField] private Sprite pageNormalSprite;


    private void Start()
    {
        //ShowCharacterArchive();
    }


    public void OnTabBtnClick(string name)  // 버튼 클릭 이벤트용 함수
    {
        // LoPol 추가
        for (int i = 0; i < tabButtons.Length; i++)
        {
            // 선택된 버튼인지 비교
            bool isSelected = (buttonKeys[i] == name);

            // 이미지 교체
            tabButtonImages[i].sprite = isSelected ? selectedSprite : normalSprite;
            // 글자 색 변경
            tabButtonTexts[i].color = isSelected ? selectedColor : normalColor;
        }


        fName = name;
        pageNum = 1;

        string factionId = fNameTextTable.GetName(name);
        unitFactionName.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", $"{factionId}", LocalizationSettings.SelectedLocale);

        SetPage();
        ShowUnit();
        OnCharacterBtnClick(0);   // 첫번째 캐릭터 정보 보여주기

        unitDetailPanel.SetActive(false);
        isDetailOn = false;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
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

        // LoPol 추가
        for (int i = 0; i < pageBtnArray.Length; i++)
        {
            Image img = pageBtnArray[i].GetComponent<Image>();

            if (img != null)
                img.sprite = ((i + 1) == pageNum) ? pageSelectedSprite : pageNormalSprite;
        }
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


             // lopol 추가
            // 선택 버튼인지 판별해서 이미지 변경
            Image img = pageBtnArray[i].GetComponent<Image>();
            if (img != null)
                img.sprite = ((i + 1) == pageNum) ? pageSelectedSprite : pageNormalSprite;
        }
    }

    public void ShowUnit()
    {
        ResetCharacterBtn();

        int toShow = unitCount - ((pageNum - 1) * 9);
        int temp = Mathf.Min(toShow, 9);

        for (int i = 0; i < temp; i++)
        {
            UnitData unit = units[((pageNum - 1) * 9) + i];     // 보여줘야할 데이터 순번

            string unitId = unit.Id + "_name";
            
            SetCharacterBtn(i, unit.Icon,
                LocalizationSettings.StringDatabase.
                GetLocalizedString("UnitStringData(Name, Description)", $"{unitId}", LocalizationSettings.SelectedLocale));  //unit.Name);
            characterBtnArray[i].gameObject.SetActive(true);
        }
    }

    public void ShowCharacterArchive()  // 로비 버튼용
    {
        gameObject.SetActive(true);
        string name = "ally";
        OnTabBtnClick(name);    // 도감창 열면 보여질 첫 페이지

        inputEventManager.OnESCTarget = this;

        // 탭버튼 누를 때마다 클릭 사운드 재생되므로 주석 처리
        //if (SoundManager.Instance != null)
        //{
        //    SoundManager.Instance.PlayUIClickSFX();
        //}
    }

    public void HideCharacterArchive()  // 도감창 뒤로가기 버튼용
    {
        gameObject.SetActive(false);

        inputEventManager.OnESCTarget = null;

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HideCharacterArchive();
        }
    }
}
