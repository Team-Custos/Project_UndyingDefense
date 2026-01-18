using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static CommandSkill;

public class CommandSkillTargetingController : MonoBehaviour, IInputClick, IInputESC, IInputRightClick
{
    private ActiveCommandSkill currentSkill;
    [SerializeField] private GameObject circle;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SelectedUnitManager SelectedUnitManager;   // 유닛 선택 스킬_집중포화스킬
    [SerializeField] private InGameManager ingameManager;

    private Ray ray;
    private RaycastHit hit;

    void Update()
    {
        if (currentSkill == null)
            return;

        if (currentSkill.Data.TargetType == TargetType.MOUSEPOSAREA)
        {
            UpdateCirclePosition();
        }
    }
    private void UpdateCirclePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            circle.transform.position = hitInfo.point;
        }
    }

    public void BeginTargeting(ActiveCommandSkill skill)
    {
        currentSkill = skill;

        inputEventManager.OnClickTarget = this;
        inputEventManager.OnRightClickTarget = this;
        inputEventManager.OnESCTarget = this;

        switch (skill.Data.TargetType)
        {
            case TargetType.AREA:
            case TargetType.MOUSEPOSAREA:
                circle.SetActive(true);
                break;
        }
    }

    public void CancelTargeting()
    {
        circle.SetActive(false);
        currentSkill.SetSkillState(false);
        currentSkill = null;

        RestoreInputTarget();
    }

    private void RestoreInputTarget()
    {
        inputEventManager.OnClickTarget = SelectedUnitManager;
        inputEventManager.OnESCTarget = ingameManager;
        inputEventManager.OnRightClickTarget = SelectedUnitManager;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (inputEventManager.IsPointerOnUIElements())
                return;
            //if (!isSkillActivated)
            //    return;

            if (Physics.Raycast(ray, out hit, float.MaxValue, currentSkill.GetSelectTargetLayer()))
            {
                currentSkill.OnTargetSelected(hit);
                RestoreInputTarget();
                circle.SetActive(false);
            }
        }
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CancelTargeting();
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CancelTargeting();
        }
    }


}
