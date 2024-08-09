using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]

public class UnitSpawnData
{
    public int modelType;
    public float spawnTime;
    public int HP;
    public float speed;
    public UnitType unitType;

}

public enum UnitType
{
    Basic,
    what
}


public class UD_Ingame_UnitSpawnManager : MonoBehaviour
{
    UD_Ingame_GridManager GRIDMANAGER;
    public static UD_Ingame_UnitSpawnManager inst;

    public int unitType = 0;
    public UnitSpawnData[] spawnData;

    public GameObject Test_Ally;
    public GameObject Test_Enemy;

    public Button MinByeongBtn = null;
    public Button HunterBtn = null;


    public GameObject UnitStateChangeBox;
    private GameObject currentUnitStateChangeBox;
    private Vector3 currentSpawnPosition;

    public GameObject MinByeongPrefab;
    public GameObject HunterPrefab;

    private Camera mainCamera;

    public Canvas canvas;

    public Transform SpawnPos;
    private bool isSpawnBtnClick = false;

    private string UnitType;

    private AllyMode allyMode;

    private UD_Ingame_UnitCtrl selectedUnit;

    public void SetSelectedUnit(UD_Ingame_UnitCtrl unit)
    {
        selectedUnit = unit;
    }

    // 선택된 유닛을 반환
    public UD_Ingame_UnitCtrl GetSelectedUnit()
    {
        return selectedUnit;
    }


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        GRIDMANAGER = UD_Ingame_GameManager.inst.gridManager;
        inst = this;

        if(MinByeongBtn != null )
        {
            MinByeongBtn.onClick.AddListener(() => OnButtonClicked("MinByeong"));
        }

        if (HunterBtn != null)
        {
            HunterBtn.onClick.AddListener(() => OnButtonClicked("Hunter"));
        }

