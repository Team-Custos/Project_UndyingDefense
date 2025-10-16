using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using InputEventInterface;
using UnityEngine.UI;
using UnityEngine.AI;

public class AllyUnitSpawner : MonoBehaviour, IInputClick, IInputUnitSpawn, IInputESC, IInputRightClick
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
    [SerializeField] private DollyCamera dollyCamera;

    [SerializeField] private Image[] alarmImages;

    [Header("■ Units")]
    [SerializeField] private AllyUnitData[] units;

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
        unit.Initialize(data, unitPools[index], this);

        UnitStats unitStats = unitDataLoader.GetUnitDataById(unit.UnitId);
        unit.SetUnitStats(unitStats);

        unit.SetDurationEffectPool(durationEffectPool);
        unit.SetInstantEffectPool(instantEffectPool);
        unit.SetHitVFXPool(hitVFXPool);
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

            //return upgradeUnit;
        }

        UpgradeUnitInitialize(upgradeUnit, allyUnitData, allyUnitPrefab, transform, tile);
        return upgradeUnit;
    }

    private void UpgradeUnitInitialize(AllyUnit upgradeUnit, AllyUnitData allyUnitData, GameObject allyUnitPrefab, Transform transform, Tile tile)
    {
        upgradeUnit.Initialize();
        upgradeUnit.Initialize(allyUnitData, upgradeUnitPoolsDic[allyUnitPrefab], this);
        UnitStats unitStats = unitDataLoader.GetUnitDataById(upgradeUnit.UnitId);
        upgradeUnit.SetUnitStats(unitStats);

        upgradeUnit.SetDurationEffectPool(durationEffectPool);
        upgradeUnit.SetInstantEffectPool(instantEffectPool);
        upgradeUnit.SetHitVFXPool(hitVFXPool);

        upgradeUnit.transform.position = transform.position;
        upgradeUnit.transform.rotation = transform.rotation;

        upgradeUnit.gameObject.SetActive(true);
        upgradeUnit.UnitGrid.SetTargetTile(tile);
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
        if (index == 3)
        {
            SoundManager.Instance.PlayUnableUIClickSFX();
            return;
        }

        if (inGameManager.inGameGold < units[index].Cost)
        {
            ingameScreenUI.ShowError("군자금이 모자랍니다!");
        }

        inputMng.OnESCTarget = this;
        inputMng.OnRightClickTarget = this;

        if (index == selectedIndex)
        {
            CancelSpawn();
            inputMng.OnESCTarget = inGameManager;
            inputMng.OnRightClickTarget = selectedUnitManager;
            inputMng.OnClickTarget = selectedUnitManager;
            selectedUnitUI.HideUntInfo();
        }
        else
        {
            if (index >= 0 && index < alarmImages.Length)
            {
                if (alarmImages[index] != null)
                {
                    alarmImages[index].gameObject.SetActive(false);
                }
            }

            selectedUnitManager.DeSelecteUnit();
            commandSkillManager.CancelSkill();
            selectedIndex = index;
            spawn = true;
            //indicator.SetActive(true);
            inputMng.OnClickTarget = this;
            unitSpawnUI.Select(index);

            //Unit buttonUnit = units[index].Prefab.GetComponent<Unit>();


            selectedUnitUI.UpdateUnitInfoByBtn(units[index], unitDataLoader);
        }

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
        if (dollyCamera.IsCamPanning || inGameManager.IsGameEnd)
            return;

        if (selectedUnitManager.SelectedUnit != null)
            return;

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
                if (!hit.transform.CompareTag("Tile"))
                    return;



                ObjectPoolWithList<AllyUnit> pool = unitPools[selectedIndex];
                AllyUnit unit = pool.Pool.Get();

                var allyUnitData = unit.Data as AllyUnitData;

                if (inGameManager.inGameGold < allyUnitData.Cost)
                {
                    ingameScreenUI.ShowError("군자금이 모자랍니다!");
                    return;
                }

                // 유닛의 소환 방향 설정
                unit.transform.forward = spawnDirection.forward;

                // 소환진 설정
                UnitSpawnPoint spawnPoint = spawnPointPool.Pool.Get();


                Tile tile = hit.transform.GetComponent<Tile>();
                if (tile.SetAllyUnit(unit) == null)
                {
                    ingameScreenUI.ShowError("그곳엔 장애물이 있습니다.");
                    return;
                }
                    
                   

                UnitGrid unitGrid = unit.UnitGrid.GetComponent<UnitGrid>();
                unitGrid.SetTargetTile(tile);

                spawnPoint.transform.position = tile.transform.position; // grid.CellToWorld(grid.WorldToCell(hit.point)) + new Vector3(grid.cellSize.x * 0.5f, 0f, grid.cellSize.y * 0.5f);



                //Vector3Int cellPos = grid.WorldToCell(hit.point);
                //Vector3 center = grid.GetCellCenterWorld(cellPos);

                //if (!gridManager.OccupiedGridDic.ContainsKey(grid.GetCellCenterWorld(cellPos)))
                //{
                //    Debug.Log(grid.GetCellCenterWorld(cellPos));
                //    gridManager.OccupiedGridDic.Add(grid.GetCellCenterWorld(cellPos), true);
                //}

                //Debug.Log(grid.WorldToCell(hit.point));
                spawnPoint.gameObject.SetActive(true);
                SoundManager.Instance.PlaySFX(allySummon);
                spawnPoint.Initialize(unit);

                
                //unit.Setpriority(priority);
                //if (priority > 100)
                //{
                //    priority = 51; // 우선순위 초기화
                //}
                //else
                //    priority++;

                inGameManager.SetGold(allyUnitData.Cost, false);
                ingameScreenUI.SetspawnBtnPriceTextColor();
                    
            }
        }
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CancelSpawn();
            inputMng.OnESCTarget = inGameManager;
            inputMng.OnRightClickTarget = selectedUnitManager;
            inputMng.OnClickTarget = selectedUnitManager;
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            CancelSpawn();
            inputMng.OnESCTarget = inGameManager;
            inputMng.OnRightClickTarget = selectedUnitManager;
            inputMng.OnClickTarget = selectedUnitManager;
        }
    }

    public void ResetAllyUnitRotation(AllyUnit allyUnit)
    {
        //allyUnit.transform.forward = spawnDirection.forward;

        Vector3 direction = spawnDirection.forward;
        Quaternion rot = Quaternion.LookRotation(direction);
        allyUnit.transform.rotation = Quaternion.Slerp(allyUnit.transform.rotation, rot, Time.deltaTime * 10.0f);

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
}