using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_CamManager : MonoBehaviour
{
    UD_Ingame_InputSystem inputSystem;

    float camPosition_X = 0;
    float camPosition_Z = 0;

    public float camPosition_Y = 0;

    public float moveSpeed = 2;
    Vector3 dragOrigin;


    // Start is called before the first frame update
    void Start()
    {
        inputSystem = UD_Ingame_InputSystem.Instance;   
    }

    // Update is called once per frame
    void LateUpdate()
    {
        MouseMove();
        KeyboardMove();
        


        if (inputSystem.IsWheelScrollUp)
        {
            camPosition_Y += 0.5f;
            
        }
        else if (inputSystem.IsWheelScrollDown)
        {
            camPosition_Y -= 0.5f;
        }
    }

    private void MoveCamera(float xInput, float zInput)
    {
        float zMove = Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * zInput - Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180) * xInput;
        float xMove = Mathf.Sin(transform.eulerAngles.y * Mathf.PI / 180) * zInput + Mathf.Cos(transform.eulerAngles.y * Mathf.PI / 180) * xInput;

        transform.position = transform.position + new Vector3(xMove, 0, zMove);
    }

    // Get mouse drag inputs
    void MouseMove()
    {
        if (inputSystem.IsPressedSecondaryButton)
        {
            dragOrigin = transform.position;
            dragOrigin = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        if (inputSystem.IsPressingSecondaryButton)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition) - dragOrigin;
            Vector3 desirePos = dragOrigin + -1 * new Vector3(pos.x, 0, pos.y) * moveSpeed;
            Vector3 move = desirePos - transform.position;
            MoveCamera(move.x, move.z);
        }

        if (Input.GetMouseButtonUp(0))
        {
        }
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
