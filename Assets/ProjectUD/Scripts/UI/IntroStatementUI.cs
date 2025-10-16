using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class IntroStatementUI : MonoBehaviour
{
    [SerializeField] private UltEvent nextEvent;

    public void OnStatementFinished()
    {
        nextEvent.Invoke();
    }
}
