using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameCommandSkillManager : MonoBehaviour
{
    [SerializeField] private SelectedUnitManager SelectedUnitManager;
    [SerializeField] private GameObject mouseIndicator;
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
                    CommandSkill[] skill = GetComponentsInChildren<CommandSkill>();
                    if (skill[idx].Data.CommandType == CommandSkill.CommandSkillType.ACTIVE)
                    {
                        ActivateCommandSkill(skill[idx]);
                    }
                }
            });
        }
    }


    public void ActivateCommandSkill(CommandSkill skill)
    {
        selectedTargetUnit = null;
        switch (skill.Data.TargetType)
        {
            case CommandSkill.TargetType.NONE:
                skill.Activate();
                break;
            case CommandSkill.TargetType.UNIT:
                selectedTargetUnit = SelectedUnitManager.SelectedUnit;
                skill.Activate(selectedTargetUnit);
                break;
            case CommandSkill.TargetType.AREA:
                skill.Activate(mouseIndicator.transform);
                break;
        }
    }


}