        allyMode = AllyMode.Siege;

    }
    // Update is called once per frame
    void Update()
    {
        UD_Ingame_UnitCtrl[] allUnits = FindObjectsOfType<UD_Ingame_UnitCtrl>();
        foreach (UD_Ingame_UnitCtrl unit in allUnits)
        {
            if (unit.unitStateChangeTime > 0)
            {
                unit.unitStateChangeTime -= Time.deltaTime;
            }
        }

        DeleteSlectedUnit();

        //if (isSpawnBtnClick)
        //{
        //    if (Input.GetMouseButtonDown(0))
        //    {
        //        if (EventSystem.current.IsPointerOverGameObject()) return;

        //        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        //        if (Physics.Raycast(ray, out RaycastHit hit))
        //        {
        //            if (hit.collider.CompareTag("Ground") || hit.collider.CompareTag("Tile"))
        //            {
        //                currentSpawnPosition = hit.point;
        //                SpawnSelectedUnit();
        //                isSpawnBtnClick = false;
        //                RemoveUnitStateChangeBox();
        //            }
        //        }
        //    }
        //}
        if (isSpawnBtnClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (EventSystem.current.IsPointerOverGameObject()) return;

                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    //if (hit.collider.CompareTag("Ground"))
                    //{
                    //    currentSpawnPosition = hit.point;
                    //    SpawnSelectedUnit();
                    //    isSpawnBtnClick = false;
                    //    RemoveUnitStateChangeBox();
                    //}



                    if (hit.collider.CompareTag("Tile"))
                    {
                        UD_Ingame_GridTile gridTile = hit.collider.GetComponent<UD_Ingame_GridTile>();

                        if (gridTile != null && gridTile.IsPlaceable())
                        {
                            currentSpawnPosition = hit.point;
                            SpawnSelectedUnit(gridTile);
                            isSpawnBtnClick = false;
                            RemoveUnitStateChangeBox();
                        }
                    }
                }
            }
        }
        else if(Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Unit"))
                {
                    UD_Ingame_UnitCtrl unitCtrl = hit.collider.gameObject.GetComponent<UD_Ingame_UnitCtrl>();

                   // Debug.Log(selectedUnit.name);

                    if (unitCtrl.unitStateChangeTime <= 0)
                    {
                        CreateUnitStateChangeBox(hit.point, unitCtrl);
                        SetSelectedUnit(unitCtrl); // 선택된 유닛을 설정
                    }
                }
            }
        }

        
    }

    //유닛 소환
    public GameObject UnitSpawn(bool IsAlly , float X, float Y)
    {
        GameObject Obj = null;

        if (IsAlly)
        {
            Obj = Instantiate(Test_Ally);
            Obj.transform.position = new Vector3(X, 0, Y);
            Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2 ((int)(X/2), (int)(Y/2));
            
        }
        else
        {
            Obj = Instantiate(Test_Enemy);
            Obj.transform.position = new Vector3(X, 0, Y);
            Obj.GetComponent<UD_Ingame_UnitCtrl>().Init(spawnData[unitType]);
        }

        return Obj;
    }

    public void MinByeongSpawn(Vector3 spawnPosition, UD_Ingame_GridTile gridTile)
    {
        Vector3 tileCenter = gridTile.transform.position;
        GameObject MinByeong = Instantiate(MinByeongPrefab, tileCenter, Quaternion.identity);
        MinByeong.GetComponent<UD_Ingame_UnitCtrl>().Ally_Mode = AllyMode.Siege;
        MinByeong.GetComponent<UD_Ingame_UnitCtrl>().unitName = "민병";
        gridTile.currentPlacedUnit = MinByeong;
        gridTile.SetPlaceable(false);
    }

    public void HunterSpawn(Vector3 spawnPosition, UD_Ingame_GridTile gridTile)
    {
        Vector3 tileCenter = gridTile.transform.position;
        GameObject Hunter = Instantiate(HunterPrefab, tileCenter, Quaternion.identity);
        Hunter.GetComponent<UD_Ingame_UnitCtrl>().Ally_Mode = AllyMode.Siege;
        Hunter.GetComponent<UD_Ingame_UnitCtrl>().unitName = "사냥꾼";
        gridTile.currentPlacedUnit = Hunter;
        gridTile.SetPlaceable(false);
    }

    void OnButtonClicked(string unitType)
    {
        isSpawnBtnClick = true;
        UnitType = unitType;

        // 타일 색상 업데이트
        UD_Ingame_GridTile[] allTiles = FindObjectsOfType<UD_Ingame_GridTile>();
        foreach (var tile in allTiles)
        {
            tile.ShowPlacementColors(true);
        }
    }


    //void FreeState()
    //{
    //    SpawnSelectedUnit();
    //    RemoveUnitStateChangeBox();
    //}

    //void SiegeState()
    //{
    //    SpawnSelectedUnit();
    //    RemoveUnitStateChangeBox();
    //}


    void CreateUnitStateChangeBox(Vector3 worldPosition, UD_Ingame_UnitCtrl selectedUnit)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

        if (currentUnitStateChangeBox != null)
        {
            Destroy(currentUnitStateChangeBox);
        }

        currentUnitStateChangeBox = Instantiate(UnitStateChangeBox) as GameObject;

        GameObject canvas = GameObject.Find("Canvas");
        currentUnitStateChangeBox.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = currentUnitStateChangeBox.GetComponent<RectTransform>();
        screenPos.x += 180;
        screenPos.y -= 90;

        rectTransform.position = screenPos;

        Button changeStateBtn = currentUnitStateChangeBox.transform.Find("ChangeStateBtn").GetComponent<Button>();

        // ChangeState 버튼 클릭 시 동작 추가
        changeStateBtn.onClick.AddListener(() =>
        {
            selectedUnit.previousAllyMode = selectedUnit.Ally_Mode;  // 현재 상태를 이전 상태로 저장

            // 상태 변경 시간을 현재 시간으로 업데이트
            selectedUnit.unitStateChangeTime = 10.0f;
            selectedUnit.Ally_Mode = AllyMode.Change;  // Change 상태로 설정

            RemoveUnitStateChangeBox();
        });
    }

    void RemoveUnitStateChangeBox()
    {
        if (currentUnitStateChangeBox != null)
        {
            Destroy(currentUnitStateChangeBox);
            currentUnitStateChangeBox = null;
        }
    }

    void SpawnSelectedUnit(UD_Ingame_GridTile gridTile)
    {
        if (UnitType == "MinByeong")
        {
            MinByeongSpawn(gridTile.transform.position, gridTile);
        }
        else if (UnitType == "Hunter")
        {
            HunterSpawn(gridTile.transform.position, gridTile);
        }

        UD_Ingame_GridTile[] allTiles = FindObjectsOfType<UD_Ingame_GridTile>();
        foreach (var tile in allTiles)
        {
            tile.ShowPlacementColors(false);
        }
    }

    void DeleteSlectedUnit()
    {   
        if (selectedUnit != null && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Delete Unit");

            Destroy(selectedUnit);
            selectedUnit = null;
        }
    }
}
