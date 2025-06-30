using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using InputEventInterface;
using UnityEngine.UI;

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
<<<<<<< HEAD
    [SerializeField] private UnitDataLoader unitDataLoader;
=======
<<<<<<< Updated upstream
=======
    [SerializeField] private UnitDataLoader unitDataLoader;
    [SerializeField] private DurationEffectPool durationEffectPool;
>>>>>>> Stashed changes
>>>>>>> KimJK

    [SerializeField] private Image[] alarmImages;

    [Header("■ Units")]
    [SerializeField] private AllyUnitData[] units;

    [Header("■ Spawn Point")]
    [SerializeField] private GameObject spawnPointPrefab;

    [Header("■ UI")]
    [SerializeField] private UnitSpawnUI unitSpawnUI;
    [SerializeField] private GameObject indicator;
<<<<<<< HEAD
    [SerializeField] private GameObject mouseIndicator;

    
=======
<<<<<<< Updated upstream
    //[SerializeField] private GameObject mouseIndicator;
=======
    [SerializeField] private GameObject mouseIndicator;

    
>>>>>>> Stashed changes
>>>>>>> KimJK

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
<<<<<<< HEAD
            //Unit unit = units[i].Prefab.GetComponent<Unit>();
=======
<<<<<<< Updated upstream
=======
            //Unit unit = units[i].Prefab.GetComponent<Unit>();
>>>>>>> Stashed changes
>>>>>>> KimJK

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

<<<<<<< HEAD
=======
<<<<<<< Updated upstream
                indicator.transform.position = 
                    grid.CellToWorld(grid.WorldToCell(hit.point)) + new Vector3(grid.cellSize.x * 0.5f, 0f, grid.cellSize.y * 0.5f) 
                    + Vector3.up * 0.01f;
                //mouseIndicator.transform.position = hit.point;
            }
        }
    }
=======
>>>>>>> KimJK
                indicator.transform.position = grid.CellToWorld(grid.WorldToCell(hit.point)) + new Vector3(grid.cellSize.x * 0.5f, 0f, grid.cellSize.y * 0.5f);
                mouseIndicator.transform.position = hit.point;
            }
        }
    }

<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> KimJK
    private AllyUnit CreateUnit(int index)
    {
        AllyUnitData data = units[index];
        GameObject obj = Instantiate(data.Prefab);
        obj.SetActive(false);
        AllyUnit unit = obj.GetComponent<AllyUnit>();
        unit.Initialize(data, unitPools[index], this);
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
=======
        unit.SetDurationEffectPool(durationEffectPool);

>>>>>>> Stashed changes
>>>>>>> KimJK
        return unit;
    }

    public AllyUnit CreateUpgradeUnit(GameObject allyUnitPrefab, AllyUnitData allyUnitData, Transform transform, AllyUnit.Mode mode,
        Tile tile)
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
<<<<<<< HEAD
                upgradeUnit.previousMode = mode;
                upgradeUnit.SetUnitDataLoader(unitDataLoader);
=======
<<<<<<< Updated upstream
                upgradeUnit.previousMode = mode;
                //upgradeUnit.UnitGrid.SetTargetTile(tile);
=======
                upgradeUnit.SetDurationEffectPool(durationEffectPool);
                upgradeUnit.previousMode = mode;
                upgradeUnit.SetUnitDataLoader(unitDataLoader);
>>>>>>> Stashed changes
>>>>>>> KimJK
                upgradeUnit.UpgradeInitialize();

                upgradeUnit.IsSelected = true;

                upgradeUnit.gameObject.SetActive(true);
                upgradeUnit.UnitGrid.SetTargetTile(tile);

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
<<<<<<< HEAD
                upgradeUnit.previousMode = mode;
                upgradeUnit.SetUnitDataLoader(unitDataLoader);
=======
<<<<<<< Updated upstream
                upgradeUnit.previousMode = mode;
=======
                upgradeUnit.SetDurationEffectPool(durationEffectPool);
                upgradeUnit.previousMode = mode;
                upgradeUnit.SetUnitDataLoader(unitDataLoader);
