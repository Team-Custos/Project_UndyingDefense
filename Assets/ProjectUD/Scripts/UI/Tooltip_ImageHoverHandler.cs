using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tooltip_ImageHoverHandler : Ingame_ImageHoverHandler
{
    [SerializeField] private ToolTipUI toolTipUI;
    public override void OnPointerExit(PointerEventData eventData)
    {
        mouseOverObject.SetActive(false);

    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        mouseOverObject.SetActive(true);
        toolTipUI.SetPanelHeight();
    }
}
