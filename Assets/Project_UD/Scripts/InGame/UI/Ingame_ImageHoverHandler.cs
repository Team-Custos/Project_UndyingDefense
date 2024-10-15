using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ingame_ImageHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject SkillInfoPanel;

    public void OnPointerExit(PointerEventData eventData)
    {
        SkillInfoPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SkillInfoPanel.SetActive(true);
    }
}
