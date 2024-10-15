using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Ingame_CamManager : MonoBehaviour
{
    Ingame_InputSystem inputSystem;


    public float camZoomValue = 0.5f;

    public float moveSpeed = 2;

    private bool _userMoveInput; // 현재 조작을 하고있는지 확인을 위한 변수
    private Vector3 _startPosition;  // 입력 시작 위치를 기억
    private Vector3 _directionForce; // 조작을 멈췄을때 서서히 감속하면서 이동 시키기 


    // Start is called before the first frame update
    void Start()
    {
        inputSystem = Ingame_InputSystem.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        MouseMove();
        KeyboardMove();
        ZoomCamera();




    }

    private void MoveCamera(float xInput, float zInput)
    {
        float zMove = Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * zInput - Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180) * xInput;
        float xMove = Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180) * zInput + Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * xInput;

        transform.position = transform.position + new Vector3(xMove, 0, zMove);
    }

    void ZoomCamera()
    {
        if (inputSystem.IsWheelScrollUp)
        {
            transform.position = transform.position + new Vector3(0, camZoomValue, 0);
        }
        else if (inputSystem.IsWheelScrollDown)
        {
            transform.position = transform.position - new Vector3(0, camZoomValue, 0);
        }
    }



    // Get mouse drag inputs
    void MouseMove()
    {
        var mouseWorldPosition = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);

        if (inputSystem.IsPressedSecondaryButton)
        {
            _userMoveInput = true;
            _startPosition = mouseWorldPosition;
            _directionForce = Vector2.zero;
        }

        else if (inputSystem.IsPressingSecondaryButton)
        {
            if (!_userMoveInput)
            {
                _userMoveInput = true;
                _startPosition = mouseWorldPosition;
                _directionForce = Vector2.zero;
                return;
            }

            _directionForce = _startPosition - mouseWorldPosition;
        }
        else
        {
            _userMoveInput = false;
        }
    }
    private void UpdateCameraPosition()
    {
        // 이동 수치가 없으면 아무것도 안함
        if (_directionForce == Vector3.zero)
        {
            return;
        }

        var currentPosition = transform.position;
        var targetPosition = currentPosition + _directionForce;
        transform.position = Vector3.Lerp(currentPosition, targetPosition, 0.5f);
    }

    void KeyboardMove()
    {
        //transform.position = new Vector3(camPosition_X, camPosition_Y, camPosition_Z);

        float inputZ = 0f;
        float inputX = 0f;

        if (inputSystem.AxisY < 0)
        {
            inputZ -= moveSpeed * Time.deltaTime;
        }
        else if (inputSystem.AxisY > 0)
        {
            inputZ += moveSpeed * Time.deltaTime;
        }

        if (inputSystem.AxisX < 0)
        {
            inputX -= moveSpeed * Time.deltaTime;
        }
        else if (inputSystem.AxisX > 0)
        {
            inputX += moveSpeed * Time.deltaTime;
        }

        MoveCamera(inputX, inputZ);

    }

}
