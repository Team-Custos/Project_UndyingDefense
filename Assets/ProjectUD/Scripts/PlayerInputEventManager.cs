using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using InputEventInterface;
using System.Collections;



public class PlayerInputEventManager : MonoBehaviour
{
    public IInputSubmit OnSubmitTarget { set; private get; }
    public IInputNavigate OnNavigateTarget { set; private get; }
    public IInputClick OnClickTarget { set; private get; }
    public IInputRightClick OnRightClickTarget { set; private get; }
    public IInputScrollWheel OnScrollTarget { set; private get; }
    public IInputSpeedUp OnSpeedUpTarget { set; private get; }
    public IInputUnitDelete OnUnitDeleteTarget { set; private get; }
    public IInputUnitSpawn OnUnitSpawnTarget { set; private get; }
    public IInputUnitUpgrade OnUnitUpgradeTarget { set; private get; }
    public IInputUnitModeChange OnUnitModeChangeTarget { set; private get; }
    public IInputPerformUnitUpgrade OnPerformUnitUpgradeTarget { set; private get; }
    public IInputESC OnESCTarget { set; private get; } 
    public IInputOnSpace OnSpaceTarget { set; private get; }
    public IInputUpArrow OnUpArrowTarget { set; private get; }
    public IInputDownArrow OnDownArrowTarget { set; private get; }

    [SerializeField] private SelectedUnitManager selectedUnitManager;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;
    [SerializeField] private CommandSkillTargetingController commandSkillTargetingController;

    private InputState leftClickState;


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


    // 좌클릭에 대한 권한을 가진 상태에 따라 이벤트를 전달
    public void OnClick(InputAction.CallbackContext context)
    {
        if (OnClickTarget != null)
            OnClickTarget.OnClick(context);

        //Debug.Log(OnClickTarget);

        //switch(leftClickState)
        //{
        //    case LeftClickState.UNIT_CONTROL:
        //        selectedUnitManager.OnClick(context);
        //        break;

        //    case LeftClickState.UNIT_SPAWN:
        //        allyUnitSpawner.OnClick(context);
        //        break;

        //    case LeftClickState.COMMAND_SKILL:
        //        commandSkillTargetingController.OnClick(context);
        //        break;
        //}
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (OnRightClickTarget != null)
            OnRightClickTarget.OnRightClick(context);
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        if (OnScrollTarget != null)
            OnScrollTarget.OnScrollWheel(context);
    }

    public void OnSpeedUp(InputAction.CallbackContext context)
    {
        if (OnSpeedUpTarget != null)
            OnSpeedUpTarget.OnSpeedUp(context);
    }


    public void OnUnitDelete(InputAction.CallbackContext context)
    {
        if (OnUnitDeleteTarget != null)
            OnUnitDeleteTarget.OnUnitDelete(context);
    }

    public void OnUnitSpawn(InputAction.CallbackContext context)
    {
        if (OnUnitSpawnTarget != null)
            OnUnitSpawnTarget.OnUnitSpawn(context);
    }

    public void OnUnitUpgrade(InputAction.CallbackContext context)
    {
        if (OnUnitUpgradeTarget != null)
            OnUnitUpgradeTarget.OnUnitUpgrade(context);
    }

    public void OnUnitModeChange(InputAction.CallbackContext context)
    {
        if (OnUnitModeChangeTarget != null)
            OnUnitModeChangeTarget.OnUnitModeChange(context);
    }

    public void OnPerformUnitUpgrade(InputAction.CallbackContext context)
    {
        if (OnPerformUnitUpgradeTarget != null)
            OnPerformUnitUpgradeTarget.OnPerformUnitUpgrade(context);
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (OnESCTarget != null)
            OnESCTarget.OnESC(context);
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if (OnSpaceTarget != null)
            OnSpaceTarget.OnSpace(context);
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

    public void OnUpArrow(InputAction.CallbackContext context)
    {
        if (OnUpArrowTarget != null)
            OnUpArrowTarget.OnUpArrow(context);
    }

    public void OnDownArrow(InputAction.CallbackContext context)
    {
        if(OnDownArrowTarget != null)
            OnDownArrowTarget.OnDownArrow(context);
    }
}
