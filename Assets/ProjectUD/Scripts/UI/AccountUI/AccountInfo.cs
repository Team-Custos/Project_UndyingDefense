using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class AccountInfo : MonoBehaviour, IInputESC
{
    [Header("Input Event Manager")]
    [SerializeField] private PlayerInputEventManager inputEventManager;

    [Header("Rank System")]
    [SerializeField] private RankSystem rankSystem;

    [Header("-- Portrait/Nickname/Commander Rank --")]
    [SerializeField] private Image portraitLineImg;
    [SerializeField] private Button nickNameBtn;
    [SerializeField] private GameObject portraitPanel;
    [SerializeField] private NickNamePanel nickNamePanelScript;
    [SerializeField] private GameObject resetCheckPanel;
    [SerializeField] private TextMeshProUGUI nickNameText;
    [SerializeField] private TextMeshProUGUI commanderRankTxt;

    [Header("-- Point --")]
    [SerializeField] private TextMeshProUGUI pointPercentTxt;
    [SerializeField] private Image percentageBar;
    //[SerializeField] private Button resetBtn;
    [SerializeField] private PortraitSelectManager portraitSelectManager;

    public void Start()
    {
        // 닉네임 버튼에 클릭 이벤트 등록
        //NickNameBtn.onClick.AddListener(OnClickNickNameBtn);
        nickNameText.text = PlayerPrefs.GetString("PlayerName");
        string commanderID = PlayerPrefs.GetString("CommanderID");
        Debug.Log($"[RankSystem] 현재 지휘관 ID: {commanderID}");
        commanderRankTxt.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("LobbyUI", $"{commanderID}", LocalizationSettings.SelectedLocale);

        pointPercentTxt.text = $"{PlayerPrefs.GetFloat("Point").ToString()}/{rankSystem.NextRankData.requirePoint}";
        percentageBar.fillAmount = PlayerPrefs.GetFloat("Point") / rankSystem.NextRankData.requirePoint;
    }

    public void ShowAccountPanel(Sprite portraitLine)
    {
        portraitLineImg.sprite = portraitLine;
        gameObject.SetActive(true);

        inputEventManager.OnESCTarget = this;
    }

    public void OnClickNickNameBtn()
    {
        // 닉네임 변경 UI로 이동
        Debug.Log("닉네임 변경 버튼 클릭");
        nickNamePanelScript.ShowNicknamePanel();

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    public void SetNickName(string name)
    {
        nickNameText.text = name;
    }

    public void OnClickResetBtn()
    {
        resetCheckPanel.SetActive(true);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    // 기획상 게임초기화 버튼 삭제 (안쓰는 메서드)
    /*
    public void OnClickResetCheckBtn()  
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("게임 데이터 초기화 완료");
        resetCheckPanel.SetActive(false);
        PlayerPrefsData.instance.SetDefaultPlayerPrefs();
        PlayerPrefs.SetString("PlayerName", nickNameText.text);
        LoadingSceneManager.LoadScene("TitleScene_Lopol");
        // 초기화 후 필요한 UI 업데이트
        //nickNameText.text = "";
        //commanderRankTxt.text = "";
    }
    */

    public void OnClickResetCancelBtn()
    {
        resetCheckPanel.SetActive(false);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    public void OnClickPortraitBtn()
    {
        // 프로필 사진 변경 UI로 이동
        Debug.Log("프로필 사진 변경 버튼 클릭");
        portraitPanel.SetActive(true);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }

    }

    // 뒤로가기 버튼
    public void OnClickBackBtn()
    {
        if(nickNamePanelScript.gameObject.activeSelf)
        {
            nickNamePanelScript.gameObject.SetActive(false);
        }
        if(portraitPanel.activeSelf)
        {
            portraitPanel.SetActive(false);
            portraitSelectManager.DisableSelectedPanel();
        }
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
            //if (portraitPanel.activeSelf)
            //{
            //    portraitPanel.SetActive(false);
            //}
            if (nickNamePanelScript.gameObject.activeSelf)
            {
                //nickNamePanelScript.gameObject.SetActive(false);
                return;
            }
            gameObject.SetActive(false);

            inputEventManager.OnESCTarget = null;

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayUIClickSFX();
            }
        }
    }

    // 강제로 마우스 호버링 상태를 해제하는 기능
    private void OnDisable()
    {
        var hoverHandlers = GetComponentsInChildren<Ingame_ImageHoverHandler>(true);
        foreach (var handler in hoverHandlers)
        {
            handler.ForceExit();
        }
    }
}
