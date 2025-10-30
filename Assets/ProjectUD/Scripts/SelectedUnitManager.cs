using InputEventInterface;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectedUnitManager : MonoBehaviour, IInputClick, IInputRightClick, IInputUnitDelete
    , IInputUnitUpgrade, IInputUnitModeChange, IInputPerformUnitUpgrade, IInputESC
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

    [SerializeField] private Button upgradeBtn;


    [SerializeField] private AudioClip upgradeSfx;
    [SerializeField] private AudioClip siegeSfx;
    [SerializeField] private AudioClip freeSfx;

    private Unit selectedUnit;
    private AllyUnit selectedAllyUnit;
    private bool isUpgradeOn;
    private bool rightClickOn;

    public bool RightClickOn => rightClickOn;

    public Unit SelectedUnit => selectedUnit;

    public void OnUpgrade(bool on)
    {
        if(on)
        {
            isUpgradeOn = true;
        }
        else
        {
            isUpgradeOn = false;
        }
    }


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
                    inputEventManager.OnRightClickTarget = this;
                    inputEventManager.OnESCTarget = this;

                    Unit unit = hit.collider.GetComponent<Unit>();

                    unitSelectUI.OffUpgradeUI();


                    if (unit is AllyUnit)
                    {
                        AllyUnit allyUnit = unit as AllyUnit;
                        if (allyUnit.IsChange || allyUnit.IsUpgrade)
                            return;
                    }

                    if (unit.HpPercent <= 0.0f)
                    {
                        return;
                    }

                    SoundManager.Instance.PlayUIClickSFX();


                    if (selectedUnit != null)   // 선택한 유닛이 잇음
                    {
                        if(unit != selectedUnit)    // 선택한 유닛이 새 유닛
                        {
                            // 기존 유닛 해제
                            selectedUnit.IsSelected = false;
                            selectedUnit.SetSelectedUnitUI(null);
                            selectedUnit.SetSelectedUnitManager(null);
                            selectedUnit = null;

                            // 새 유닛 설정
                            selectedUnit = unit;
                            selectedUnit.SetSelectedUnitManager(this);
                            selectedUnit.SetSelectedUnitUI(unitSelectUI);
                            selectedUnit.IsSelected = true;
                            
                        }

                    }
                    else
                    {
                        // 새 유닛 설정
                        selectedUnit = unit;
                        selectedUnit.SetSelectedUnitManager(this);
                        selectedUnit.SetSelectedUnitUI(unitSelectUI);
                        selectedUnit.IsSelected = true;
                    }

                    UnitData unitData = selectedUnit.Data;


                    unitSelectUI.UpdateUnitInfo(selectedUnit);


                    if (selectedUnit is AllyUnit)
                    {
                        selectedAllyUnit = (AllyUnit)selectedUnit;
                        unitSelectUI.ShowAllyUI((AllyUnit)selectedUnit);
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
                        // 시즈모드시 타일 누르면 선택 해제
                        if ((selectedAllyUnit.ModeType == AllyUnit.Mode.SEIGE))
                        {
                            if(selectedUnit != null && selectedUnit is AllyUnit)
                            {
                                selectedUnit.IsSelected = false;
                                selectedUnit.SetSelectedUnitUI(null);
                                selectedUnit.SetSelectedUnitManager(null);
                                selectedUnit = null;
                                unitSelectUI.HideAllyUI();
                                unitSelectUI.HideHp();
                                unitSelectUI.HideUpgrdeUI();
                            }

                            
                        }
                        
                        // 프리 모드시 이동 불가 타일 확인
                        else if ((selectedAllyUnit.ModeType == AllyUnit.Mode.FREE))
                        {
                            //Tile tile = hit.collider.GetComponent<Tile>();
                            //if(tile.TileAllyUnit != null)
                            //{
                            //    ingameScreenUI.ShowError("병사가 이동할 수 없습니다");
                            //    return;
                            //}

                            selectedAllyUnit.MoveCommandDestination(hit.point);
                            mouseIndicatorParticle.gameObject.SetActive(true);
                            mouseIndicatorParticle.Play();
                            mouseIndicatorParticle.transform.position = hit.point;
                        }
                    }
                }
                //else
                //{
                //    Debug.Log(hit.collider.name);
                //}

            }
        }
    }


    // 마우스 우클릭은 선택 해제
    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DeSelecteUnit();
            inputEventManager.OnESCTarget = inGameManager;

            //Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            //RaycastHit hit;

            //if (Physics.Raycast(ray, out hit))
            //{
            //    if (selectedUnit != null)
            //    {
            //        selectedUnit.IsSelected = false;
            //        selectedUnit = null;
            //        unitSelectUI.HideAllyUI();
            //        unitSelectUI.HideHp();
            //        unitSelectUI.HideUpgrdeUI();
            //        unitSelectUI.HideUntInfo();
            //    }
            //    else
            //    {
            //        unitSelectUI.HideUntInfo();
            //    }

            //    commandSkillManager.CancelSkill();



            //    allyUnitSpawner.CancelSpawn();
            //}

            //isUpgradeOn = false;
        }
    }

    public void ShowUpgradeMenu()
    {
        if(selectedUnit.Data.Tier >= 4)
        {
            Debug.Log("업그레이드 불가");
            return;
        }

        if(selectedUnit.Data.Name == "수행자")
        {
            SoundManager.Instance.PlayUnableUIClickSFX();
            //upgradeBtn.interactable = false;
            return;
        }

        //upgradeBtn.interactable = true;
        SoundManager.Instance.PlayUIClickSFX();

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
            //unitSelectUI.HideUpgrdeUI();
            // 승급불과 효과음 추가 예정
            return;
        }

        SoundManager.Instance.PlayUIClickSFX();

        selectedAllyUnit.UpgradeOrder(index);

        inputEventManager.OnESCTarget = this;
        inputEventManager.OnRightClickTarget = this;

        inGameManager.SetGold(nextUnitData.Cost, false);
        ingameScreenUI.SetspawnBtnPriceTextColor();

        SoundManager.Instance.PlaySFX(upgradeSfx, selectedAllyUnit.transform.position);

        unitSelectUI.HideUpgrdeUI();
        unitSelectUI.HideAllyUI();
    }

    public void ModeChangeSelectedUnit()
    {
        if(!selectedAllyUnit.IsSelected)
            return;

        SoundManager.Instance.PlayUIClickSFX();

        if(selectedAllyUnit.ModeType == AllyUnit.Mode.SEIGE)
        {
            SoundManager.Instance.PlaySFX(siegeSfx, selectedAllyUnit.transform.position);
        }
        else if (selectedAllyUnit.ModeType == AllyUnit.Mode.FREE)
        {
            SoundManager.Instance.PlaySFX(freeSfx, selectedAllyUnit.transform.position);
        }

        selectedAllyUnit.ChangeOrder();
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
                    EnemyUnit enemyUnit = selectedUnit as EnemyUnit; 
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
                if (selectedAllyUnit.IsChange ||
                    selectedAllyUnit.IsUpgrade)
                    return;

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
            if (selectedAllyUnit != null)
            {
                AllyUnitData allyUnitData = selectedAllyUnit.Data as AllyUnitData;


                if (selectedAllyUnit.IsChange ||
                    selectedAllyUnit.IsUpgrade)
                    return;

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
                AllyUnitData allyUnitData = selectedAllyUnit.Data as AllyUnitData;

                if (allyUnitData.UpgradeUnits.Length <= 0 || selectedAllyUnit.Data.Name == "언월도병")
                    return;

                string keyName = context.control.name;

                if (int.TryParse(keyName, out int upgradeOption))
                {
                    if(upgradeOption == 2 && allyUnitData.UpgradeUnits.Length <= 1)
                    {
                        return;
                    }
                    

                    UpgradeSelectedUnit(upgradeOption - 1);
                    isUpgradeOn = false;
                }
                else
                    return;
            }
        }
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            DeSelecteUnit();
            inputEventManager.OnESCTarget = inGameManager;
        }
    }

    public void DeSelecteUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.IsSelected = false;
            selectedUnit.SetSelectedUnitUI(null);
            selectedUnit.SetSelectedUnitManager(null);
            unitSelectUI.HideAllyUI();
            unitSelectUI.HideHp();
            unitSelectUI.OffUpgradeUI();
            unitSelectUI.HideUntInfo();
            selectedUnit = null;
        }
    }

    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;

        if(selectedUnit is AllyUnit)
            selectedAllyUnit = (AllyUnit)selectedUnit;
    }
}
