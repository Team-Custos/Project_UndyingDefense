using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPrefsData : MonoBehaviour
{// 싱글톤?=> ㅇㅇ
    //private string haveCommanderSkills = string.Empty;
    //private string selectCommanderSkills = string.Empty;    // 배열로 ..?

    //** 지휘관 스킬
    private List<string> haveCommanderSkills = new List<string>();
    private List<string> selectCommanderSkills = new List<string>();

    private string[] skills = new string[3];

    //** 인물도감
    private List<string> characterArchive = new List<string>();

    //** 계정 설정 => 닉네임만 바꿀때, 이미지만 바꿀때는?

    // 여기서 하고싶은데
    // 계정 구조체 만들기

    public struct AAA
    {

    }
    
    public void SetAccount(string pID, string pNickName, string imageID)    // 구조체
    {
        PlayerPrefs.SetString("PlayerAccount", $"{pID},{pNickName},{imageID}");
    }

    // 설정을 가져오는 메서드도 작성해야하나?


    //** 지휘관 계급
    public void SetCommanderRank(string commanderID)
    {
        PlayerPrefs.SetString("CommanderRank", commanderID);
    }

    //** 공로포인트
    public void SetPoint(float point)
    {
        PlayerPrefs.SetFloat("Point", point);
    }
    
    public void SetCharacterArchive(string cArchiveID)
    {
        if (characterArchive.Contains(cArchiveID))
            return;
        characterArchive.Add(cArchiveID);

        string cArchive = string.Empty;
        for(int i = 0; i < characterArchive.Count; i++)
        {
            cArchive += $"{characterArchive[i]}\n";
        }
        PlayerPrefs.SetString("CArchive", cArchive);
    }
    
    public void SetHaveCommanderSkills(string skillID)
    {
        //haveCommanderSkills += $"{id}\n";

        if(haveCommanderSkills.Contains(skillID)) 
            return;

        haveCommanderSkills.Add(skillID);
        string haveCSkill = string.Empty;
        for (int i = 0; i < haveCommanderSkills.Count; i++)
        {
            haveCSkill += $"{haveCommanderSkills[i]}\n";
        }
        PlayerPrefs.SetString("haveCommaderSkill", haveCSkill);
    }

    public void SetSelectCSkill(string id)  // ?? -> 언제저장 물어볼것
    {
        //selectCommanderSkills += $"{id}\n";
        //selectCommanderSkills.Add(id);
        // 지휘관 스킬 선택하는 부분 보고 수정 

        string selectCSkill = string.Empty;
        for(int i = 0; i < selectCommanderSkills.Count; i++)
        {
            selectCSkill += $"{selectCommanderSkills[i]}\n";
        }
        PlayerPrefs.SetString("selectCommanderSkill", selectCSkill);
    }

    // 게임을 켰을 때 인물도감 가져오는 메서드 => 재활용방법?
    public List<string> GetCArchiveList()
    {
        string cArchive = PlayerPrefs.GetString("CArchive");

        if (!string.IsNullOrEmpty(cArchive))
        {
            string[] cArchiveID = cArchive.Split('\n');
            for(int i = 0; i < cArchiveID.Length; i++)
            {
                characterArchive.Add(cArchiveID[i]);
            }
        }

        return characterArchive;
    }

    public List<string> GetCommanderSkill()
    {
        string commanderSkill = PlayerPrefs.GetString("haveCommaderSkill");
        if (!string.IsNullOrEmpty(commanderSkill))
        {
            string[] skills = commanderSkill.Split("\n");
            for(int i = 0; i < skills.Length; i++)
            {
                haveCommanderSkills.Add(skills[i]);
            }
        }

        return haveCommanderSkills;
    }

    // 선택한 스킬도 불러올지=> 기획
}
