using InputEventInterface;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectedUnitManager : MonoBehaviour, IInputClick, IInputRightClick, IInputUnitDelete
    , IInputUnitUpgrade, IInputUnitModeChange
{
    [SerializeField] private SelectedUnitUI unitSelectUI;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private EnemyUnitSpawner enemyUnitSpawner;


    private Unit selectedUnit;
    private AllyUnit selectedAllyUnit;
    private bool isUpgradeOn;

    public Unit SelectedUnit => selectedUnit;


    private void Start()
    {
        inputEventManager.OnClickTarget = this;
        inputEventManager.OnRightClickTarget = this;
        inputEventManager.OnUnitDeleteTarget = this;
        inputEventManager.OnUnitModeChangeTarget = this;
        inputEventManager.OnUnitUpgradeTarget = this;
    }

    // 마우스 좌클릭 선택
    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (inputEventManager.IsPointerOnUIElements())
                    return;

                if (hit.collider.CompareTag("Unit"))
                {
                    allyUnitSpawner.CancelSpawn();

                    Unit unit = hit.collider.GetComponent<Unit>();

                    if (unit.HpPercent <= 0.0f)
                    {
                        return;
                    }

                    if (selectedUnit != null) // 새 유닛 선택
                    {
                        if(unit != selectedUnit)
                        {
                            selectedUnit.IsSelected = false;
                            selectedUnit.SetUnitUI(null);
                        }

                        unit.IsSelected = true;

                        selectedUnit = unit;
                    }
                    else
                    {
                        selectedUnit = unit;
                    }

                    UnitData unitData = selectedUnit.Data;

                    selectedUnit.IsSelected = true;

                    unitSelectUI.UpdateUnitInfo(selectedUnit);


                    if (selectedUnit is AllyUnit)
                    {
                        selectedAllyUnit = (AllyUnit)selectedUnit;
                        unitSelectUI.ShowAllyUI((AllyUnit)selectedUnit, (AllyUnitData)unitData);
                    }
                    else
                    {
                        unitSelectUI.HideAllyUI();
                    }

                    unitSelectUI.ShowHp(selectedUnit);
                }
                else if (hit.collider.CompareTag("Tile"))
                {
                    if (selectedAllyUnit != null && selectedAllyUnit.IsSelected)
                    {
                        if (!(selectedAllyUnit.ModeType == AllyUnit.Mode.FREE))
                            return;

                        selectedAllyUnit.DestinationPosition = hit.point;

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
                if (selectedUnit != null)
                {
                    selectedUnit.IsSelected = false;
                    selectedUnit = null;
                    unitSelectUI.HideAllyUI();
                    unitSelectUI.HideHp();
                    unitSelectUI.HideUpgrdeUI();
                }

                allyUnitSpawner.CancelSpawn();
            }

            isUpgradeOn = false;
        }
    }

    public void ShowUpgradeMenu()
    {
        if(selectedUnit.Data.Tier >= 4)
        {
            Debug.Log("업그레이드 불가");
            return;
        }

        unitSelectUI.ShowUpgradeMenu(selectedUnit);
    }

    public void UpgradeSelectedUnit(int index)
    {
        AllyUnitData allyUnitData = selectedAllyUnit.Data as AllyUnitData;

        AllyUnitData nextUnitData = allyUnitData.UpgradeUnits[index] as AllyUnitData;


        // 골드 부족 여부 확인
        if (inGameManager.inGameGold < nextUnitData.Cost)
        {
            Debug.Log("골드 부족");
            unitSelectUI.HideUpgrdeUI();
            return;
        }

        selectedAllyUnit.Upgrade(index);

        inGameManager.SetGold(nextUnitData.Cost, false);

        unitSelectUI.HideUpgrdeUI();
        unitSelectUI.HideAllyUI();
    }

    public void ModeChangeSelectedUnit()
    {
        selectedAllyUnit.ChangeMode(AllyUnit.Mode.CHANGE);
        unitSelectUI.HideAllyUI();
    }

    public void OnUnitDelete(InputAction.CallbackContext context)
    {
        if (context.action.name == "UnitDelete" && context.performed)
        {
            if (selectedUnit != null)
            {
                selectedUnit.gameObject.SetActive(false);

                if (selectedUnit is EnemyUnit)
                {
                    enemyUnitSpawner.OnEnemyDead();
                }

                selectedUnit = null;

                unitSelectUI.HideHp();
                unitSelectUI.HideAllyUI();
            }
        }
    }

    public void OnUnitUpgrade(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (selectedUnit != null && selectedUnit is AllyUnit)
            {
                ShowUpgradeMenu();
                //isUpgradeOn = true;

                //if(isUpgradeOn)
                //{
                //    string keyNumber = context.control.name;

                //    if (int.TryParse(keyNumber, out int upgradeOption))
                //    {
                //        UpgradeSelectedUnit(upgradeOption - 1);
                //    }
                //}
            }
        }
    }

    public void OnUnitModeChange(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (selectedUnit != null && selectedUnit is AllyUnit)
            {
                ModeChangeSelectedUnit();
            }
        }
    }
}
