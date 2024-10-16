using System.Collections.Generic;
using UnityEngine;



public enum UnitType
{
    MinByeong,
    Hunter,
    SpearMan,
    Archer,
}


public class UnitSpawnManager : MonoBehaviour
{
    public List<Ingame_UnitData> unitDatas;
    [SerializeField]
    private GameObject unitPrefab;

    public static UnitSpawnManager inst;

    public GameObject Test_Ally;

    public int unitToSpawn = 0;

    public Transform SpawnPos;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
    }
    // Update is called once per frame
    void Update()
    {
        Ingame_UnitCtrl[] allUnits = FindObjectsOfType<Ingame_UnitCtrl>();
        foreach (Ingame_UnitCtrl unit in allUnits)
        {
            if (unit.unitStateChangeTime > 0)
            {
                unit.unitStateChangeTime -= Time.deltaTime;
            }
        }
    }

    //유닛 소환
    public GameObject UnitSpawn(int unitType, float X, float Y)
    {
        GameObject Obj;

        //Debug.Log(new Vector3(X, 0, Y));

        Obj = Instantiate(Test_Ally);
        Obj.transform.position = new Vector3(X, 0, Y);
        //Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2(X, Y);
        Obj.GetComponent<Ingame_UnitCtrl>().unitData = unitDatas[unitType];

        return Obj;
    }
}