using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataModel : MonoBehaviour
{
    public static UserDataModel instance;

    public List<CommandSkillData> skillDatas = new List<CommandSkillData>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 리스트가 모두 찼는지 확인하는 메서드
    public bool IsSkillListFull()
    {
        // 리스트의 크기가 3이고, 모든 요소가 null이 아닌 경우
        return skillDatas.Count == 3 && !skillDatas.Contains(null);
    }
}
