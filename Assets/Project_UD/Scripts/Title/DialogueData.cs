using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ProjectUD/DialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        [SerializeField] private string speakerName;
        [SerializeField, TextArea] private string text;

        public string SpeakerName => speakerName;
        public string Text => text;
    }

    [SerializeField] private DialogueLine[] lines;
    public DialogueLine[] Lines => lines;
}
