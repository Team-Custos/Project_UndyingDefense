using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] protected DialogueData dialogueData;
    [SerializeField] protected DialogueData ingamedialogueData;
    [SerializeField] private IntroScene introScene;

    [SerializeField] protected TextMeshProUGUI dialogueText;
    [SerializeField] protected Button nextBtn;

    [SerializeField] private Button acceptBtn;
    [SerializeField] private Button refuseBtn;

    protected int currentLineIndex = 0;

    protected virtual void Start()
    {
        StartDialogue(dialogueData);
    }

    protected void StartDialogue(DialogueData dialogueData)
    {
        this.dialogueData = dialogueData;
        currentLineIndex = 0;
        ShowDialogueLine();
    }

    public void OnNextButtonClicked()
    {
        currentLineIndex++;

        if (currentLineIndex < dialogueData.Lines.Length - 1)
        {
            ShowDialogueLine();
        }
        else if (currentLineIndex == dialogueData.Lines.Length - 1)
        {
            ShowDialogueLine();
            EndDialogue();
        }
    }

    protected void ShowDialogueLine()
    {
        var line = dialogueData.Lines[currentLineIndex];
        dialogueText.text = line.Text;
    }

    protected virtual void EndDialogue()
    {
        nextBtn.gameObject.SetActive(false);

        acceptBtn.gameObject.SetActive(true);
        refuseBtn.gameObject.SetActive(true);
    }

    


    public void PlayDeclaration()
    {
        introScene.PlayDeclarationDropAnimation();
    }

    public void RetrunTitle()
    {
        LoadingSceneManager.LoadScene("TitleScene_LoPol");
    }
}
