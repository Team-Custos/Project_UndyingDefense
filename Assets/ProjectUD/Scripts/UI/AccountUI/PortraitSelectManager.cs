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
    [Header("RectTransform")]
    [SerializeField] private RectTransform portraitSelectUI;
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
    private PortraitData currentPortraitData;
    private PortraitData onSelectPortraitData;

    public void LoadPortraitData()  // 로비매니저에서 호출
    {
        portraitDataList = new List<PortraitData>(Resources.LoadAll<PortraitData>("Data/PortraitData"));
        dataCount = portraitDataList.Count;
    }
    
    public void SetPage()
    {
        portraitSelectPanel.SetActive(true);
        //LoadPortraitData();
        SetPageBtn();
        SetPortrait();

        inputEventManager.OnClickTarget = this;
        inputEventManager.OnESCTarget = this;
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
        for (int i = 0; i < temp; i++)
        {
            portraits[i].SetPortraitData(portraitDataList[((pageNum - 1) * 12) + i], playerRank);
            portraits[i].gameObject.SetActive(true);
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
        }
    }

    public void OnClickConfirm()
    {
        if (onSelectPortraitData != null)
        {
            SetPortraitData(onSelectPortraitData);
            confirmBtn.interactable = false;
            indicator.gameObject.SetActive(false);
            //isChange = false;

            // 선택한 초상화 저장
            PlayerPrefs.SetInt("SelectedPortraitID", onSelectPortraitData.portraitID);
            PlayerPrefs.Save();
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!portraitSelectPanel.activeSelf)
            return;

        if (context.performed)
        {
            Debug.Log("클릭 감지됨");
            // 클릭한 위치의 UI 요소 감지
            Vector2 clickPosition = Mouse.current.position.ReadValue();
            
            // 클릭한 위치가 초상화 선택 UI 내에 있는지 확인
            // 초상화 선택 UI가 아니라면 UI 닫기
            if (!RectTransformUtility.RectangleContainsScreenPoint(portraitSelectUI, clickPosition))
            {
                portraitSelectPanel.SetActive(false);
                indicator.gameObject.SetActive(false);
                confirmBtn.interactable = false;
            }
        }
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            portraitSelectPanel.SetActive(false);
            indicator.gameObject.SetActive(false);
            confirmBtn.interactable = false;
        }
    }
}
