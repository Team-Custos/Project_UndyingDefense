using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TipTextData", menuName = "ProjectUD/TipTextData")]
public class TipTextData : ScriptableObject
{
    [SerializeField, TextArea] private string tipText;
    public string TipText => tipText;
}
