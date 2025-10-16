using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButtonUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Button button;
    private int index;
    private UltEvent nextEvent;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
    }

    public void SetButton(int i, string buttonText, UltEvent buttonEvent)
    {
        index = i;
        text.text = buttonText;
        nextEvent = buttonEvent;
        //button.onClick.AddListener(() => buttonEvent.Invoke());
    }

    public void ResetButton()
    {
        text.text = "";
        //button.onClick.RemoveAllListeners();
    }

    public int GetIndex()
    {
        return index;
    }
    public UltEvent GetEvent()
    {
        return nextEvent;
    }
}
