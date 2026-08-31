using InputEventInterface;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectedUnitManager : MonoBehaviour, IInputClick, IInputUnitDelete
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

    [SerializeField] private Button upgradeBtn;


    [SerializeField] private AudioClip upgradeSfx;
    [SerializeField] private AudioClip siegeSfx;
    [SerializeField] private AudioClip freeSfx;

    private Unit selectedUnit;
    private AllyUnit selectedAllyUnit;
    private bool isActivateUpgrade;
    private bool rightClickOn;

    public bool IsActivateUpgrade => isActivateUpgrade;
    public bool RightClickOn => rightClickOn;

    public Unit SelectedUnit => selectedUnit;

    private void Start()
    {
        inputEventManager.OnUnitModeChangeTarget = this;
        inputEventManager.OnUnitUpgradeTarget = this;
        inputEventManager.OnPerformUnitUpgradeTarget = this;
    }

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

                if (hit.collider.CompareTag("Unit"))    // 유닛 클릭
                {
                    //allyUnitSpawner.CancelSpawn();
                    

                    Unit unit = hit.collider.GetComponent<Unit>();

                    if (unit.IsDead)
                        return;


                    AllyUnit allyUnit = unit as AllyUnit;

                    if (allyUnit != null)
                    {
                        if (allyUnit.IsChange || allyUnit.IsUpgrade)
                            return;

                        inGameManager.UpdateOperateState(OperateState.ALLYUNIT);
                    }
                    else
                        inGameManager.UpdateOperateState(OperateState.DEFAULT);



                    SoundManager.Instance.PlayUIClickSFX();


                    if (selectedUnit != null)   // 선택한 유닛이 잇음
                    {
                        if (unit != selectedUnit)    // 선택한 유닛이 새 유닛
                        {
                            // 기존 유닛 해제
                            DeSelecteUnit();

                            // 새 유닛 설정
                            SetSelectedUnit(unit);
                        }

                    }
                    else
                    {
                        // 새 유닛 설정
                        SetSelectedUnit(unit);
                    }

                    if (allyUnit != null)
                        unitSelectUI.ShowAllyUI(allyUnit);
                    else
                        unitSelectUI.HideAllyUI();

                    unitSelectUI.UpdateUnitInfo(unit);
                    unitSelectUI.ShowHp(unit);
                    //inputEventManager.OnClickTarget = this;
                }
                else if (hit.collider.CompareTag("Tile")) // 타일 클릭
                {
                    if (selectedAllyUnit != null)
                    {
                        // 시즈모드시 타일 누르면 선택 해제
                        if ((selectedAllyUnit.ModeType == AllyUnit.Mode.SEIGE))
                        {
                            DeSelecteUnit();
                        }

                        // 프리 모드시 이동 불가 타일 확인
                        else if ((selectedAllyUnit.ModeType == AllyUnit.Mode.FREE))
                        {
                            if (selectedAllyUnit.IsPathBlocked(hit.point))
                            {
                                ingameScreenUI.ShowError("IngameUI", "MSG_noMove2");
                            }
                            else
                            {
                                selectedAllyUnit.UpdateCommandDestination(hit.point);
                                mouseIndicatorParticle.transform.position = hit.point;
                                mouseIndicatorParticle.gameObject.SetActive(true);
                                mouseIndicatorParticle.Play();
                            }
                        }
                    }
                }
                else if (!hit.collider.CompareTag("Tile"))
                {
                    if (selectedAllyUnit != null)
                    {
                        if ((selectedAllyUnit.ModeType == AllyUnit.Mode.FREE))
                        {
                            ingameScreenUI.ShowError("IngameUI", "MSG_noMove");
                        }
                    }

                }
                else
                {
                    Debug.Log(hit.collider.name);
                }

            }
        }
    }



    public void ActivateUpgrade()
    {
        if(selectedUnit.Data.Tier >= 4)
        {
            Debug.Log("업그레이드 불가");
            return;
        }

        SoundManager.Instance.PlayUIClickSFX();

        inGameManager.UpdateOperateState(OperateState.UPGRADE);
        isActivateUpgrade = true;

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
            return;
        }

        SoundManager.Instance.PlayUIClickSFX();

        selectedAllyUnit.UpgradeOrder(index);

        CancleUpgrade();
        inGameManager.UpdateOperateState(OperateState.ALLYUNIT);

        inGameManager.SetGold(nextUnitData.Cost, false);
        ingameScreenUI.SetspawnBtnPriceTextColor();

        SoundManager.Instance.PlayUISFX(upgradeSfx);

        //UpdateUpgradeState(false);
        unitSelectUI.HideAllyUI();
    }

    public void ModeChangeSelectedUnit()
    {
        if(!selectedAllyUnit.IsSelected)
            return;

        SoundManager.Instance.PlayUIClickSFX();

        if(selectedAllyUnit.ModeType == AllyUnit.Mode.SEIGE)
        {
            SoundManager.Instance.PlayUISFX(siegeSfx);
        }
        else if (selectedAllyUnit.ModeType == AllyUnit.Mode.FREE)
        {
            SoundManager.Instance.PlayUISFX(freeSfx);
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
                Debug.Log("삭제 키 잠금");
                return;


                //selectedUnit.gameObject.SetActive(false);

                //if (selectedUnit is EnemyUnit)
                //{
                //    EnemyUnit enemyUnit = selectedUnit as EnemyUnit; 
                //    enemyUnitSpawner.OnEnemyDead();
                //}

                //selectedUnit = null;

                //unitSelectUI.HideHp();
                //unitSelectUI.HideAllyUI();
            }
        }
    }

    public void OnUnitUpgrade(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inGameManager.IsGameStart || inGameManager.IsGamgePause)
                return;

            if (selectedUnit != null && selectedUnit is AllyUnit)
            {
                if (selectedAllyUnit.IsChange ||
                    selectedAllyUnit.IsUpgrade)
                    return;

                ActivateUpgrade();

                isActivateUpgrade = true;

                
            }
        }
    }


    public void OnUnitModeChange(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inGameManager.IsGameStart || inGameManager.IsGamgePause) 
                return;

            if (selectedAllyUnit != null)
            {
                AllyUnitData allyUnitData = selectedAllyUnit.Data as AllyUnitData;


                if (selectedAllyUnit.IsChange ||
                    selectedAllyUnit.IsUpgrade)
                    return;

                if(isActivateUpgrade)
                {
                    CancleUpgrade();
                    inGameManager.UpdateOperateState(OperateState.ALLYUNIT);
                }

                ModeChangeSelectedUnit();
            }
        }
    }

    public void OnPerformUnitUpgrade(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!inGameManager.IsGameStart || inGameManager.IsGamgePause)
                return;

            if (selectedAllyUnit != null && isActivateUpgrade)
            {
                AllyUnitData allyUnitData = selectedAllyUnit.Data as AllyUnitData;

                if (allyUnitData.UpgradeUnits.Length <= 0)
                    return;

                string keyName = context.control.name;

                if (keyName == "z")
                {
                    UpgradeSelectedUnit(0);
                }
                else if (keyName == "x")
                {
                    if (allyUnitData.UpgradeUnits.Length <= 1)
                        return;

                    UpgradeSelectedUnit(1);
                }
                else
                    return;
            }
        }
    }

    public void CancleUpgrade()
    {
        isActivateUpgrade = false;
        unitSelectUI.HideUpgrdeUI();
    }

    public void DeSelecteUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.IsSelected = false;
            selectedUnit.SetSelectedUnitUI(null);
            selectedUnit.SetSelectedUnitManager(null);
            unitSelectUI.HideHp();
            unitSelectUI.HideUntInfo();
            selectedUnit = null;
            selectedAllyUnit = null;
            unitSelectUI.HideAllyUI();
        }
    }

    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        selectedUnit.IsSelected = true;
        selectedUnit.SetSelectedUnitUI(unitSelectUI);
        selectedUnit.SetSelectedUnitManager(this);

        if (selectedUnit is AllyUnit)
            selectedAllyUnit = (AllyUnit)selectedUnit;
        else
            selectedAllyUnit = null;
    }
}
