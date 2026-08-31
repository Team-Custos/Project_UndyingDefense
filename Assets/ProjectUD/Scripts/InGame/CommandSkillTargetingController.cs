using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static CommandSkill;

public class CommandSkillTargetingController : MonoBehaviour, IInputClick
{
    private ActiveCommandSkill currentSkill;
    [SerializeField] private GameObject circle;
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SelectedUnitManager SelectedUnitManager;   // 유닛 선택 스킬_집중포화스킬
    [SerializeField] private InGameManager ingameManager;

    [Header("스킬인디케이터")]
    [SerializeField] private GameObject indicator;

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
        if (ingameManager.IsGamgePause)
            return;

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
        //inGameManager.UpdateOperateState(OperateState.CS_Area);

        switch (skill.Data.TargetType)
        {
            case TargetType.AREA:
                circle.SetActive(false);
                break;
            case TargetType.MOUSEPOSAREA:
                inGameManager.UpdateOperateState(OperateState.CS_Area);
                circle.SetActive(true);
                break;
            case TargetType.UNIT:
                inGameManager.UpdateOperateState(OperateState.CS_Target);
                circle.SetActive(false);
                break;
        }
    }

    // 지휘관 스킬 취소
    public void CancleTargetSkill()
    {
        currentSkill.SetSkillState(false);
        currentSkill = null;
        indicator.SetActive(false);

    }
    public void CancleAreaSkill()
    {
        circle.SetActive(false);
        currentSkill.SetSkillState(false);
        currentSkill = null;
        indicator.SetActive(false);

    }

    private void RestoreInputTarget()
    {
        inputEventManager.OnClickTarget = inGameManager;
        inGameManager.UpdateOperateState(OperateState.DEFAULT);
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
                indicator.SetActive(false);
            }
        }
    }

    //public void OnESC(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        CancelTargeting();
    //        RestoreInputTarget();
    //    }
    //}

    //public void OnRightClick(InputAction.CallbackContext context)
    //{
    //    if (context.performed)
    //    {
    //        CancelTargeting();
    //        RestoreInputTarget();
    //    }
    //}


}
