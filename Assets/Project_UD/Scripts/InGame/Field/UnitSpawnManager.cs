using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum UnitType
{
    MinByeong,
    Hunter,
    SpearMan,
    Archer,
    Gunner,
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
        //foreach (Ingame_UnitCtrl unit in allUnits)
        //{
        //    if (unit.unitStateChangeTime > 0)
        //    {
        //        unit.unitStateChangeTime -= Time.deltaTime;
        //    }
        //}
    }

    //유닛 소환
    //public GameObject UnitSpawn(int unitType, float X, float Y)
    //{
    //    GameObject Obj;

    //    //Debug.Log(new Vector3(X, 0, Y));

    //    Obj = Instantiate(Test_Ally);
    //    Obj.transform.position = new Vector3(X, 0, Y);
    //    //Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2(X, Y);
    //    Obj.GetComponent<Ingame_UnitCtrl>().unitData = unitDatas[unitType];
    //    Ingame_ParticleManager.Instance.PlaySummonParticleEffect(Obj.transform, true);
    //    return Obj;
    //}

    // 유닛 생성시 3초 딜레이 추가 및 파티클 생성
    public GameObject UnitSpawn(int unitType, float X, float Y)
    {
        GameObject tempObject = new GameObject("TempTransform");
        tempObject.transform.position = new Vector3(X, 0, Y);

        Ingame_ParticleManager.Instance.PlaySummonParticleEffect(tempObject.transform, true);

        StartCoroutine(SpawnUnitAfterDelay(unitType, X, Y, tempObject));
        return null;
    }

    // 유닛 생성 3초 딜레이
    private IEnumerator SpawnUnitAfterDelay(int unitType, float X, float Y, GameObject tempObject)
    {
        yield return new WaitForSeconds(3.0f);

        GameObject Obj = Instantiate(Test_Ally);
        Obj.transform.position = new Vector3(X, 0, Y);
        Obj.GetComponent<Ingame_UnitCtrl>().unitData = unitDatas[unitType];

        Destroy(tempObject);

        yield return Obj;
    }
}