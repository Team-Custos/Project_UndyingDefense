using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataModel : MonoBehaviour
{
    public static UserDataModel instance;

    [SerializeField] private bool isTutorialEnd = false;
    [SerializeField] private bool isGameFinshed = false;
    [SerializeField] private bool isGameWin = false;
    [SerializeField] private bool introDialogueEnd = false;
    [SerializeField] private bool firstMainDialogue = false;
    [SerializeField] private bool afterTutorialDialogue = false;
    [SerializeField] private bool afterGameDialogue = false;
    [SerializeField] private bool afterGameWinDialogue = false;

    public bool IsTutorialEnd => isTutorialEnd;
    public bool IsGameFinished => isGameFinshed;
    public bool IsGameWin => isGameWin;
    public bool IntroDialogueEnd => introDialogueEnd;
    public bool FirstMainDialogue => firstMainDialogue;
    public bool AfterTutorialDialogue => afterTutorialDialogue;
    public bool AfterGameDialogue => afterGameDialogue;
    public bool AfterGameWinDialogue => afterGameWinDialogue;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetIntroDialogueEnd(bool value)
    {
        introDialogueEnd = value;
    }
    public void SetFirstMainDialogue(bool value)
    {
        firstMainDialogue = value;
    }
    public void SetAfterTutorialDialogue(bool value)
    {
        afterTutorialDialogue = value;
    }
    public void SetAfterGameDialogue(bool value)
    {
        afterGameDialogue = value;
    }
    public void SetAfterGameWinDialogue(bool value)
    {
        afterGameWinDialogue = value;
    }



    public void SetTutorialEnd(bool value)
    {
        isTutorialEnd = value;
    }
    public void SetGameFinished(bool value)
    {
        isGameFinshed = value;
    }
    public void SetGameWin(bool value)
    {
        isGameWin = value;
    }
}
