using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataModel : MonoBehaviour
{
    public static UserDataModel instance;

    public List<string> skillIDs = new List<string>();

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
