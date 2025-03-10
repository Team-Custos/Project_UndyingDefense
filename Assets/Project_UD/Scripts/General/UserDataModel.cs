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
}
