using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DollyCamera : MonoBehaviour
{
    [SerializeField] private CinemachineDollyCart dollyCart;
    [SerializeField] private GameObject virtualCamera;
    [SerializeField] private bool isCamPanning = true;
    public bool IsCamPanning => isCamPanning;

    void Start()
    {
        dollyCart.m_Position = 0f;
    }

    void Update()
    {
        if (isCamPanning)
        {
            if (dollyCart.m_Position >= dollyCart.m_Path.MaxPos)
            {
                isCamPanning = false;
                virtualCamera.SetActive(true);
            }
        }
    }
}
