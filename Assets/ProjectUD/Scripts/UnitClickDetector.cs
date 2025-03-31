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

    private Unit selectedUnit;
    private AllyUnit selectedAllyUnit;


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
                    selectedUnit = hit.collider.GetComponent<Unit>();
                    UnitData unitData = selectedUnit.Data;

                    if (selectedUnit is AllyUnit)
                    {
                        selectedAllyUnit = (AllyUnit)selectedUnit;
                        selectedAllyUnit.isSelected = true;
                        unitSelectUI.ShowAllyUI((AllyUnit)selectedUnit, (AllyUnitData)unitData);
                    }
                    else
                    {
                        unitSelectUI.HideAllyUI();
                    }

                    unitSelectUI.ShowHp(selectedUnit);
                }
                else if (hit.collider.CompareTag("Ground"))
                {
                    if (selectedAllyUnit != null && selectedAllyUnit.isSelected)
                    {
                        hit.point = new Vector3(hit.point.x, hit.point.y + 0.5f, hit.point.z);

                        selectedAllyUnit.destinaitonTransfrom.position = hit.point;

                        Debug.Log(selectedAllyUnit.destinaitonTransfrom.position);
                    }
                }
                else
                {
                    Debug.Log(hit.collider.name);
                    //unitSelectUI.HideAllyUI();
                    unitSelectUI.HideHp();
                    selectedUnit = null;
                    //selectedAllyUnit.destinaitonTranfrom = null;
                }

            }
        }
    }

    public void UpgradeSelectedUnit(int index)
    {
        Debug.Log("Uprade");
        selectedAllyUnit.Upgrade(index);
    }

    public void ModeChangeSelectedUnit()
    {
        Debug.Log("Mode Change");
        selectedAllyUnit.ChangeMode(AllyUnit.Mode.CHANGE);
    }
}
