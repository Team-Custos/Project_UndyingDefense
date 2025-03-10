using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//이 스크립트는 아군 병사를 스폰시키기 위한 스크립트입니다.

public class UnitSpawnManager : MonoBehaviour
{
    public List<Ingame_UnitData> unitDatas;
    [SerializeField]
    private GameObject unitPrefab;

    public static UnitSpawnManager inst;

    public GameObject Test_Ally;

    public UnitType unitToSpawn;

    public Transform SpawnPos;

    int unitPriority = 0;

    // Start is called before the first frame update
    void Start()
    {
        inst = this;
    }

    // 유닛 생성시 3초 딜레이 추가 및 파티클 생성
    public GameObject UnitSpawn(int unitType, float X, float Y)
    {
        InGameManager.inst.gold -= UnitSpawnManager.inst.unitDatas[unitType].cost;

        GameObject tempObject = new GameObject("TempTransform");
        tempObject.transform.position = new Vector3(X,- 0.9f, Y);

        // 파티클 생성
        Ingame_ParticleManager.Instance.PlaySummonParticleEffect(tempObject.transform, true);

        // 파티클 생성 시 타일을 배치 불가능으로 설정
        GridManager.inst.SetTilePlaceable(tempObject.transform.position, true, false);

        StartCoroutine(SpawnUnitAfterDelay(unitType, X, Y, tempObject));

        return null;
    }

    // 유닛 생성 3초 딜레이
    private IEnumerator SpawnUnitAfterDelay(int unitType, float X, float Y, GameObject tempObject)
    {
        yield return new WaitForSeconds(3.0f);

        // 유닛 생성
        GameObject Obj = Instantiate(Test_Ally);
        Obj.transform.position = new Vector3(X, 0, Y);
        Obj.GetComponent<Ingame_UnitCtrl>().unitData = unitDatas[unitType];

        SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_allySpawn);

        //NavMeshAgent Priority 설정.
        Obj.GetComponent<NavMeshAgent>().avoidancePriority = (unitPriority % 50) + 50;
        unitPriority++;
        // 유닛이 생성된 후 타일 상태를 갱신 (배치 불가능 유지)
        GridManager.inst.SetTilePlaceable(Obj.transform.position, true, false);
        Destroy(tempObject);

        yield return Obj;
    }

}