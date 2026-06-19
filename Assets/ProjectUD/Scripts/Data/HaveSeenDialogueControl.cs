using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaveSeenDialogueControl : MonoBehaviour
{
    public void HaveSeenAllDialogue()
    {
        // 인트로 플래그
        PlayerPrefs.SetInt("IntroVideo", 1);

        // 메인 화면 대화 플래그
        PlayerPrefs.SetInt("FirstMainDialogue", 1);
        PlayerPrefs.SetInt("AfterTutorialDialogue", 1);
        PlayerPrefs.SetInt("AfterGameDialogue", 1);
        PlayerPrefs.SetInt("AfterGameWinDialogue", 1);
    }
}
