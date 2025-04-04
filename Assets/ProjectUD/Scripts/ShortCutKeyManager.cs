using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShortCutKeyManager : MonoBehaviour, IInputSpeedUp, IInputUnitDelete
{
    [SerializeField] private PlayerInputEventManager inputEventManager;

    private ObjectPoolWithList<Unit> unitPool;
    private Unit selectedUnit;

    private void Start()
    {
        inputEventManager.OnSpeedUpTarget = this;
        inputEventManager.OnUnitDeleteTarget = this;
    }

    public void OnSpeedUp(InputAction.CallbackContext context)
    {
        if (context.action.name == "SpeedUp" && context.performed)
        {
            Time.timeScale = 3.0f;
        }
        else if (context.canceled)
        {
            Time.timeScale = 1.0f;
        }
    }

    public void OnUnitDelete(InputAction.CallbackContext context)
    {
        if (context.action.name == "UnitDelete" && context.performed)
        {
            if (selectedUnit != null)
            {
                //unitPool.Pool.Release(selectedUnit);
                Destroy(selectedUnit.gameObject);
                selectedUnit = null;
            }
        }
    }
}
