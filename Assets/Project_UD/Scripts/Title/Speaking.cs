using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaking : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    [SerializeField] private string id;
    //-- 로컬테이블string
    [SerializeField] private string tableName;

    public string GetSpeakingID()
    {
        return id;
    }

    public string GetTableName()
    {
        return tableName;
    }
    public CharacterData GetCharacterData()
    {
        return characterData;
    }
}
