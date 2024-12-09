using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiCursorManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MouseCursorManager cursorManager;

    void Start()
    {
        cursorManager = FindObjectOfType<MouseCursorManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cursorManager.SetUiCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursorManager.SetDefaultCursor();
    }
}
