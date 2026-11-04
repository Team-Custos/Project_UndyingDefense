using InputEventInterface;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class AllyUnitSpawner : MonoBehaviour, IInputClick, IInputUnitSpawn
{
    [Header("■ Components")]
    [SerializeField] private PlayerInputEventManager inputMng;
    [SerializeField] private Grid grid;
    [SerializeField] private Transform spawnDirection;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private SelectedUnitUI selectedUnitUI;
    [SerializeField] private IngameCommandSkillManager commandSkillManager;
    [SerializeField] private Ingame_CursorManager cursorManager;
    [SerializeField] private UnitDataLoader unitDataLoader;
    [SerializeField] private DurationEffectPool durationEffectPool;
    [SerializeField] private InstantEffectPool instantEffectPool;
    [SerializeField] private VFXObjectPool hitVFXPool;
    [SerializeField] private VFXObjectPool skillVFXPool;
    [SerializeField] private EffectImagePool effectImagePool;
    [SerializeField] private DollyCamera dollyCamera;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private EnemyUnitSpawner enemyUnitSpawner;


    [SerializeField] private Image[] alarmImages;

    [Header("■ Units")]
    [SerializeField] private AllyUnitData[] units;
    private EnemyUnit immortalityEnemy;

    [Header("■ Spawn Point")]
    [SerializeField] private GameObject spawnPointPrefab;

    [Header("■ UI")]
    [SerializeField] private UnitSpawnUI unitSpawnUI;
    [SerializeField] private GameObject indicator;


    [Header("■ Ground Layer")]
    [SerializeField] private LayerMask groundLayer;

    private int selectedIndex; // 현재 선택된 소환 가능한 유닛의 인덱스(-1 : 선택되지 않음)
    private bool spawn;
    private List<ObjectPoolWithList<AllyUnit>> unitPools;
    private ObjectPoolWithList<UnitSpawnPoint> spawnPointPool;

    private Dictionary<GameObject, ObjectPoolWithList<AllyUnit>> upgradeUnitPoolsDic =
        new Dictionary<GameObject, ObjectPoolWithList<AllyUnit>>(); // 업그레이드 유닛을 담을 풀

    [SerializeField] SelectedUnitManager selectedUnitManager;

    [SerializeField] private AudioClip allySummon;

    public AllyUnitData[] Units => units;



    private void Start()
    {
        selectedIndex = -1;
        unitPools = new List<ObjectPoolWithList<AllyUnit>>();
        for (int i = 0; i < units.Length; i++)
        {
            int index = i;
            unitPools.Add(new ObjectPoolWithList<AllyUnit>(() => CreateUnit(index)));
            spawnPointPool = new ObjectPoolWithList<UnitSpawnPoint>(CreateSpawnPoint);
            //Unit unit = units[i].Prefab.GetComponent<Unit>();

            // UI 설정
            AllyUnitData data = units[i];
            unitSpawnUI.SetSpawnButton(i, data.Icon, data.Tier, (int)data.Cost);
        }

        inputMng.OnUnitSpawnTarget = this;
    }

    private void Update()
    {
        if (spawn)
        {
            if (inputMng.IsPointerOnUIElements())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!hit.transform.CompareTag("Tile"))
                {
                    indicator.SetActive(false);
                    return;
                }

                indicator.transform.position = grid.CellToWorld(grid.WorldToCell(hit.point)) + new Vector3(grid.cellSize.x * 0.5f, 0.1f, grid.cellSize.y * 0.5f);
                indicator.SetActive(true);
            }
        }
    }

    private AllyUnit CreateUnit(int index)
    {
        AllyUnitData data = units[index];
        GameObject obj = Instantiate(data.Prefab);
        obj.SetActive(false);
        AllyUnit unit = obj.GetComponent<AllyUnit>();
        unit.Initialize(data, unitPools[index], this, waveManager);
        UnitStats unitStats = unitDataLoader.GetUnitDataById(unit.UnitId);
        unit.SetUnitStats(unitStats);

        unit.SetDurationEffectPool(durationEffectPool);
        unit.SetInstantEffectPool(instantEffectPool);
        unit.SetEffectImagePool(effectImagePool);
        unit.SetHitVFXPool(hitVFXPool, skillVFXPool);

        // 상태를 받았을 때 실행 될 함수 등록
        RegisterGetEffectEvent(unit);

        unitPools[index].List.Add(unit);

        return unit;
    }

    public AllyUnit CreateUpgradeUnit(GameObject allyUnitPrefab, AllyUnitData allyUnitData, Transform transform,
        Tile tile)
    {
        GameObject obj;
        AllyUnit upgradeUnit;

        // 해당 유닛의 값이 있는지 확인
        if (upgradeUnitPoolsDic.ContainsKey(allyUnitPrefab))
        {
            if (upgradeUnitPoolsDic[allyUnitPrefab].List.Count <= 0)    // 값은 있는데 남은 유닛이 없음
            {
                obj = Instantiate(allyUnitPrefab);
                obj.SetActive(false);

                upgradeUnit = obj.GetComponent<AllyUnit>();

                upgradeUnitPoolsDic[allyUnitPrefab].List.Add(upgradeUnit);
            }
            else    // 값도 있고 남은 유닛도 있음
            {

                upgradeUnit = upgradeUnitPoolsDic[allyUnitPrefab].Pool.Get();
                unitDataLoader.GetUnitDataById(upgradeUnit.UnitId);
                upgradeUnitPoolsDic[allyUnitPrefab].List.Add(upgradeUnit);
                //return upgradeUnit;
            }
        }
        else    // 해당 유닛의 값이 없음
        {
            upgradeUnitPoolsDic.Add(allyUnitPrefab, new ObjectPoolWithList<AllyUnit>(() =>
            {
                GameObject obj = Instantiate(allyUnitPrefab);
                obj.SetActive(false);

                AllyUnit upgradeUnit = obj.GetComponent<AllyUnit>();
                return upgradeUnit;
            }));

            upgradeUnit = upgradeUnitPoolsDic[allyUnitPrefab].Pool.Get();
            upgradeUnitPoolsDic[allyUnitPrefab].List.Add(upgradeUnit);

            // 상태를 받았을 때 실행 될 함수 등록
            RegisterGetEffectEvent(upgradeUnit);

            //return upgradeUnit;
        }

        UpgradeUnitInitialize(upgradeUnit, allyUnitData, allyUnitPrefab, transform, tile, effectImagePool, waveManager);

        return upgradeUnit;
    }

    private void UpgradeUnitInitialize(AllyUnit upgradeUnit, AllyUnitData allyUnitData, GameObject allyUnitPrefab, 
        Transform transform, Tile tile, EffectImagePool poolEffectImage, WaveManager waveManager)
    {
        UnitStats unitStats = unitDataLoader.GetUnitDataById(upgradeUnit.UnitId);
        upgradeUnit.SetUnitStats(unitStats);
        upgradeUnit.Initialize(allyUnitData, upgradeUnitPoolsDic[allyUnitPrefab], this, waveManager);
        upgradeUnit.Initialize();

        upgradeUnit.SetDurationEffectPool(durationEffectPool);
        upgradeUnit.SetInstantEffectPool(instantEffectPool);
        upgradeUnit.SetHitVFXPool(hitVFXPool, skillVFXPool);
        upgradeUnit.SetEffectImagePool(poolEffectImage);

        upgradeUnit.transform.position = transform.position;
        upgradeUnit.transform.rotation = transform.rotation;

        upgradeUnit.gameObject.SetActive(true);
        upgradeUnit.UnitGrid.SetTargetTile(tile);

        if (immortalityEnemy != null)
            upgradeUnit.SetImmortalityEnemy(immortalityEnemy);
    }

    private UnitSpawnPoint CreateSpawnPoint()
    {
        GameObject obj = Instantiate(spawnPointPrefab);
        obj.SetActive(false);
        UnitSpawnPoint spawnPoint = obj.GetComponent<UnitSpawnPoint>();
        spawnPoint.Initialize(spawnPointPool);

        return spawnPoint;
    }

    public void ToggleSpawnUnit(int index)
    {
        if (inGameManager.inGameGold < units[index].Cost)
        {
            //-- Localization
            ingameScreenUI.ShowError("IngameUI", "MSG_noGold");
        }


        if (index >= 0 && index < alarmImages.Length)
        {
            if (alarmImages[index] != null)
            {
                alarmImages[index].gameObject.SetActive(false);
            }
        }

        selectedIndex = index;
        spawn = true;
        //indicator.SetActive(true);
        inputMng.OnClickTarget = this;
        //inGameManager.CancleOperateState(OperateState.SPAWN);
        inGameManager.UpdateOperateState(OperateState.SPAWN);
        unitSpawnUI.Select(index);

        selectedUnitUI.UpdateUnitInfoByBtn(units[index], unitDataLoader);

        SoundManager.Instance.PlayUIClickSFX();
    }

    public void CancelSpawn()
    {
        selectedIndex = -1;
        spawn = false;
        indicator.SetActive(false);
        //mouseIndicator.SetActive(false);
        unitSpawnUI.Deselect();
        selectedUnitUI.HideUntInfo();
    }

    // 단축키로 유닛 스폰
    public void OnUnitSpawn(InputAction.CallbackContext context)
    {
        if (dollyCamera.IsCamPanning || !inGameManager.IsGameStart || inGameManager.IsGamgePause)
            return;

        if (selectedUnitManager.SelectedUnit != null )
        {
            selectedUnitManager.DeSelecteUnit();

            //return;
        }

        if (context.performed)
        {
            string keyName = context.control.name;

            if (int.TryParse(keyName, out int keyNumber))
            {
                ToggleSpawnUnit(keyNumber - 1);
            }

        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (inputMng.IsPointerOnUIElements())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag("Tile"))
                {
                    ObjectPoolWithList<AllyUnit> pool = unitPools[selectedIndex];
                    AllyUnit unit = pool.Pool.Get();

                    var allyUnitData = unit.Data as AllyUnitData;

                    if (inGameManager.inGameGold < allyUnitData.Cost)
                    {
                        //--Localization
                        //ingameScreenUI.ShowError("군자금이 모자랍니다!");
                        ingameScreenUI.ShowError("IngameUI", "MSG_noGold");
                        return;
                    }

                    Tile tile = hit.transform.GetComponent<Tile>();
                    if (tile.SetAllyUnit(unit) == null)
                    {
                        ingameScreenUI.ShowError("IngameUI", "MSG_noPlace");
                        return;
                    }

                    // 소환 될 유닛에게 영생 유닛 정보 알려줌
                    if(immortalityEnemy != null)
                        unit.SetImmortalityEnemy(immortalityEnemy);
                    // 유닛의 소환 방향 설정
                    unit.transform.forward = spawnDirection.forward;

                    // 소환진 설정
                    UnitSpawnPoint spawnPoint = spawnPointPool.Pool.Get();

                    UnitGrid unitGrid = unit.UnitGrid.GetComponent<UnitGrid>();
                    unitGrid.SetTargetTile(tile);

                    spawnPoint.transform.position = tile.transform.position; // grid.CellToWorld(grid.WorldToCell(hit.point)) + new Vector3(grid.cellSize.x * 0.5f, 0f, grid.cellSize.y * 0.5f);


                    spawnPoint.gameObject.SetActive(true);
                    SoundManager.Instance.PlaySFX(allySummon, unit.transform.position);
                    spawnPoint.Initialize(unit);


                    inGameManager.SetGold(allyUnitData.Cost, false);
                    ingameScreenUI.SetspawnBtnPriceTextColor();
                }
                else// if(hit.transform.CompareTag("Obstacle"))
                {
                    ingameScreenUI.ShowError("IngameUI", "MSG_noPlace");
                }
            }
        }
    }

    public void StopActivateAlly()
    {
        foreach (var pool in unitPools)
        {
            foreach (var unit in pool.List)
            {
                if (unit != null)
                    unit.StopUnit();
            }
        }

        foreach (var kvp in upgradeUnitPoolsDic)
        {
            foreach (var unit in kvp.Value.List)
            {
                if (unit != null)
                    unit.StopUnit();
            }
        }
    }

    public void SetIdleState(bool isWaveEnd)
    {
        foreach (var pool in unitPools)
        {
            foreach (var unit in pool.List)
            {
                if (unit != null)
                    unit.SetIdleState(isWaveEnd);
            }
        }

        foreach (var kvp in upgradeUnitPoolsDic)
        {
            foreach (var unit in kvp.Value.List)
            {
                if (unit != null)
                    unit.SetIdleState(isWaveEnd);
            }
        }
    }

    // 소환 된 유닛들에게 영생 유닛 정보 전달
    private void NotifyImmortalityEnemyInfo(EnemyUnit immortalityEnemy)
    {
        foreach (var pool in unitPools)
        {
            foreach (var unit in pool.List)
            {
                if (unit != null)
                    unit.SetImmortalityEnemy(immortalityEnemy);
            }
        }

        foreach (var kvp in upgradeUnitPoolsDic)
        {
            foreach (var unit in kvp.Value.List)
            {
                if (unit != null)
                    unit.SetImmortalityEnemy(immortalityEnemy);
            }
        }
    }

    // 영생 유닛이 소환 / 사망 시 호출
    public void SetImmortalityUnit(EnemyUnit ImmortalityEnemy)
    {
        this.immortalityEnemy = ImmortalityEnemy;
        NotifyImmortalityEnemyInfo(ImmortalityEnemy);
    }

    // 상태 적용 시 특수 능력 발동 경우

    // 소환 되어있는 유닛들에게 상태 적용 시 발동되는 능력 이벤트 적용
    private void RegisterGetEffectEvent(AllyUnit newUnit)
    {
        // 새로 소환되는 유닛의 능력이 상태 적용 시 발동되는 능력이면 자기 자신의 이벤트에 자기 능력 등록
        if (newUnit.SpecialAbility != null && newUnit.SpecialAbility.ActiveCondition == SpecialAbility.ActiveType.GET_EFFECT)
            newUnit.onGetDurationEffect += newUnit.SpecialAbility.OnGetDurationEffect;

        foreach (var pool in unitPools)
        {
            foreach (AllyUnit existingUnit in pool.List)
            {
                if (existingUnit != null)
                    RegisterGetEffectEvent(existingUnit, newUnit);
            }
        }

        foreach (var pool in upgradeUnitPoolsDic)
        {
            foreach (var existingUnit in pool.Value.List)
            {
                if (existingUnit != null)
                    RegisterGetEffectEvent(existingUnit, newUnit);
            }
        }
    }

    // 기존 유닛이 상태 적용 시 발동되는 능력을 보유 했으면 새 유닛 이벤트에 기존 유닛의 능력 등록
    private void RegisterGetEffectEvent(AllyUnit existingUnit, AllyUnit newUnit)
    {
        // 새 유닛이 정화 유닛이면 기존 일반 유닛 이벤트에 새 유닛의 능력 등록
        if (newUnit.SpecialAbility != null && newUnit.SpecialAbility.ActiveCondition == SpecialAbility.ActiveType.GET_EFFECT)
        {
            if (existingUnit.SpecialAbility == null || 
                existingUnit.SpecialAbility.ActiveCondition != SpecialAbility.ActiveType.GET_EFFECT)
                existingUnit.onGetDurationEffect += newUnit.SpecialAbility.OnGetDurationEffect;

            return;
        }

        // 새 유닛이 일반 유닛이면 기존 정화 유닛의 능력을 새 유닛 이벤트에 등록
        if (existingUnit.SpecialAbility != null && 
            existingUnit.SpecialAbility.ActiveCondition == SpecialAbility.ActiveType.GET_EFFECT)
            newUnit.onGetDurationEffect += existingUnit.SpecialAbility.OnGetDurationEffect;
    }
}