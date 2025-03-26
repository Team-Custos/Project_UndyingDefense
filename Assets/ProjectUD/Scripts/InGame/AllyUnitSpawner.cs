using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using InputEventInterface;
using UnityEngine.UIElements;
using static UnitDataManager;

public class AllyUnitSpawner : MonoBehaviour, IInputClick
{
    [Header("■ Components")]
    [SerializeField] private PlayerInputEventManager inputMng;
    [SerializeField] private Grid grid;
    [SerializeField] private Transform spawnDirection;

    [Header("■ Units")]
    [SerializeField] private AllyUnitData[] units;

    [Header("■ Spawn Point")]
    [SerializeField] private GameObject spawnPointPrefab;

    [Header("■ UI")]
    [SerializeField] private UnitSpawnUI unitSpawnUI;
    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject mouseIndicator;

    [Header("■ Ground Layer")]
    [SerializeField] private LayerMask groundLayer;

    private int selectedIndex; // 현재 선택된 소환 가능한 유닛의 인덱스(-1 : 선택되지 않음)
    private bool spawn;
    private List<ObjectPoolWithList<AllyUnit>> unitPools;
    private ObjectPoolWithList<UnitSpawnPoint> spawnPointPool;

    private Dictionary<GameObject, List<AllyUnit>> upgradeUnitPoolsDic = new Dictionary<GameObject, List<AllyUnit>>(); // 업그레이드 유닛을 담을 풀

    [SerializeField] UnitClickDetector unitClickDector;

    private void Start()
    {
        selectedIndex = -1;
        unitPools = new List<ObjectPoolWithList<AllyUnit>>();
        for(int i = 0; i < units.Length; i++)
        {
            int index = i;
            unitPools.Add(new ObjectPoolWithList<AllyUnit>(() => CreateUnit(index)));
            spawnPointPool = new ObjectPoolWithList<UnitSpawnPoint>(CreateSpawnPoint);

            // UI 설정
            AllyUnitData data = units[i];
            unitSpawnUI.SetSpawnButton(i, data.Icon, data.Tier, (int)data.Cost);
        }
    }

    private void Update()
    {
        if(spawn)
        {
            if (inputMng.IsPointerOnUIElements())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!hit.transform.CompareTag("Ground"))
                    return;

                indicator.transform.position = grid.CellToWorld(grid.WorldToCell(hit.point)) + new Vector3(grid.cellSize.x * 0.5f, 0f, grid.cellSize.y * 0.5f);
                mouseIndicator.transform.position = hit.point;
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

        return unit;
    }

    public AllyUnit CreatUpgradeUnit(GameObject allyUnitPrefab, AllyUnitData allyUnitData, Transform transform)
    {
        GameObject obj;

        // 해당 유닛의 값이 있는지 확인
        if (upgradeUnitPoolsDic.TryGetValue(allyUnitPrefab, out List<AllyUnit> unitPool))
        {
            if(unitPool.Count <= 0) // 풀이 비어있는지 확인
            { 
                obj = Instantiate(allyUnitPrefab);
                obj.SetActive(false);

                AllyUnit upgradeUnit = obj.GetComponent<AllyUnit>();
                upgradeUnit.gameObject.SetActive(true);

                upgradeUnit.transform.position = transform.position;
                upgradeUnit.transform.rotation = transform.rotation;

                return upgradeUnit;
            }
            else // 풀에 유닛 있음
            {
                AllyUnit upgradeUnit = unitPool[0];
                unitPool.RemoveAt(0);
                upgradeUnit.gameObject.SetActive(true);

                upgradeUnit.transform.position = transform.position;
                upgradeUnit.transform.rotation = transform.rotation;

                return upgradeUnit;
            }
        }
        else
        {
            List<AllyUnit> newPool = new List<AllyUnit>();
            upgradeUnitPoolsDic.Add(allyUnitPrefab, newPool);

            obj = Instantiate(allyUnitPrefab);
            obj.SetActive(false);

            AllyUnit upgradeUnit = obj.GetComponent<AllyUnit>();
            upgradeUnit.gameObject.SetActive(true);

            upgradeUnit.transform.position = transform.position;
            upgradeUnit.transform.rotation = transform.rotation;

            return upgradeUnit;
        }
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
        if(index == selectedIndex)
        {
            CancelSpawn();
        }
        else
        {
            selectedIndex = index;
            spawn = true;
            indicator.SetActive(true);
            mouseIndicator.SetActive(true);
            inputMng.OnClickTarget = this;
            unitSpawnUI.Select(index);
        }
    }

    private void CancelSpawn()
    {
        selectedIndex = -1;
        spawn = false;
        indicator.SetActive(false);
        mouseIndicator.SetActive(false);
        inputMng.OnClickTarget = unitClickDector; //  null;
        unitSpawnUI.Deselect();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            if (inputMng.IsPointerOnUIElements())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if(Physics.Raycast(ray, out RaycastHit hit))
            {
                if (!hit.transform.CompareTag("Ground"))
                    return;

                ObjectPoolWithList<AllyUnit> pool = unitPools[selectedIndex];
                AllyUnit unit = pool.Pool.Get();               
                // 유닛의 소환 방향 설정
                unit.transform.forward = spawnDirection.forward;

                // 소환진 설정
                UnitSpawnPoint spawnPoint = spawnPointPool.Pool.Get();
                spawnPoint.transform.position = grid.CellToWorld(grid.WorldToCell(hit.point)) + new Vector3(grid.cellSize.x * 0.5f, 0f, grid.cellSize.y * 0.5f);
                spawnPoint.gameObject.SetActive(true);
                spawnPoint.Initialize(unit);

                CancelSpawn();
            }
        }
    }
}
