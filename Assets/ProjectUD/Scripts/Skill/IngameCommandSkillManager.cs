using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IngameCommandSkillManager : MonoBehaviour, IInputClick
{
    [SerializeField] private SelectedUnitManager SelectedUnitManager;
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject BurningOilPos;
    private Unit selectedTargetUnit;
    [SerializeField] private Button[] skillButtons;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputEventManager inputEventManager;

    [SerializeField] private Transform selectedUI;



    private void Awake()
    {
        if (GetComponentsInChildren<CommandSkill>() == null)
        {
            Debug.LogError("CommandSkillNullError");
            return;
        }

        for (int i = 0; i < skillButtons.Length; i++)
        {
            int idx = i;
            skillButtons[idx].onClick.AddListener(() => 
            {
                Debug.Log("Button " + idx + " Clicked");
                if (GetComponentsInChildren<CommandSkill>() != null)
                {
                    ActiveCommandSkill[] skill = GetComponentsInChildren<ActiveCommandSkill>();

                    ActivateCommandSkill(skill[idx], this.transform);

                    //isCommandSkillActive = true;

                    //if (isCommandSkillActive && clickPos != null)
                    //{
                        
                    //}
                   
                }
            });
        }
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
                skill.Activate(BurningOilPos.transform);
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

                if (hit.collider.CompareTag("Tile"))
                {
                    ActiveCommandSkill[] skill = GetComponentsInChildren<ActiveCommandSkill>();

                    ActivateCommandSkill(skill[1], hit.transform);
                   
                    inputEventManager.OnClickTarget = SelectedUnitManager;
                    selectedUI.gameObject.SetActive(false);

                }    
            }
        }
    }

    public void ActivateCommandSkill()
    {
        ActiveCommandSkill[] skill = GetComponentsInChildren<ActiveCommandSkill>();
        ActivateCommandSkill(skill[2], this.transform);
    }


    public void GetClickControl()
    {
        inputEventManager.OnClickTarget = this;
        selectedUI.gameObject.SetActive(true);
    }

    public void CancleSkill()
    {
        selectedUI.gameObject.SetActive(false);
    }
}
