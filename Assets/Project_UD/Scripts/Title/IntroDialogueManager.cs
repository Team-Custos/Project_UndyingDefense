using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroDialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private IntroScene introScene;

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button nextBtn;

    [SerializeField] private Button acceptBtn;
    [SerializeField] private Button refuseBtn;

    private int currentLineIndex = 0;

    private void Start()
    {
        StartDialogue(dialogueData);
    }

    public void StartDialogue(DialogueData dialogueData)
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

    private void ShowDialogueLine()
    {
        var line = dialogueData.Lines[currentLineIndex];
        dialogueText.text = line.Text;
    }

    private void EndDialogue()
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
