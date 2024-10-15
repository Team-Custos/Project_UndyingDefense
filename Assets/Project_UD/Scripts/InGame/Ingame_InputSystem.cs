using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ingame_InputSystem : MonoBehaviour
{

    public static Ingame_InputSystem Instance { get; private set; }

    public float AxisX = 0;
    public float AxisY = 0;

    public bool IsPressedPrimaryButton = false;
    public bool IsPressedSecondaryButton = false;
    public bool IsPressingSecondaryButton = false;
    public bool IsPressedWheelButton = false;

    public bool IsWheelScrollUp = false;
    public bool IsWheelScrollDown = false;

    public System.Action OnPrimaryPerformed;
    public System.Action OnSecondaryPerformed;
    public System.Action OnWheelButtonPerformed;

    private void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        //Instance = null;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        _inputInit();
    }

    void _inputInit()
    {
        IsPressedPrimaryButton = Input.GetMouseButtonDown(0);
        if (IsPressedPrimaryButton)
        {
            OnPrimaryPerformed?.Invoke();
        }

        IsPressedSecondaryButton = Input.GetMouseButtonDown(1);
        if (IsPressedSecondaryButton)
        {
            OnSecondaryPerformed?.Invoke();
        }
        IsPressingSecondaryButton = Input.GetMouseButton(1);

        IsPressedWheelButton = Input.GetMouseButtonDown(2);
        if (IsPressedWheelButton)
        {
            OnWheelButtonPerformed?.Invoke();
        }

        IsWheelScrollUp = Input.mouseScrollDelta.y > 0;
        IsWheelScrollDown = Input.mouseScrollDelta.y < 0;

        AxisX = Input.GetAxisRaw("Horizontal");
        AxisY = Input.GetAxisRaw("Vertical");

    }

}
