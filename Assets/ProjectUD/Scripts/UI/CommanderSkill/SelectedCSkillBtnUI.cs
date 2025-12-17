using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedCSkillBtnUI : MonoBehaviour
{
    [SerializeField] private Image skillIconImage;

    public void SetSelectedCSkillUI(CommandSkillData data)
    {
        if(data != null)
        {
            skillIconImage.sprite = data.Icon;
            //skillIconImage.color = Color.white;
        }
        else
        {
            skillIconImage.sprite = null;
            //skillIconImage.color = new Color(1, 1, 1, 0);
        }
    }
    public void SetSelectedCSkillUI()
    {

    }
}
