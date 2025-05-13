using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IngameCommandSkillManager : MonoBehaviour, IInputClick
{
    [SerializeField] private SelectedUnitManager SelectedUnitManager;
    //[SerializeField] private GameObject mouseIndicator;
    [SerializeField] private Transform BurningOilPos;
    private Unit selectedTargetUnit;
    //[SerializeField] private Button[] skillButtons;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputEventManager inputEventManager;

    [SerializeField] private Transform selectedUI0;
    [SerializeField] private Transform selectedUI1;
    [SerializeField] private GameObject circle;
    [SerializeField] private LayerMask groundLayer;
    private LayerMask targetUnitLayer;
    private bool isSkillActivated = false;

    private ActiveCommandSkill[] skill;
    private int activatedSkillButtonIdx = 0;

    void Update()
    {
        if (isSkillActivated)
        {
            if (inputEventManager.IsPointerOnUIElements())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                circle.transform.position = hit.point;
            }
        }
    }

    private void Awake()
    {
        if (GetComponentsInChildren<CommandSkill>() == null)
        {
            Debug.LogError("CommandSkillNullError");
            return;
        }
        skill = GetComponentsInChildren<ActiveCommandSkill>();
    }

    public void ActivateCommandSkill(ActiveCommandSkill skill, Transform pos)
    {
        switch (skill.Data.TargetType)
        {
            case CommandSkill.TargetType.NONE:
                Debug.Log("Skill Activated");
                skill.Activate();
                break;
            case CommandSkill.TargetType.UNIT:
                Debug.Log("UnitSkill Activated");
                skill.Activate(selectedTargetUnit);
                selectedTargetUnit = null;
                break;
            case CommandSkill.TargetType.MOUSEPOSAREA:
                inputEventManager.OnClickTarget = this;
                skill.Activate(pos);
                break;
            case CommandSkill.TargetType.AREA:
                Debug.Log("AreaSkill Activated");
                skill.Activate(BurningOilPos);
                break;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (skill[activatedSkillButtonIdx].Data.TargetType
                    == CommandSkill.TargetType.UNIT)
            {
                if (inputEventManager.IsPointerOnUIElements())
                    return;
                targetUnitLayer = skill[activatedSkillButtonIdx].AttackTargetLayer;
                if (Physics.Raycast(ray, out hit, float.MaxValue, targetUnitLayer))
                {
                    if (hit.collider.GetComponent<Unit>() != null)
                    {
                        selectedTargetUnit = hit.collider.GetComponent<Unit>();
                        ActivateCommandSkill(skill[activatedSkillButtonIdx], hit.transform);

                        inputEventManager.OnClickTarget = SelectedUnitManager;
                        selectedUI0.gameObject.SetActive(false);
                        selectedUI1.gameObject.SetActive(false);
                        return;
                    }
                }
            }
            else if (skill[activatedSkillButtonIdx].Data.TargetType
                == CommandSkill.TargetType.MOUSEPOSAREA)
            {
                if (Physics.Raycast(ray, out hit,float.MaxValue,groundLayer))
                {
                    if (inputEventManager.IsPointerOnUIElements())
                        return;
                    if (hit.collider.CompareTag(CONSTANT.TAG_TILE))
                    {
                        ActivateCommandSkill(skill[activatedSkillButtonIdx], hit.transform);

                        inputEventManager.OnClickTarget = SelectedUnitManager;
                        selectedUI0.gameObject.SetActive(false);
                        selectedUI1.gameObject.SetActive(false);
                        circle.SetActive(false);
                        isSkillActivated = false;
                    }
                }
            }

            /*if (Physics.Raycast(ray, out hit))
            //{
            //    if (inputEventManager.IsPointerOnUIElements())
            //        return;

            //    if (skill[activatedSkillButtonIdx].Data.TargetType
            //        == CommandSkill.TargetType.UNIT)
            //    {
            //        if (hit.collider.GetComponent<Unit>() != null)
            //        {
            //            selectedTargetUnit = hit.collider.GetComponent<Unit>();
            //            ActivateCommandSkill(skill[activatedSkillButtonIdx], hit.transform);
                        
            //            inputEventManager.OnClickTarget = SelectedUnitManager;
            //            selectedUI0.gameObject.SetActive(false);
            //            selectedUI1.gameObject.SetActive(false);
            //            return;
            //        }
            //    }

            //    if (hit.collider.CompareTag(CONSTANT.TAG_TILE))
            //    {
            //        ActivateCommandSkill(skill[activatedSkillButtonIdx], hit.transform);

            //        inputEventManager.OnClickTarget = SelectedUnitManager;
            //        selectedUI0.gameObject.SetActive(false);
            //        selectedUI1.gameObject.SetActive(false);
            //        circle.SetActive(false);
            //        isSkillActivated = false;
            //    }
            //}
            */

        }
    }

    public void GetClickControl(int idx)
    {
        activatedSkillButtonIdx = idx;
        if (!skill[activatedSkillButtonIdx].IsCoolDown)
        {
            Debug.Log(skill[activatedSkillButtonIdx].name + "이 쿨타임 중...)");
            return;
        }
        else
        {
            CommandSkillData skillData;
            skillData = skill[activatedSkillButtonIdx].Data;
            if (skillData.TargetType == CommandSkill.TargetType.MOUSEPOSAREA
                || skillData.TargetType == CommandSkill.TargetType.UNIT)
            {
                inputEventManager.OnClickTarget = this;
                if (idx == 0)
                {
                    circle.SetActive(false);
                    selectedUI0.gameObject.SetActive(true);
                    selectedUI1.gameObject.SetActive(false);
                    isSkillActivated = false;
                }
                else if (idx == 1)
                {
                    selectedUI0.gameObject.SetActive(false);
                    selectedUI1.gameObject.SetActive(true);
                    isSkillActivated = true;
                    circle.SetActive(true);
                }

            }
            else if (skillData.TargetType == CommandSkill.TargetType.AREA)
            {
                selectedUI0.gameObject.SetActive(false);
                selectedUI1.gameObject.SetActive(false);
                isSkillActivated = false;
                circle.SetActive(false);
                ActivateCommandSkill(skill[activatedSkillButtonIdx], BurningOilPos);
            }
            else
            {
                //ActivateCommandSkill(skill[activatedSkillButtonIdx], );
            }
            
        }
    }

    public void CancelSkill()
    {
        selectedUI0.gameObject.SetActive(false);
        selectedUI1.gameObject.SetActive(false);
    }
}
