using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterButtonUI : MonoBehaviour
{
    private Image buttonImage;
    private TextMeshProUGUI buttonText;

    public void SetButton(Sprite image, string text)
    {
        buttonImage.sprite = image;
        buttonText.text = text;
    }

    public void ResetButton()
    {
        buttonText.text = "";
        buttonImage.sprite = null;
    }
}
