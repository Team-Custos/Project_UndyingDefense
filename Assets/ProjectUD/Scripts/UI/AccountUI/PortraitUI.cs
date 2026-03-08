using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortraitUI : MonoBehaviour
{
    [SerializeField] private Image portrait;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Button portraitButton;
    private PortraitData portraitData;
    private bool isOpen = false;

    public void SetPortraitData(PortraitData data, int playerRank)
    {
        portraitData = data;
        isOpen = playerRank >= portraitData.openRank;
        portrait.sprite = portraitData.portrait;
        lockIcon.gameObject.SetActive(!isOpen);
        portraitButton.interactable = isOpen;
    }

    public void ResetPortraitUI()
    {
        portrait.sprite = null;
        lockIcon.gameObject.SetActive(false);
        portraitData = null;
        isOpen = false;
    }

    public PortraitData GetPortraitData()
    {
        return portraitData;
    }
}
