using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyCameraMove : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart dollyCart;
    [SerializeField] private float duration = 5f;

    [SerializeField] private float timer = 0f;

    void Start()
    {
        //dollyCart.m_Position = 0f;
    }

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / duration);
            dollyCart.m_Position = t;
        }
        else
        {
            dollyCart.m_Speed = 0f;
        }
    }
}
