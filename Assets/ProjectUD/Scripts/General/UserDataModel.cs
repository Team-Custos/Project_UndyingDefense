using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDataModel : MonoBehaviour
{
    public static UserDataModel instance;

    [SerializeField] private bool isTutorialEnd = false;
    [SerializeField] private bool isGameFinshed = false;

    public bool IsTutorialEnd => isTutorialEnd;
    public bool IsGameFinshed => isGameFinshed;

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


    public void SetTutorialEnd(bool value)
    {
        isTutorialEnd = value;
    }
    public void SetGameFinished(bool value)
    {
        isGameFinshed = value;
    }
}
