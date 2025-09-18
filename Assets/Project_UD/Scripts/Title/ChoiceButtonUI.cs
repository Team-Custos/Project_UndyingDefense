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

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();
    }

    public void SetButton(string buttonText, UltEvent buttonEvent)
    {
        text.text = buttonText;
        button.onClick.AddListener(() => buttonEvent.Invoke());
    }

    public void ResetButton()
    {
        text.text = "";
        button.onClick.RemoveAllListeners();
    }
}
