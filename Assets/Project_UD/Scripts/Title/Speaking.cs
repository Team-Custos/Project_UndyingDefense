using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speaking : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    [SerializeField] private string id;

    public string GetSpeakingID()
    {
        return id;
    }
    public CharacterData GetCharacterData()
    {
        return characterData;
    }
}
