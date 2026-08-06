using Cinemachine;
using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


//카메라 조작을 위한 스크립트
public class Ingame_CamManager : MonoBehaviour, IInputNavigate, IInputScrollWheel, IInputOnSpace
{
    [Header("■ Components")]
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private SelectedUnitManager    selectedUnitManager;
    [SerializeField] private DollyCamera dollyCamera;
    [SerializeField] private Transform cameraPivot;
    [SerializeField] private Transform virtualCamPos;
    private CinemachineFramingTransposer framingTransposer;

    [Header("■ Cam Controll")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 2.0f;
    [SerializeField] private float zoomMin = 5.0f;
    [SerializeField] private float zoomMax = 20.0f;
    [SerializeField] private float maxRotationX = 60f;
    [SerializeField] private float minRotationX = 30f;
    private float targetRotationX;
    private float rotationVelocityX;

    [Header("■ Cam Limtis")]
    [SerializeField] private float xMax = 30.0f;
    [SerializeField] private float xMin = -40.0f;
    [SerializeField] private float zMax = 4.5f;
    [SerializeField] private float zMin = -25.0f;
    
    private Transform startTranfrom; // 카메라 시작 위치

    private Vector3 moveDirection = Vector3.zero; // 이동 방향 저장

    public float ZoomMax => zoomMax;
    public float ZoomMin => zoomMin;

    private void Start()
    {
        if (virtualCamera != null)
        {
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        // startTranfrom 초기화
        startTranfrom = new GameObject("StartPosition").transform;
        startTranfrom.position = cameraPivot.position; // 카메라 시작 위치 저장

        inputEventManager.OnNavigateTarget = this;
        inputEventManager.OnScrollTarget = this;
        inputEventManager.OnSpaceTarget = this;

        targetRotationX = virtualCamPos.eulerAngles.x;
    }

    private void Update()
    {
        if (moveDirection != Vector3.zero && !dollyCamera.IsCamPanning)
        {
            Vector3 movement = moveDirection.normalized * moveSpeed * Time.deltaTime;
            cameraPivot.position += movement;

            // 카메라 이동 범위 제한
            Vector3 clampedPosition = cameraPivot.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, xMin, xMax);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, zMin, zMax);
            cameraPivot.position = clampedPosition;
        }

        if (!dollyCamera.IsCamPanning)
        {
            float currentRotationX = virtualCamPos.eulerAngles.x;

            if (float.IsNaN(currentRotationX)) return;
            if (float.IsNaN(targetRotationX)) return;

            float dampedRotationX = Mathf.SmoothDampAngle(
                currentRotationX,
                targetRotationX,
                ref rotationVelocityX,
                0.2f
            );

            if (float.IsNaN(dampedRotationX))
            {
                rotationVelocityX = 0f;
                return;
            }

            Vector3 newEuler = new Vector3(
                dampedRotationX,
                virtualCamPos.eulerAngles.y,
                virtualCamPos.eulerAngles.z
            );

            if (float.IsNaN(newEuler.y) || float.IsNaN(newEuler.z)) return;

            virtualCamPos.eulerAngles = newEuler;
        }
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            if (dollyCamera.IsCamPanning || !inGameManager.IsGameStart)
                return;


            Vector2 input = context.ReadValue<Vector2>();


            Vector3 forward = virtualCamPos.forward;
            Vector3 right = virtualCamPos.right;


            // 높이 방향 제거
            forward.y = 0;
            right.y = 0;


            forward.Normalize();
            right.Normalize();


            moveDirection = forward * input.y + right * input.x;
        }
        else if (context.canceled)
        {
            moveDirection = Vector3.zero;
        }

        //if (context.started || context.performed)
        //{
        //    if (dollyCamera.IsCamPanning || !inGameManager.IsGameStart)
        //        return;

        //    Vector2 input = context.ReadValue<Vector2>();
        //    moveDirection = Vector3.zero;


        //    if (input.y > 0f) // W 키
        //        moveDirection += new Vector3(1f, 0f, 1f);
        //    if (input.y < 0f) // S 키
        //        moveDirection += new Vector3(-1f, 0f, -1f);
        //    if (input.x > 0f) // D 키
        //        moveDirection += new Vector3(1f, 0f, -1f);
        //    if (input.x < 0f) // A 키
        //        moveDirection += new Vector3(-1f, 0f, 1f);
        //}
        //else if (context.canceled)
        //{
        //    moveDirection = Vector3.zero; // 키를 떼면 멈추기
        //}
    }


    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (dollyCamera.IsCamPanning || !inGameManager.IsGameStart)
                return;

            float scrollInput = context.ReadValue<Vector2>().y;

            float currentFov = framingTransposer.m_CameraDistance;
            currentFov -= scrollInput * zoomSpeed * Time.deltaTime;
            currentFov = Mathf.Clamp(currentFov, zoomMin, zoomMax);
            framingTransposer.m_CameraDistance = currentFov;

            float ratio = (currentFov - zoomMin) / (zoomMax - zoomMin);
            targetRotationX = Mathf.Lerp(minRotationX, maxRotationX, ratio);

        }
    }

    public void FocusSelectedUnit(Vector3 targetPosition)
    {
        Vector3 newPosition = new Vector3(targetPosition.x, cameraPivot.position.y, targetPosition.z);
        cameraPivot.position = newPosition;
    }

    public void OnSpace(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (dollyCamera.IsCamPanning || !inGameManager.IsGameStart)
                return;

            if (selectedUnitManager.SelectedUnit != null)
                FocusSelectedUnit(selectedUnitManager.SelectedUnit.transform.position);
            else
            {
                FocusSelectedUnit(startTranfrom.position);
            }

        }
    }
}