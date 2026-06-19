using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class DelayedEventInvoker : MonoBehaviour
{
    [SerializeField] private float delay;
    [SerializeField] private UltEvent onDelayedEvent;

    // 대화 -> 다음 이벤트(UltEvent)에서 이 함수를 호출
    public void TriggerDelayed()
    {
        StartCoroutine(DelayRoutine());
    }

    private IEnumerator DelayRoutine()
    {
        yield return new WaitForSeconds(delay);
        onDelayedEvent?.Invoke();
    }
}
