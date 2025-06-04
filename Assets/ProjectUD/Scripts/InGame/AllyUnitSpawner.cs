using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using InputEventInterface;
using UnityEngine.UIElements;

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

    public AllyUnit CreateUpgradeUnit(GameObject allyUnitPrefab, AllyUnitData allyUnitData, Transform transform, AllyUnit.Mode mode)
    {
        GameObject obj;

        // 해당 유닛의 값이 있는지 확인
        if (upgradeUnitPoolsDic.ContainsKey(allyUnitPrefab))
        {
            if (upgradeUnitPoolsDic[allyUnitPrefab].List.Count <= 0)
            {
                obj = Instantiate(allyUnitPrefab);
                obj.SetActive(false);

                AllyUnit upgradeUnit = obj.GetComponent<AllyUnit>();

                upgradeUnit.Initialize(allyUnitData, upgradeUnitPoolsDic[allyUnitPrefab], this);
                upgradeUnit.previousMode = mode;
                upgradeUnit.UpgradeInitialize();

                upgradeUnit.IsSelected = true;

                upgradeUnit.gameObject.SetActive(true);

                upgradeUnitPoolsDic[allyUnitPrefab].List.Add(upgradeUnit);

                upgradeUnit.transform.position = transform.position;
                upgradeUnit.transform.rotation = transform.rotation;

                upgradeUnit.IsSelected = true;

                selectedUnitManager.SetSelectedUnit(upgradeUnit);
                selectedUnitUI.UpdateUnitInfo(upgradeUnit);



                return upgradeUnit;
            }
            else
            {

                AllyUnit upgradeUnit = upgradeUnitPoolsDic[allyUnitPrefab].Pool.Get();

                upgradeUnit.Initialize(allyUnitData, upgradeUnitPoolsDic[allyUnitPrefab], this);
                upgradeUnit.previousMode = mode;
                upgradeUnit.UpgradeInitialize();

                upgradeUnit.IsSelected = true;

                selectedUnitManager.SetSelectedUnit(upgradeUnit);
                upgradeUnit.gameObject.SetActive(true);


                upgradeUnit.transform.position = transform.position;
                upgradeUnit.transform.rotation = transform.rotation;

                upgradeUnitPoolsDic[allyUnitPrefab].List.Add(upgradeUnit);

                

                selectedUnitUI.UpdateUnitInfo(upgradeUnit);

                return upgradeUnit;
            }
        }
        else
        {
            upgradeUnitPoolsDic.Add(allyUnitPrefab, new ObjectPoolWithList<AllyUnit>(() =>
            {
                GameObject obj = Instantiate(allyUnitPrefab);
                obj.SetActive(false);

                AllyUnit upgradeUnit = obj.GetComponent<AllyUnit>();
                return upgradeUnit;
            }));

            AllyUnit upgradeUnit = upgradeUnitPoolsDic[allyUnitPrefab].Pool.Get();

            upgradeUnit.Initialize(allyUnitData, upgradeUnitPoolsDic[allyUnitPrefab], this);
            upgradeUnit.previousMode = mode;
            upgradeUnit.UpgradeInitialize();
            //upgradeUnit.ModeType = upgradeUnit.PreviousMode;
            selectedUnitManager.SetSelectedUnit(upgradeUnit);
            upgradeUnit.IsSelected = true;

            upgradeUnit.gameObject.SetActive(true);
            selectedUnitUI.ShowAllyUI(upgradeUnit);


            upgradeUnit.transform.position = transform.position;
            upgradeUnit.transform.rotation = transform.rotation;

            upgradeUnitPoolsDic[allyUnitPrefab].List.Add(upgradeUnit);

            selectedUnitUI.UpdateUnitInfo(upgradeUnit);

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
        if(inGameManager.inGameGold < units[index].Cost)
        {
            ingameScreenUI.ShowError("군자금이 모자랍니다!");
            return;
        }

        inputMng.OnESCTarget = this;
        inputMng.OnRightClickTarget = this;

        if (index == selectedIndex)
        {
            CancelSpawn();
            selectedUnitUI.HideUntInfo();
        }
        else
        {
            selectedUnitManager.DeSelecteUnit();
            commandSkillManager.CancelSkill();
            selectedIndex = index;
            spawn = true;
            indicator.SetActive(true);
            mouseIndicator.SetActive(true);
            inputMng.OnClickTarget = this;
            unitSpawnUI.Select(index);

            //Unit buttonUnit = units[index].Prefab.GetComponent<Unit>();


            selectedUnitUI.UpdateUnitInfoByBtn(units[index]);
        }

        SoundManager.Instance.PlayUIClickSFX();
    }

    public void CancelSpawn()
    {
        selectedIndex = -1;
        spawn = false;
        indicator.SetActive(false);
        mouseIndicator.SetActive(false);
        inputMng.OnClickTarget = selectedUnitManager; //  null;
        unitSpawnUI.Deselect();
    }

    // 단축키로 유닛 스폰
    public void OnUnitSpawn(InputAction.CallbackContext context)
    {
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

                inGameManager.SetGold(allyUnitData.Cost, false);
                ingameScreenUI.SetspawnBtnPriceTextColor();

                if (inGameManager.inGameGold < allyUnitData.Cost)
                    CancelSpawn();
            }
        }
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CancelSpawn();
            selectedUnitUI.HideUntInfo();
            inputMng.OnESCTarget = inGameManager;
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            CancelSpawn();
            selectedUnitUI.HideUntInfo();
            inputMng.OnRightClickTarget = selectedUnitManager;
        }
    }
}