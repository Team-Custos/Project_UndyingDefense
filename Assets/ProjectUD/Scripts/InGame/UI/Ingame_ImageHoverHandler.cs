using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

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
}