>>>>>>> Stashed changes
>>>>>>> KimJK
                upgradeUnit.UpgradeInitialize();

                upgradeUnit.IsSelected = true;

                selectedUnitManager.SetSelectedUnit(upgradeUnit);
                upgradeUnit.gameObject.SetActive(true);
                upgradeUnit.UnitGrid.SetTargetTile(tile);


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
<<<<<<< HEAD
            upgradeUnit.previousMode = mode;
            upgradeUnit.SetUnitDataLoader(unitDataLoader);
=======
<<<<<<< Updated upstream
            upgradeUnit.previousMode = mode;
=======
            upgradeUnit.SetDurationEffectPool(durationEffectPool);
            upgradeUnit.previousMode = mode;
            upgradeUnit.SetUnitDataLoader(unitDataLoader);
>>>>>>> Stashed changes
>>>>>>> KimJK
            upgradeUnit.UpgradeInitialize();
            //upgradeUnit.ModeType = upgradeUnit.PreviousMode;
            selectedUnitManager.SetSelectedUnit(upgradeUnit);
            upgradeUnit.IsSelected = true;

<<<<<<< HEAD
            
=======
<<<<<<< Updated upstream
=======
            
>>>>>>> Stashed changes
>>>>>>> KimJK
            upgradeUnit.gameObject.SetActive(true);
            upgradeUnit.UnitGrid.SetTargetTile(tile);
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
            indicator.SetActive(true);
<<<<<<< HEAD
            mouseIndicator.SetActive(true);
=======
<<<<<<< Updated upstream
            //mouseIndicator.SetActive(true);
=======
            mouseIndicator.SetActive(true);
>>>>>>> Stashed changes
>>>>>>> KimJK
            inputMng.OnClickTarget = this;
            unitSpawnUI.Select(index);

            //Unit buttonUnit = units[index].Prefab.GetComponent<Unit>();


<<<<<<< HEAD
            selectedUnitUI.UpdateUnitInfoByBtn(units[index], unitDataLoader);
=======
<<<<<<< Updated upstream
            selectedUnitUI.UpdateUnitInfoByBtn(units[index]);
=======
            selectedUnitUI.UpdateUnitInfoByBtn(units[index], unitDataLoader);
>>>>>>> Stashed changes
>>>>>>> KimJK
        }

        SoundManager.Instance.PlayUIClickSFX();
    }

    public void CancelSpawn()
    {
        selectedIndex = -1;
        spawn = false;
        indicator.SetActive(false);
<<<<<<< HEAD
        mouseIndicator.SetActive(false);
=======
<<<<<<< Updated upstream
        //mouseIndicator.SetActive(false);
=======
        mouseIndicator.SetActive(false);
>>>>>>> Stashed changes
>>>>>>> KimJK
        unitSpawnUI.Deselect();
        selectedUnitUI.HideUntInfo();
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

                if (inGameManager.inGameGold < allyUnitData.Cost)
                {
                    ingameScreenUI.ShowError("군자금이 모자랍니다!");
                    return;
                    //CancelSpawn();
                    //inputMng.OnESCTarget = inGameManager;
                    //inputMng.OnRightClickTarget = selectedUnitManager;
                    //inputMng.OnClickTarget = selectedUnitManager;
                }

<<<<<<< HEAD
                unit.SetUnitDataLoader(unitDataLoader);

=======
<<<<<<< Updated upstream
=======
                unit.SetUnitDataLoader(unitDataLoader);

>>>>>>> Stashed changes
>>>>>>> KimJK

                // 유닛의 소환 방향 설정
                unit.transform.forward = spawnDirection.forward;

                // 소환진 설정
                UnitSpawnPoint spawnPoint = spawnPointPool.Pool.Get();

<<<<<<< HEAD

=======
<<<<<<< Updated upstream
=======

>>>>>>> Stashed changes
>>>>>>> KimJK
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
                {
                    ingameScreenUI.ShowError("군자금이 모자랍니다!");
                    //CancelSpawn();
                    //inputMng.OnESCTarget = inGameManager;
                    //inputMng.OnRightClickTarget = selectedUnitManager;
                    //inputMng.OnClickTarget = selectedUnitManager;
                }
                    
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
}