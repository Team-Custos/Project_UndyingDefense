using InputEventInterface;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SelectedUnitManager : MonoBehaviour, IInputClick, IInputUnitDelete
    , IInputUnitUpgrade, IInputUnitModeChange, IInputPerformUnitUpgrade
{
    [Header("Selected Unit UI")]
    [SerializeField] private SelectedUnitUI unitSelectUI;                   // 선택한 유닛 UI
    [Header("Selected Unit Info Panel")]
    [SerializeField] private SelectedUnitInfoPanel selectedUnitInfoPanel;   // 선택한 유닛 정보 패널
    [Header("Unit Upgrade UI")]
    [SerializeField] private UpgradeMenuUI unitUpgradeMenuUI;                      // 유닛 업그레이드 UI

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

    private float currentGold;

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

                    ShowSelectedUnitUI();       // 리팩토링 적용
                    //if (allyUnit != null)
                    //    unitSelectUI.ShowAllyMenu(allyUnit);
                    //else
                    //    unitSelectUI.HideAllyMenu();

                    //selectedUnitInfoPanel.UpdateUnitInfo(unit);
                    //unitSelectUI.Show(unit);
                    //unitSelectUI.UpdateUnitInfo(unit);
                    //unitSelectUI.ShowHp(unit);    // 삭제 261025
                    //inputEventManager.OnClickTarget = this;
                }
                else if (hit.collider.CompareTag("Tile")) // 타일 클릭
                {
                    if (selectedAllyUnit != null)
                    {
                        // 시즈모드시 타일 누르면 선택 해제
                        if ((selectedAllyUnit.ModeType == AllyUnit.Mode.SEIGE))
                        {
                            ingameScreenUI.ShowError("IngameUI", "MSG_noMove2");
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


    //------------------------------
    // UI 관련 리팩토링 필요_261026 _ 병사 승급 비용 관련 진행중
    //------------------------------

    //------------------------------
    // 선택된 유닛이 보고 (Unit/AllyUnit에서 호출)
    //------------------------------

    // 선택 유닛 UI 표시_ 아군/적 판단은 여기서만
    public void ShowSelectedUnitUI()
    {
        if (selectedAllyUnit != null)
            unitSelectUI.ShowAlly(selectedAllyUnit);
        else if (selectedUnit is EnemyUnit enemy)
            unitSelectUI.ShowEnemy(enemy);

        selectedUnitInfoPanel.UpdateUnitInfo(selectedUnit);
    }

    public void OnSelectedUnitHpChanged(Unit unit)
    {
        if (unit != selectedUnit) return;

        unitSelectUI.SetHp(unit.HpPercent);
        selectedUnitInfoPanel.UpdateHPUI(unit.Hp, unit.Maxhp);
    }

    public void OnSelectedUnitStateChanged(Unit unit)
    {
        if (unit != selectedUnit) return;
        selectedUnitInfoPanel.UpdateUnitStateUI(unit.EffectList);
    }

    public void OnSelectedUnitDead(Unit unit)
    {
        if (unit != selectedUnit) return;
        CancleUpgrade();
        DeSelecteUnit();
    }

    // 임무 전환·승급 진행 바
    public void OnSelectedUnitOrderProgress(AllyUnit unit, float progress)
    {
        if (unit != selectedUnit) return;
        unitSelectUI.ShowUnitDuration(progress);
    }

    // 임무 전환 완료
    public void OnSelectedUnitModeChanged(AllyUnit unit)
    {
        if (unit != selectedUnit) return;
        unitSelectUI.HideUnitDuration();
        unitSelectUI.ShowAllyMenu(unit);
    }

    // 승급 완료: 선택 대상을 새 유닛으로 교체
    public void OnSelectedUnitUpgraded(AllyUnit oldUnit, AllyUnit newUnit)
    {
        if (oldUnit != selectedUnit) return;

        oldUnit.IsSelected = false;
        oldUnit.SetSelectedUnitManager(null);

        SetSelectedUnit(newUnit);
        ShowSelectedUnitUI();
        //unitSelectUI.Show(newUnit);
        //unitSelectUI.ShowAllyMenu(newUnit);
        //selectedUnitInfoPanel.UpdateUnitInfo(newUnit);
    }

    // InGameManager가 골드를 알려줄 때 호출
    public void OnGoldChanged(float gold)
    {
        currentGold = gold;

        if (isActivateUpgrade)      // 승급창이 열려 있을 때만 갱신
            RefreshUpgradeCost();
    }

    // 창을 열 때, 승급창이 열러있을 때 공통   
    private void RefreshUpgradeCost()
    {
        if (selectedUnit == null || !(selectedUnit.Data is AllyUnitData data))
            return;

        for (int i = 0; i < data.UpgradeUnits.Length; i++)
        {
            if (!(data.UpgradeUnits[i] is AllyUnitData upgradeData))
                continue;

            unitUpgradeMenuUI.SetUpgradeAvailable(i, currentGold >= upgradeData.Cost);
        }
    }

    public void ActivateUpgrade()   // 승급 버튼 클릭 이벤트용 메서드
    {
        if(selectedUnit.Data.Tier >= 4)
        {
            Debug.Log("업그레이드 불가");
            return;
        }

        SoundManager.Instance.PlayUIClickSFX();

        inGameManager.UpdateOperateState(OperateState.UPGRADE);
        isActivateUpgrade = true;

        // 병사 승급 창 
        //unitSelectUI.ShowUpgradeMenu(selectedUnit);             // 삭제할 코드
        if (!inGameManager.IsGameStart)
            return;
        //float gold = inGameManager.inGameGold;
        unitUpgradeMenuUI.SetUpgradeUnitMenu(selectedUnit);     // v 리팩토링 코드_261021 // v 골드 안넘겨주기
        RefreshUpgradeCost();

        camManager.FocusSelectedUnit(selectedUnit.transform.position);
    }

    //// 인게임매니저에서 골드를 가져와 비용을 확인하고 비용글자색을 바꾸는 메서드
    //public void UpdateUpgradePanelCost(float currentGold)
    //{
    //    // 여기서 계산
    //    AllyUnitData data = selectedUnit.Data as AllyUnitData;
    //    UnitData[] upgradeUnitdatas = data.UpgradeUnits;
    //    for (int i = 0; i < upgradeUnitdatas.Length; i++)
    //    {
    //        AllyUnitData upgradeUnitData = upgradeUnitdatas[i] as AllyUnitData;
    //        float cost = upgradeUnitData.Cost;
    //        unitUpgradeMenuUI.UpdateCostTxtColor(i, currentGold > cost);    // 인덱스 포함
    //    }
    //}

    public void UpgradeSelectedUnit(int index)
    {
        AllyUnitData allyUnitData = selectedAllyUnit.Data as AllyUnitData;

        AllyUnitData nextUnitData = allyUnitData.UpgradeUnits[index] as AllyUnitData;

        if (nextUnitData == null)
            return;


        // 골드 부족 여부 확인
        if (currentGold < nextUnitData.Cost)    //inGameManager.inGameGold
        {
            Debug.Log("골드 부족");
            return;
        }

        SoundManager.Instance.PlayUIClickSFX();

        selectedAllyUnit.UpgradeOrder(index);

        CancleUpgrade();
        inGameManager.UpdateOperateState(OperateState.ALLYUNIT);

        inGameManager.SetGold(nextUnitData.Cost, false);
        //ingameScreenUI.SetspawnBtnPriceTextColor();

        SoundManager.Instance.PlayUISFX(upgradeSfx);

        //UpdateUpgradeState(false);
        unitSelectUI.HideAllyMenu();
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
        unitSelectUI.HideAllyMenu();
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

    // 단축키 병사 승급 
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
        // 병사승급 UI 닫기 & 선택한 유닛 UI 보여주기
        if(selectedUnit != null && selectedUnit is AllyUnit)
        {
            unitSelectUI.ShowAllyMenu(selectedAllyUnit);
        }
        unitUpgradeMenuUI.HideUpgradeUI();
        //unitSelectUI.HideUpgrdeUI();
    }

    //------------------------------
    // UI 관련 리팩토링 필요_261026
    //
    // >> 이벤트 구독해제가 아니라 hp가 바뀌면-> SelectedUnitManager에서 UI에 일어나야할 일들을 모아서 명령
    //------------------------------
    public void DeSelecteUnit()
    {
        if(selectedUnit == null)
            return;

        selectedUnit.IsSelected = false;
        selectedUnit.SetSelectedUnitManager(null);
        unitSelectUI.Hide();
        selectedUnitInfoPanel.HideUnitInfo();
        selectedUnit = null;
        selectedAllyUnit = null;

        //unitSelectUI.HideAllyMenu();
        //selectedUnit.SetSelectedUnitUI(null);
        //selectedUnit.SetSelectedUnitInfoPanel(null);
        //unitSelectUI.HideHp();    // 삭제 261025
        //unitSelectUI.HideUnitInfo();
    }

    public void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;                                // 유닛 대입
        selectedUnit.IsSelected = true;
        selectedUnit.SetSelectedUnitManager(this);          // 참조는 매니저 하나만
        //selectedUnit.SetSelectedUnitUI(unitSelectUI);
        //selectedUnit.SetSelectedUnitInfoPanel(selectedUnitInfoPanel);

        if (selectedUnit is AllyUnit)
            selectedAllyUnit = (AllyUnit)selectedUnit;
        else
            selectedAllyUnit = null;
    }
}
