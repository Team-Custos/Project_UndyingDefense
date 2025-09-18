using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

[System.Serializable]   //MonoBehaviour 를 안쓰는 대신에 유니티상에서 보여주기 위한 직렬화
public class Choice
{
    [SerializeField] private string choiceID;
    [SerializeField] private UltEvent nextEvent;

    public string GetChoiceID()
    {
        return choiceID;
    }

    public void InvokeNextEvent()
    {
        nextEvent.Invoke();
    }

    public UltEvent NextEvent()
    {
        return nextEvent;
    }
}
