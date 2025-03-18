using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using InputEventInterface;

public class PlayerInputEventManager : MonoBehaviour
{
    public IInputSubmit OnSubmitTarget { set; private get; }
    public IInputNavigate OnNavigateTarget { set; private get; }
    public IInputClick OnClickTarget { set; private get; }
    public IInputScrollWheel OnScrollTarget { set; private get; }

    [SerializeField] private GraphicRaycaster graphicRaycaster;

    private PointerEventData pointerEventData;
    private List<RaycastResult> pointerRaycastResults = new List<RaycastResult>();

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (OnSubmitTarget != null)
            OnSubmitTarget.OnSubmit(context);
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (OnNavigateTarget != null)
            OnNavigateTarget.OnNavigate(context);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (OnClickTarget != null)
            OnClickTarget.OnClick(context);
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        if(OnScrollTarget != null)
            OnScrollTarget.OnScrollWheel(context);
    }

    public bool IsPointerOnUIElements()
    {
        if (pointerEventData == null)
            pointerEventData = new PointerEventData(EventSystem.current);

        pointerEventData.position = Mouse.current.position.value;
        pointerRaycastResults.Clear();

        graphicRaycaster.Raycast(pointerEventData, pointerRaycastResults);

        return pointerRaycastResults.Count > 0;
    }
}
