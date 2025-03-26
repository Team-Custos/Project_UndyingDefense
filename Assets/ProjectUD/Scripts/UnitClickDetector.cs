using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitClickDetector : MonoBehaviour, IInputClick
{
    [SerializeField] private UnitSelectUI unitSelectUI;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputEventManager inputEventManager;


    private void Start()
    {
        inputEventManager.OnClickTarget = this;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.CompareTag("Unit"))
                {
                    Unit unit = hit.collider.GetComponent<Unit>();
                    UnitData unitData = unit.Data;

                    if (unit is AllyUnit)
                    {
                        unitSelectUI.ShowAllyUI((AllyUnit)unit, (AllyUnitData)unitData);
                    }
                    else
                    {
                        unitSelectUI.HideAllyUI();
                    }

                    unitSelectUI.ShowHp(unit);
                }
                else
                {
                    //unitSelectUI.HideAllyUI();
                    unitSelectUI.HideHp();
                }

            }
        }
    }
}
