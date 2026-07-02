using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillTooltipPointerEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CommandSkillBtnUI parentBtnUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        parentBtnUI.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        parentBtnUI.OnPointerExit(eventData);
    }
}
