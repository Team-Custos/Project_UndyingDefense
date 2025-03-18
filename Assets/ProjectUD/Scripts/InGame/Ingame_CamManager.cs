using Cinemachine;
using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


//카메라 조작을 위한 스크립트. (삭제 예정.)
public class Ingame_CamManager : MonoBehaviour, IInputNavigate, IInputScrollWheel
{
    [SerializeField] private PlayerInputEventManager inputEventManager;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Transform cameraPivot;

    [Header("■ Cam Controll")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 2.0f;
    [SerializeField] private float zoomMin = 5.0f;
    [SerializeField] private float zoomMax = 20.0f;

    [Header("■ Cam Limtis")]
    [SerializeField] private float xMax = 30.0f;
    [SerializeField] private float xMin = -40.0f;
    [SerializeField] private float zMax = 4.5f;
    [SerializeField] private float zMin = -25.0f;

    private Vector3 moveDirection = Vector3.zero; // 이동 방향 저장

    private void Start()
    {
        inputEventManager.OnNavigateTarget = this;
        inputEventManager.OnScrollTarget = this;
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (context.started || context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();
            moveDirection = Vector3.zero;

            if (input.y > 0) // W 키
                moveDirection += new Vector3(1, 0, 1);
            if (input.y < 0) // S 키
                moveDirection += new Vector3(-1, 0, -1);
            if (input.x > 0) // D 키
                moveDirection += new Vector3(1, 0, -1);
            if (input.x < 0) // A 키
                moveDirection += new Vector3(-1, 0, 1);
        }
        else if (context.canceled)
        {
            moveDirection = Vector3.zero; // 키를 떼면 멈추기
        }
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float scrollInput = context.ReadValue<Vector2>().y;

            float currentFov = virtualCamera.m_Lens.FieldOfView;
            currentFov -= scrollInput * zoomSpeed * Time.deltaTime;
            currentFov = Mathf.Clamp(currentFov, zoomMin, zoomMax);
            virtualCamera.m_Lens.FieldOfView = currentFov;

        }
    }


    private void Update()
    {
        if (moveDirection != Vector3.zero)
        {
            Vector3 movement = moveDirection.normalized * moveSpeed * Time.deltaTime;
            cameraPivot.position += movement;

            // 카메라 이동 범위 제한
            Vector3 clampedPosition = cameraPivot.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, xMin, xMax);
            clampedPosition.z = Mathf.Clamp(clampedPosition.z, zMin, zMax);
            cameraPivot.position = clampedPosition;
        }
    }

    //public float camZoomValue = 0.5f;

    //public float moveSpeed = 2;

    //private bool _userMoveInput; // 현재 조작을 하고있는지 확인을 위한 변수
    //private Vector3 _startPosition;  // 입력 시작 위치를 기억
    //private Vector3 _directionForce; // 조작을 멈췄을때 서서히 감속하면서 이동 시키기 

    //// Update is called once per frame
    //void Update()
    //{
    //    MouseMove();
    //    KeyboardMove();
    //    ZoomCamera();
    //}

    //private void MoveCamera(float xInput, float zInput)
    //{
    //    float zMove = Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * zInput - Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180) * xInput;
    //    float xMove = Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180) * zInput + Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * xInput;

    //    transform.position = transform.position + new Vector3(xMove, 0, zMove);
    //}

    //void ZoomCamera()
    //{
    //    //if (inputSystem.IsWheelScrollUp)
    //    //{
    //    //    transform.position = transform.position + new Vector3(0, camZoomValue, 0);
    //    //}
    //    //else if (inputSystem.IsWheelScrollDown)
    //    //{
    //    //    transform.position = transform.position - new Vector3(0, camZoomValue, 0);
    //    //}
    //}



    //// Get mouse drag inputs
    //void MouseMove()
    //{
    //    var mouseWorldPosition = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

    //    if (inputSystem.IsPressedSecondaryButton)
    //    {
    //        _userMoveInput = true;
    //        _startPosition = mouseWorldPosition;
    //        _directionForce = Vector2.zero;
    //    }

    //    else if (inputSystem.IsPressingSecondaryButton)
    //    {
    //        if (!_userMoveInput)
    //        {
    //            _userMoveInput = true;
    //            _startPosition = mouseWorldPosition;
    //            _directionForce = Vector2.zero;
    //            return;
    //        }

    //        _directionForce = _startPosition - mouseWorldPosition;
    //    }
    //    else
    //    {
    //        _userMoveInput = false;
    //    }
    //}
    //private void UpdateCameraPosition()
    //{
    //    // 이동 수치가 없으면 아무것도 안함
    //    if (_directionForce == Vector3.zero)
    //    {
    //        return;
    //    }

    //    var currentPosition = transform.position;
    //    var targetPosition = currentPosition + _directionForce;
    //    transform.position = Vector3.Lerp(currentPosition, targetPosition, 0.5f);
    //}

    //void KeyboardMove()
    //{
    //    //transform.position = new Vector3(camPosition_X, camPosition_Y, camPosition_Z);

    //    float inputZ = 0f;
    //    float inputX = 0f;

    //    if (inputSystem.AxisY < 0)
    //    {
    //        inputZ -= moveSpeed * Time.deltaTime;
    //    }
    //    else if (inputSystem.AxisY > 0)
    //    {
    //        inputZ += moveSpeed * Time.deltaTime;
    //    }

    //    if (inputSystem.AxisX < 0)
    //    {
    //        inputX -= moveSpeed * Time.deltaTime;
    //    }
    //    else if (inputSystem.AxisX > 0)
    //    {
    //        inputX += moveSpeed * Time.deltaTime;
    //    }

    //    MoveCamera(inputX, inputZ);

    //}
}
