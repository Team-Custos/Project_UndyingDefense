using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Data", menuName = "Character Data / ScriptableObject")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite characterSprite;
}
