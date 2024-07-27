using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
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

    public Button FreeBtn = null;
    public Button SiegeBtn = null;

    public GameObject UnitStateCheckBox;
    private GameObject currentUnitStateCheckBox; // 현재 생성된 체크박스를 참조할 변수
    private Vector3 currentSpawnPosition;

    public GameObject MinByeongPrefab;
    public GameObject HunterPrefab;

    private Camera mainCamera;

    public Transform SpawnPos;
    private bool isSpawnBtnClick = false;

    private string UnitType;

    defaultUnitState defaultunitState;

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

        defaultunitState = defaultUnitState.Free;

        if (FreeBtn != null)
        {
            FreeBtn.onClick.AddListener(() => { FreeState(); RemoveUnitStateCheckBox(); });
        }

        if (SiegeBtn != null)
        {
            SiegeBtn.onClick.AddListener(() => { SiegeState(); RemoveUnitStateCheckBox(); });
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (isSpawnBtnClick)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("Ground"))
                    {
                        currentSpawnPosition = hit.point;
                        CreateUnitStateCheckBox(currentSpawnPosition);
                        isSpawnBtnClick = false;
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

    public void MinByeongSpawn(Vector3 spawnPosition)
    {
        GameObject MinByeon = Instantiate(MinByeongPrefab) as GameObject;
        MinByeon.transform.position = spawnPosition;
    }

    public void HunterSpawn(Vector3 spawnPosition)
    {
        GameObject Hunter = Instantiate(HunterPrefab) as GameObject;
        Hunter.transform.position = spawnPosition;
    }



    void OnButtonClicked(string unitType)
    {
        isSpawnBtnClick = true;
        UnitType = unitType;
        Debug.Log($"Button clicked: {unitType}");
    }


    void FreeState()
    {
        defaultunitState = defaultUnitState.Free;
        SpawnSelectedUnit();
        RemoveUnitStateCheckBox();
    }

    void SiegeState()
    {
        defaultunitState = defaultUnitState.Siege;
        SpawnSelectedUnit();
        RemoveUnitStateCheckBox();
    }


    void CreateUnitStateCheckBox(Vector3 worldPosition)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

        if (currentUnitStateCheckBox != null)
        {
            Destroy(currentUnitStateCheckBox);
        }

        currentUnitStateCheckBox = Instantiate(UnitStateCheckBox) as GameObject;

        GameObject canvas = GameObject.Find("Canvas");

        currentUnitStateCheckBox.transform.SetParent(canvas.transform, false);

        RectTransform rectTransform = currentUnitStateCheckBox.GetComponent<RectTransform>();
        rectTransform.position = screenPos;

        Button freeBtn = currentUnitStateCheckBox.transform.Find("FreeBtn").GetComponent<Button>();
        Button siegeBtn = currentUnitStateCheckBox.transform.Find("SiegeBtn").GetComponent<Button>();

        freeBtn.onClick.AddListener(() => FreeState());
        siegeBtn.onClick.AddListener(() => SiegeState());
    }

    void RemoveUnitStateCheckBox()
    {
        if (currentUnitStateCheckBox != null)
        {
            Destroy(currentUnitStateCheckBox);
            currentUnitStateCheckBox = null;
        }
    }

    void SpawnSelectedUnit()
    {
        if (UnitType == "MinByeong")
        {
            MinByeongSpawn(currentSpawnPosition);
        }
        else if (UnitType == "Hunter")
        {
            HunterSpawn(currentSpawnPosition);
        }
    }
}
