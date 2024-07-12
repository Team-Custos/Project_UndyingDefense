using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UD_Ingame_InputSystem : MonoBehaviour
{

    public static UD_Ingame_InputSystem inst;

    public float AxisX = 0;
    public float AxisY = 0;

    public bool IsPressedPrimaryButton = false;
    public bool IsPressedSecondaryButton = false;
    public bool IsPressedWheelButton = false;

    public bool IsWheelScrollUp = false;
    public bool IsWheelScrollDown = false;

    private void Awake()
    {
        inst = this;
    }

    void OnDestroy()
    {
        inst = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        



    }

    // Update is called once per frame
    private void Update()
    {
        _inputInit();

        if (IsPressedPrimaryButton)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //오브젝트 클릭했을때
            }


        }

    }

    void _inputInit()
    {
        IsPressedPrimaryButton = Input.GetMouseButtonDown(0);
        IsPressedSecondaryButton = Input.GetMouseButtonDown(1);
        IsPressedWheelButton = Input.GetMouseButtonDown(2);

        IsWheelScrollUp = Input.mouseScrollDelta.y > 0;
        IsWheelScrollDown = Input.mouseScrollDelta.y < 0;
    }

}
