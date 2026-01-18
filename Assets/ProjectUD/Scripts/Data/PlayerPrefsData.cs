using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerPrefsData : MonoBehaviour
{// 싱글톤?=> ㅇㅇ
    //private string haveCommanderSkills = string.Empty;
    //private string selectCommanderSkills = string.Empty;    // 배열로 ..?
    public static PlayerPrefsData instance;

    [SerializeField] private TextAsset CSkillDefaultData;
    [SerializeField] private TextAsset HaveCSkillDefaultData;

    //** 지휘관 스킬
    private List<string> haveCommanderSkills = new List<string>();
    private List<string> selectCommanderSkills = new List<string>();

    private string[] skills = new string[3];

    //** 인물도감
    private List<string> characterArchive = new List<string>();

    private StringBuilder sb = new StringBuilder();

    //** 계정 설정 => 닉네임만 바꿀때, 이미지만 바꿀때는?

    // 여기서 하고싶은데
    // 계정 구조체 만들기

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        //-- 임시
        //PlayerPrefs.SetInt("SetDefaultCSkill", 0);
        if (PlayerPrefs.GetInt("SetDefaultCSkill") == 0)
        {
            LoadCSkillData(CSkillDefaultData.text);
        }
        if (PlayerPrefs.GetInt("SetHaveCSkill") == 0)
        {
            LoadHaveCSkillData(HaveCSkillDefaultData.text);
        }
    }

    // 초기 지휘관 스킬 저장 ( 선택 )
    public void LoadCSkillData(string st)
    {
        if (sb.Length > 0)
            sb.Clear();

        string[] lines = st.Split('\n');

        for (int i = 1; i < lines.Length; i++)  // 맨 윗줄 빼고라서 1부터.
        {
            if (lines[i] == string.Empty)
                continue;
            //sb.AppendLine(lines[i]);

            sb.Append($"{lines[i]}\n");
            // (3개들어가는거 확인) Debug.Log($"sb에 {lines[i]} 추가");
        }

        //PlayerPrefs.SetString("haveCommaderSkill", sb.ToString());
        PlayerPrefs.SetString("selectCommanderSkill", sb.ToString());
        PlayerPrefs.SetInt("SetDefaultCSkill", 1);
    }

    // 초기 지휘관 스킬 저장 ( 보유 )
    public void LoadHaveCSkillData(string st)
    {
        if (sb.Length > 0)
            sb.Clear();

        string[] lines = st.Split('\n');

        for (int i = 1; i < lines.Length; i++)  // 맨 윗줄 빼고라서 1부터.
        {
            if (lines[i] == string.Empty)
                continue;
            //sb.AppendLine(lines[i]);

            sb.Append($"{lines[i]}\n");
            // (3개들어가는거 확인) Debug.Log($"sb에 {lines[i]} 추가");
        }

        PlayerPrefs.SetString("haveCommaderSkill", sb.ToString());
        PlayerPrefs.SetInt("SetHaveCSkill", 1);
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
    public void SetPoint(float getPoint)
    {
        float point = PlayerPrefs.GetFloat("Point");
        point += getPoint;
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

    public void SetSelectCSkill(List<string> selectCSkillList)  
    {
        //selectCommanderSkills += $"{id}\n";
        //selectCommanderSkills.Add(id);
        // 지휘관 스킬 선택하는 부분 보고 수정 

        selectCommanderSkills.Clear();
        // selectCommanderSkills = selectCSkillList;
        for(int i = 0; i < selectCSkillList.Count; i++)
        {
            selectCommanderSkills.Add(selectCSkillList[i]);
        }

        string selectCSkill = string.Empty;

        for (int i = 0; i < selectCommanderSkills.Count; i++)
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

    public List<string> GetHaveCommanderSkill()
    {
        haveCommanderSkills.Clear();
        string commanderSkill = PlayerPrefs.GetString("haveCommaderSkill");
        if (!string.IsNullOrEmpty(commanderSkill))
        {
            string[] skills = commanderSkill.Split("\n");
            for(int i = 0; i < skills.Length; i++)
            {
                haveCommanderSkills.Add(skills[i].Trim());
            }
        }

        return haveCommanderSkills;
    }

    // 선택한 스킬도 불러올지=> 기획
    public List<string> GetSelectedCommanderSkill()
    {
        selectCommanderSkills.Clear();
        string commanderSkill = PlayerPrefs.GetString("selectCommanderSkill");
        Debug.Log($"{commanderSkill}");
        Debug.Log($"{commanderSkill.Length}");

        if (!string.IsNullOrEmpty(commanderSkill))
        {
            string[] skills = commanderSkill.Split("\n");
            for (int i = 0; i < skills.Length; i++)
            {
                if (skills[i] == string.Empty)
                    continue;
                Debug.Log($"{skills[i]}, Length: { skills[i].Length}, { skills[i].Trim().Length} ");
                selectCommanderSkills.Add(skills[i].Trim());
            }
            Debug.Log($"프랩스에서 GetSelected 한 지휘관 스킬 갯수 : {selectCommanderSkills.Count}");
        }

        return selectCommanderSkills;
    }
}
