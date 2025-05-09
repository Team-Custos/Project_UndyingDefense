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

    [SerializeField] private Transform selectedUI;

    private ActiveCommandSkill[] skill;

    private bool isTargetRequired = false;
    private int activatedSkillButtonIdx = 0;



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
        selectedTargetUnit = null;
        switch (skill.Data.TargetType)
        {
            case CommandSkill.TargetType.NONE:
                Debug.Log("Skill Activated");
                skill.Activate();
                break;
            case CommandSkill.TargetType.UNIT:
                Debug.Log("UnitSkill Activated");
                selectedTargetUnit = SelectedUnitManager.SelectedUnit;
                skill.Activate(selectedTargetUnit);
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

            if (Physics.Raycast(ray, out hit))
            {
                if (inputEventManager.IsPointerOnUIElements())
                    return;

                if (skill[activatedSkillButtonIdx].Data.TargetType
                    == CommandSkill.TargetType.UNIT)
                {
                    if (hit.collider.GetComponent<Unit>() != null)
                    {
                        ActivateCommandSkill(skill[activatedSkillButtonIdx], hit.transform);

                        inputEventManager.OnClickTarget = SelectedUnitManager;
                        selectedUI.gameObject.SetActive(false);
                        return;
                    }
                }

                if (hit.collider.CompareTag(CONSTANT.TAG_TILE))
                {
                    ActivateCommandSkill(skill[activatedSkillButtonIdx], hit.transform);

                    inputEventManager.OnClickTarget = SelectedUnitManager;
                    selectedUI.gameObject.SetActive(false);
                }
            }
        }
    }

    public void GetClickControl(int idx)
    {
        activatedSkillButtonIdx = idx;
        if (!skill[activatedSkillButtonIdx].IsCoolDown)
        {
            Debug.Log(skill[activatedSkillButtonIdx].name + "이 쿨타임 중...");
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
                selectedUI.gameObject.SetActive(true);
            }
            else if (skillData.TargetType == CommandSkill.TargetType.AREA)
            {
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
        isTargetRequired = false;
        selectedUI.gameObject.SetActive(false);
    }
}
