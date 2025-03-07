using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ingame_ImageHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject skillInfoPanel;

    public void OnPointerExit(PointerEventData eventData)
    {
        skillInfoPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        skillInfoPanel.SetActive(true);
    }
}
