using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class SpeakingArray : MonoBehaviour
{
    [Header("Speaking 배열")]
    [SerializeField] private Speaking[] array;
    [SerializeField] private UltEvent nextEvent;

    public Speaking GetSpeaking(int i)
    {
        return array[i];
    }

    public int GetArrayLength()
    {
        return array.Length;
    }

    public void InvokeNextEvent()
    {
        nextEvent.Invoke();
    }
}
