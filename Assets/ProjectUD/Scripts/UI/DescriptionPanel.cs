using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescriptionPanel : ToolTipUI
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDesc;
    [SerializeField] private TextMeshProUGUI skillEffect;
    [SerializeField] private TextMeshProUGUI skillCoolTime;


    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    public void SetPanel(string name, string desc, string effect, string coolTime)
    {
        skillName.text = name;
        skillDesc.text = desc;
        skillEffect.text = effect;
        skillCoolTime.text = coolTime;
    }
}
