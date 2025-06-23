using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ingame_ImageHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject mouseOverObject;

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseOverObject.SetActive(false);

        // 여기서 설정
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOverObject.SetActive(true);
    }
}
