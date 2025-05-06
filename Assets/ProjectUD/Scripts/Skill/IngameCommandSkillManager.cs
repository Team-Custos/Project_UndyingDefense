using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameCommandSkillManager : MonoBehaviour
{
    [SerializeField] private SelectedUnitManager SelectedUnitManager;
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject BurningOilPos;
    private Unit selectedTargetUnit;
    [SerializeField] private Button[] skillButtons;

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
                    ActivateCommandSkill(skill[idx]);
                }
            });
        }
    }


    public void ActivateCommandSkill(ActiveCommandSkill skill)
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
                skill.Activate(mouseIndicator.transform);
                break;
            case CommandSkill.TargetType.AREA:
                Debug.Log("AreaSkill Activated");
                skill.Activate(BurningOilPos.transform);
                break;
        }
    }
}
