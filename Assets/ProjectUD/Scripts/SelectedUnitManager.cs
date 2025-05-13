using InputEventInterface;
using UnityEngine;
using UnityEngine.InputSystem;

public class SelectedUnitManager : MonoBehaviour, IInputClick, IInputRightClick, IInputUnitDelete
    , IInputUnitUpgrade, IInputUnitModeChange, IInputPerformUnitUpgrade
{
    [SerializeField] private SelectedUnitUI unitSelectUI;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private EnemyUnitSpawner enemyUnitSpawner;
    [SerializeField] private Ingame_CamManager camManager;
    [SerializeField] private IngameCommandSkillManager commandSkillManager;
    [SerializeField] private ParticleSystem mouseIndicatorParticle;


    private Unit selectedUnit;
    private AllyUnit selectedAllyUnit;
    private AllyUnit tileAllyUnit;
    private bool isUpgradeOn;

    public Unit SelectedUnit => selectedUnit;


    private void Start()
    {
        inputEventManager.OnClickTarget = this;
        inputEventManager.OnRightClickTarget = this;
        inputEventManager.OnUnitDeleteTarget = this;
        inputEventManager.OnUnitModeChangeTarget = this;
        inputEventManager.OnUnitUpgradeTarget = this;
        inputEventManager.OnPerformUnitUpgradeTarget = this;
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

                    if (unit is AllyUnit)
                    {
                        AllyUnit allyUnit = unit as AllyUnit;
                        if (allyUnit.ModeType == AllyUnit.Mode.CHANGE || allyUnit.ModeType == AllyUnit.Mode.UPGRADE)
                            return;
                    }

                    if (unit.HpPercent <= 0.0f)
                    {
                        return;
                    }

                    camManager.FocusSelectedUnit(hit.transform.position);

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
                    //if(commandSkillManager.isCommandSkillActive)
                    //{
                    //    commandSkillManager.clickPos.position = hit.point;
                    //    commandSkillManager.isCommandSkillActive = false;
                    //    commandSkillManager.clickPos = null;
                    //    return;
                    //}

                    if (selectedAllyUnit != null && selectedAllyUnit.IsSelected)
                    {
                        

                        if (!(selectedAllyUnit.ModeType == AllyUnit.Mode.FREE))
                        {
                            Tile tile = hit.collider.GetComponent<Tile>();
                            if(tile.SetAllyUnit(selectedAllyUnit) == null)
                            {
                                ingameScreenUI.ShowError("병사가 이동할 수 없습니다");
                            }

                            return;
                        }
                            

                        selectedAllyUnit.DestinationPosition = hit.point;
                        mouseIndicatorParticle.gameObject.SetActive(true);
                        mouseIndicatorParticle.Play();
                        mouseIndicatorParticle.transform.position = hit.point;

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

        if (nextUnitData == null)
            return;


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

                isUpgradeOn = true;

                //string keyNumber = context.control.name;

                //  if (int.TryParse(keyNumber, out int upgradeOption))
                //  {
                //      UpgradeSelectedUnit(upgradeOption - 1);
                //  }
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

    public void OnPerformUnitUpgrade(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(selectedAllyUnit != null && isUpgradeOn)
            {
                string keyName = context.control.name;

                if (int.TryParse(keyName, out int upgradeOption))
                {
                    UpgradeSelectedUnit(upgradeOption - 1);
                    isUpgradeOn = false;
                }
                else
                    return;
            }
        }
    }
}
