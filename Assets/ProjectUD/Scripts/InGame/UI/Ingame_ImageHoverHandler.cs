using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using static CommandSkillManager;

public class Ingame_ImageHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject mouseOverObject;


    public virtual void OnPointerExit(PointerEventData eventData)
    {
        mouseOverObject.SetActive(false);

    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        mouseOverObject.SetActive(true);
    }

    // 강제로 호버 상태를 해제할 때 사용
    public void ForceExit()
    {
        mouseOverObject.SetActive(false);
    }
}
