using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class NickNamePanel : MonoBehaviour
{
    [Header("ReferenceClass")]
    [SerializeField] private AccountInfo accountInfo;
    [SerializeField] private LobbyManager lobbyManager;
    [SerializeField] private MessageUI messageUI;

    [Header("UIComponent")]
    [SerializeField] private Button checkBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private TMP_InputField nickNameInput;

    [Header("Localization")]
    [SerializeField] private string tableName; // = "LobbyUI";            // 테이블 이름
    [SerializeField] private string errorKey; // = "MSG_nicknameWarning"; // Key 이름

    private const int MIN_LENGTH = 1;
    private const int MAX_LENGTH = 12;
    private readonly Regex validPattern = new Regex(@"^[a-zA-Z0-9가-힣ㄱ-ㅎㅏ-ㅣ]+$");    // 영어, 숫자, 한글만 허용하는 정규화 패턴


    /*
    public void OnClickCheckBtn()
    {
        PlayerPrefs.SetString("PlayerName", nickNameInput.text);
        accountInfo.SetNickName(nickNameInput.text);
        lobbyManager.SetLobbyNickName(nickNameInput.text);
        gameObject.SetActive(false);
    }
*/
    public void OnClickCheckBtn()   // 확인버튼
    {
        string nickname = nickNameInput.text.Trim();

        if (!ValidateNickname(nickname))
        {
            string errorMsg = LocalizationSettings.StringDatabase
                .GetLocalizedString(tableName, errorKey, LocalizationSettings.SelectedLocale);
            Debug.Log($"로컬라이즈 에러메세지는 : {errorMsg}");
            messageUI.AddMessage(errorMsg);
            return;
        }

        PlayerPrefs.SetString("PlayerName", nickname);
        accountInfo.SetNickName(nickname);
        lobbyManager.SetLobbyNickName(nickname);
        gameObject.SetActive(false);
    }

    public void OnClickCancleBtn()  // 취소버튼
    {
        gameObject.SetActive(false);
    }

    public void ShowNicknamePanel()
    {
        nickNameInput.text = PlayerPrefs.GetString("PlayerName");
        gameObject.SetActive(true);
    }

    private bool ValidateNickname(string nickname)
    {
        if (string.IsNullOrEmpty(nickname) || nickname.Length < MIN_LENGTH || nickname.Length > MAX_LENGTH)
        {
            Debug.Log("글자수에러");
            return false;

        }

        return validPattern.IsMatch(nickname);
    }
}
