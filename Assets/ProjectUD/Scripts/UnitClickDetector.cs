using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitClickDetector : MonoBehaviour, IInputClick, IInputRightClick
{
    [SerializeField] private UnitSelectUI unitSelectUI;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private InGameManager inGameManager; 

    private Unit selectedUnit;
    private AllyUnit selectedAllyUnit;


    private void Start()
    {
        inputEventManager.OnClickTarget = this;
        inputEventManager.OnRightClickTarget = this;
    }

    // 마우스 좌클릭 선택
    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (inputEventManager.IsPointerOnUIElements())
                    return;

                if (hit.collider.CompareTag("Unit"))
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
                        if (!(selectedAllyUnit.ModeType == AllyUnit.Mode.FREE))
                            return;

                        selectedAllyUnit.destinationPosition = hit.point;

                    }
                }
                else
                {
                    Debug.Log(hit.collider.name);
                }

            }
        }
    }

    // 마우스 우클릭은 선택 해제
    public void OnRightClick(InputAction.CallbackContext context)
    {   
        if (context.performed)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(selectedUnit != null)
                {
                    selectedUnit = null;
                    selectedAllyUnit.isSelected = false;
                    unitSelectUI.HideAllyUI();
                    unitSelectUI.HideHp();
                }

                allyUnitSpawner.CancelSpawn();
            }
        }
    }

    public void UpgradeSelectedUnit(int index)
    {
        selectedAllyUnit.Upgrade(index);

        var allyUnitData = selectedAllyUnit.Data as AllyUnitData;

        inGameManager.SetGold(allyUnitData.Cost, false);

        unitSelectUI.HideAllyUI();
    }

    public void ModeChangeSelectedUnit()
    {
        selectedAllyUnit.ChangeMode(AllyUnit.Mode.CHANGE);
        unitSelectUI.HideAllyUI();
    }
}
