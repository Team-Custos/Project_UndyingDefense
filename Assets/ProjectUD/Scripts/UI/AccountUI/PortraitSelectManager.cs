using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PortraitSelectManager : MonoBehaviour, IInputClick, IInputESC
{
    [Header("PlayerInputEventManager")]
    [SerializeField] private PlayerInputEventManager inputEventManager;

    [Header("AccountUI")]
    [SerializeField] private AccountInfo accountUI;

    [Header("RectTransform")]
    [SerializeField] private RectTransform portraitSelectUI;
    [SerializeField] private RectTransform portraitOpenBtn; // 버튼 RectTransform 추가
    [SerializeField] private RectTransform panelCloseBtn; // 패널 뒤로가기 버튼

    [Header("Button Sprites")]
    [SerializeField] private Image portraitOpenBtnImage; // 버튼 이미지 추가
    [SerializeField] private Sprite btnNormalSprite;
    [SerializeField] private Sprite btnPressedSprite;

    [Header("PortraitSelectPanel")]
    [SerializeField] private GameObject portraitSelectPanel;
    // 계정 정보 UI의 초상화 이미지
    [SerializeField] private Image portraitImg;
    // 로비 초상화 이미지
    [SerializeField] private Image lobbyPortraitImg;
    // 초상화 UI 리스트
    [SerializeField] private List<PortraitUI> portraits;
    [SerializeField] private GameObject[] pageBtnArray;
    // 적용 버튼
    [SerializeField] private Button confirmBtn;
    // 선택 인디케이터 이미지
    [SerializeField] private GameObject indicator;
    // 이미지데이터 불러와서 저장
    private List<PortraitData> portraitDataList;
    private int dataCount = 0;
    private int pageNum = 1;
    private bool isChange = false;
    private bool isSelectedPanelActive = false;
    private PortraitData currentPortraitData;
    private PortraitData onSelectPortraitData;

    public void LoadPortraitData()  // 로비매니저에서 호출
    {
        portraitDataList = new List<PortraitData>(Resources.LoadAll<PortraitData>("Data/PortraitData"));
        dataCount = portraitDataList.Count;
    }

    // 초상화 선택 UI 활성화 및 초기 설정
    public void SetPage()
    {
        Debug.Log($"SetPage 호출 - isSelectedPanelActive: {isSelectedPanelActive}");
        if (isSelectedPanelActive)
        {
            DisableSelectedPanel();
            return;
        }
        portraitSelectPanel.SetActive(true);
        isSelectedPanelActive = true;
        portraitOpenBtnImage.sprite = btnPressedSprite;
        //LoadPortraitData();
        SetPageBtn();
        SetPortrait();

        inputEventManager.OnESCTarget = this;
        inputEventManager.OnClickTarget = this;

    }

    private void SetPageBtn()
    {
        ResetPageBtn();

        // 페이지 버튼 갯수
        int pageCount = (dataCount % 12 == 0) ? dataCount / 12 : (dataCount / 12) + 1;

        // 만들어 놓은 버튼 활성화
        for (int i = 0; i < pageCount; i++)
        {
            pageBtnArray[i].gameObject.SetActive(true);
        }
    }

    private void SetPortrait()
    {
        ResetPortrait();
        int playerRank = PlayerPrefs.GetInt("CommanderRank");

        int startIndex = (pageNum - 1) * portraits.Count;

        int toShow = dataCount - ((pageNum - 1) * 12);
        int temp = Mathf.Min(toShow, 12);

        bool foundCurrent = false;
        for (int i = 0; i < temp; i++)
        {
            var data = portraitDataList[((pageNum - 1) * 12) + i];
            portraits[i].SetPortraitData(data, playerRank);
            portraits[i].gameObject.SetActive(true);

            if(currentPortraitData != null && data.portraitID == currentPortraitData.portraitID)
            {
                indicator.transform.position = portraits[i].transform.position;
                indicator.gameObject.SetActive(true);
                foundCurrent = true;
            }
        }

        if (!foundCurrent)
        {
            indicator.gameObject.SetActive(false);
        }
    }

    private void ResetPortrait()
    {
        for (int i = 0; i < portraits.Count; i++)
        {
            portraits[i].ResetPortraitUI();
            portraits[i].gameObject.SetActive(false);
        }
    }

    private void ResetPageBtn()
    {
        for (int i = 0; i < pageBtnArray.Length; i++)
        {
            pageBtnArray[i].gameObject.SetActive(false);
        }
    }

    public void OnPageBtnClick(int num)     // 버튼 클릭 이벤트용 함수
    {
        pageNum = num;
        SetPortrait();

        // 페이지 버튼 색상 변경
        //for (int i = 0; i < pageBtnArray.Length; i++)
        //{
        //    if (i == (num - 1))
        //    {
        //        pageBtnArray[i].GetComponent<PageButton>().ToggleSelected(true);
        //    }
        //    else
        //    {
        //        pageBtnArray[i].GetComponent<PageButton>().ToggleSelected(false);
        //    }
        //}
    }

    public void SetPortraitData(PortraitData portraitData)
    {
        currentPortraitData = portraitData;
        portraitImg.sprite = currentPortraitData.portrait;
        lobbyPortraitImg.sprite = currentPortraitData.portrait;
    }

    public void OnClickPortrait(int index)
    {
        indicator.transform.position = portraits[index].transform.position;
        indicator.gameObject.SetActive(true);

        onSelectPortraitData = portraits[index].GetPortraitData();
        if (onSelectPortraitData != null && onSelectPortraitData != currentPortraitData)
        {
            //isChange = true;

            // 선택한 초상화가 현재 적용된 초상화와 다를 때만 확인 버튼 활성화
            confirmBtn.interactable = (onSelectPortraitData != null && currentPortraitData != null 
                && onSelectPortraitData.portraitID != currentPortraitData.portraitID);
        }
    }

    public void LoadSavedPortrait()
    {
        if (PlayerPrefs.HasKey("SelectedPortraitID"))
        {
            int savedID = PlayerPrefs.GetInt("SelectedPortraitID");
            PortraitData saved = portraitDataList.Find(p => p.portraitID == savedID);
            if (saved != null)
                SetPortraitData(saved);
            return;
        }

        // 저장 데이터가 없거나, 저장된 ID에 해당하는 데이터를 못 찾은 경우
        // 기본값으로 0번 인덱스 적용
        if (portraitDataList != null && portraitDataList.Count > 0)
        {
            SetPortraitData(portraitDataList[0]);
        }
    }

    public void OnClickConfirm()
    {
        if (onSelectPortraitData != null)
        {
            SetPortraitData(onSelectPortraitData);
            confirmBtn.interactable = false;
            //indicator.gameObject.SetActive(false);
            //isChange = false;

            // 선택한 초상화 저장
            PlayerPrefs.SetInt("SelectedPortraitID", onSelectPortraitData.portraitID);
            PlayerPrefs.Save();
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log($"OnClick 호출 - performed: {context.performed}, panel: {portraitSelectPanel.activeSelf}");
        if (!portraitSelectPanel.activeSelf)
            return;

        if (context.performed)
        {
            Debug.Log("클릭 감지됨");
            // 클릭한 위치의 UI 요소 감지
            Vector2 clickPosition = Mouse.current.position.ReadValue();

            // 클릭한 위치가 초상화 선택 UI 내에 있는지 확인
            // 초상화 선택 UI가 아니라면 UI 닫기
            bool inPanel = RectTransformUtility.RectangleContainsScreenPoint(portraitSelectUI, clickPosition);
            bool inBtn = RectTransformUtility.RectangleContainsScreenPoint(portraitOpenBtn, clickPosition);
            bool inBackBtn = RectTransformUtility.RectangleContainsScreenPoint(panelCloseBtn, clickPosition);

            if (!inPanel && !inBtn && !inBackBtn)
            {
                DisableSelectedPanel();
            }
        }
    }

    public void DisableSelectedPanel()
    {
        Debug.Log("DisableSelectedPanel 호출");
        portraitSelectPanel.SetActive(false);
        indicator.gameObject.SetActive(false);
        confirmBtn.interactable = false;
        isSelectedPanelActive = false;

        portraitOpenBtnImage.sprite = btnNormalSprite; // 패널 닫힐 때 원래 상태

        inputEventManager.OnESCTarget = accountUI;  // ESC 입력 이벤트 타겟을 계정 정보 UI로 변경
        inputEventManager.OnClickTarget = null;
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DisableSelectedPanel();
        }
    }
}
