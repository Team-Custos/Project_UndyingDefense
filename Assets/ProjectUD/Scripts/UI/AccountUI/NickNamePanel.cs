using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NickNamePanel : MonoBehaviour
{
    [Header("ReferenceClass")]
    [SerializeField] private AccountInfo accountInfo;
    [SerializeField] private LobbyManager lobbyManager;

    [Header("UIComponent")]
    [SerializeField] private Button checkBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private TMP_InputField nickNameInput;

    public void OnClickCheckBtn()
    {
        PlayerPrefs.SetString("PlayerName", nickNameInput.text);
        accountInfo.SetNickName(nickNameInput.text);
        lobbyManager.SetLobbyNickName(nickNameInput.text);
        gameObject.SetActive(false);
    }

    public void OnClickCancleBtn()
    {
        gameObject.SetActive(false);
    }

    public void ShowNicknamePanel()
    {
        nickNameInput.text = PlayerPrefs.GetString("PlayerName");
        gameObject.SetActive(true);
    }

}
