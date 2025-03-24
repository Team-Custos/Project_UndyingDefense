using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShortCutKeyManager : MonoBehaviour, IInputSubmit
{
    [SerializeField] private PlayerInputEventManager inputEventManager;

    private ObjectPoolWithList<Unit> unitPool;
    private Unit selectedUnit;

    private void Start()
    {
        inputEventManager.OnSubmitTarget = this;
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.control.name == "e")
        {
            if (context.performed)
            {
                Time.timeScale = 3.0f;
            }
            else if (context.canceled)
            {
                Time.timeScale = 1.0f;
            }
        }
        else if (context.control.name == "h")
        {
            if (context.performed)
            {
                unitPool.Pool.Release(selectedUnit);
                selectedUnit = null;
                Debug.Log("유닛 삭제");
            }
        }
    }




    //public void OnSubmit(InputAction.CallbackContext context)
    //{
    //    if (context.action.name == "Submit" && context.performed)
    //    {
    //        Time.timeScale = 3.0f;
    //    }
    //    else if (context.canceled)
    //    {
    //        Time.timeScale = 1.0f;
    //    }
    //}
}
