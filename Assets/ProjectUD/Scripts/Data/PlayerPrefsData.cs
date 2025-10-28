using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsData : MonoBehaviour
{
    //private string haveCommanderSkills = string.Empty;
    //private string selectCommanderSkills = string.Empty;    // 배열로 ..?
    private List<string> haveCommanderSkills = new List<string>();
    private List<string> selectCommanderSkills = new List<string>();
    
    public void SetHaveCommanderSkills(string id)
    {
        //haveCommanderSkills += $"{id}\n";

        string haveCSkill = string.Empty;
        haveCommanderSkills.Add(id);
        for (int i = 0; i < haveCommanderSkills.Count; i++)
        {
            haveCSkill += $"{haveCommanderSkills[i]}\n";
        }
        PlayerPrefs.SetString("haveCommaderSkill", haveCSkill);
    }

    public void SetSelectCSkill(string id)
    {
        //selectCommanderSkills += $"{id}\n";
        string selectCSkill = string.Empty;
        selectCommanderSkills.Add(id);
        for(int i = 0; i < selectCommanderSkills.Count; i++)
        {
            selectCSkill += $"{selectCommanderSkills[i]}\n";
        }
        PlayerPrefs.SetString("selectCommanderSkill", selectCSkill);
    }
}
