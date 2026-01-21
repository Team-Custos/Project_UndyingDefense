using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescriptionPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillName;
    [SerializeField] private TextMeshProUGUI skillDesc;
    [SerializeField] private TextMeshProUGUI skillEffect;


    public void HidePanel()
    {
        gameObject.SetActive(false);
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    public void SetPanel(string name, string desc, string effect)
    {
        skillName.text = name;
        skillDesc.text = desc;
        skillEffect.text = effect;
    }
}
