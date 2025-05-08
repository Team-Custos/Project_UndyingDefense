using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class UIOnOff : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private float delay = 3.0f;
    private void OnEnable()
    {
        Invoke("OffUI", delay);
    }

    private void OffUI()
    {
        animator.SetTrigger("FadeOutTrigger");
    }

    private void OnDisable()
    {
        // UI가 비활성화되면 OffUI 호출 예약 취소
        CancelInvoke("OffUI");
    }


}
