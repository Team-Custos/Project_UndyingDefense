using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InGame_VirtualCamManager : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Transform cameraPivot;
    public Vector3 UnitToLookAt_Offset;

    public bool isCameraMoving = false;


    public float moveSpeed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float zoomSpeed = 2.0f;
    public float zoomMin = 5.0f;
    public float zoomMax = 20.0f;

    public float xMax = 30.0f;
    public float xMin = -40.0f;
    public float zMax = 4.5f;
    public float zMin = -25.0f;

    [SerializeField] private float zoomLerpSpeed = 10f; // 부드러움 조절
    private float targetFov; // 목표 FOV 저장

    // Start is called before the first frame update
    void Start()
    {
        targetFov = virtualCamera.m_Lens.FieldOfView; // 초기 FOV 저장
    }

    // Update is called once per frame
    void Update()
    {
        CameraMove();
        CameraZoom();
    }

    void CameraMove()
    {
        // 카메라 이동
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            // W 키를 누르면 Z 증가, X 증가
            moveDirection += new Vector3(1, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            // S 키를 누르면 Z 감소, X 감소
            moveDirection += new Vector3(-1, 0, -1);
        }
        if (Input.GetKey(KeyCode.D))
        {
            // D 키를 누르면 X 증가, Z 감소
            moveDirection += new Vector3(1, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            // A 키를 누르면 X 감소, Z 증가
            moveDirection += new Vector3(-1, 0, 1);
        }

        // 이동 벡터를 정규화하여 일정한 속도로 이동
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();
        }


        // 카메라 피벗 이동
        cameraPivot.position += moveDirection * moveSpeed * Time.deltaTime;

        // 이동 범위 제한
        float clampedX = Mathf.Clamp(cameraPivot.position.x, xMin, xMax);
        float clampedZ = Mathf.Clamp(cameraPivot.position.z, zMin, zMax);
        cameraPivot.position = new Vector3(clampedX, cameraPivot.position.y, clampedZ);


        // 카메라 회전 -> 카메라 회전 기능 삭제
        //Vector3 currentRotation = virtualCamera.transform.eulerAngles;

        //if (Input.GetKey(KeyCode.Q))
        //{
        //    currentRotation.y -= rotationSpeed * Time.deltaTime;
        //}

        //// E 키로 Y축 회전 증가
        //if (Input.GetKey(KeyCode.E))
        //{
        //    currentRotation.y += rotationSpeed * Time.deltaTime;
        //}

        //virtualCamera.transform.eulerAngles = new Vector3(45, currentRotation.y, 0);
    }

    void CameraZoom()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0)
        {
            targetFov -= scrollInput * zoomSpeed * 100f * Time.deltaTime;
            targetFov = Mathf.Clamp(targetFov, zoomMin, zoomMax);
        }

        // 현재 FOV와 목표 FOV 사이를 Lerp로 보간
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(
            virtualCamera.m_Lens.FieldOfView,
            targetFov,
            Time.deltaTime * zoomLerpSpeed
        );
    }
}