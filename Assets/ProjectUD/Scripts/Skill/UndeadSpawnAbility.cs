using UnityEngine;

public class UndeadSpawnAbility : SpecialAbility
{
    [SerializeField] private EnemyUnitData[] undeadDatas;
    [SerializeField] private float range = 5f;
    [SerializeField] private GameObject spawnVFX;
    private EnemyUnitSpawner enemySpawner;
    private AllyUnitSpawner allySpawner;

    public override void Initialize(Unit unit)
    {
        base.Initialize(unit);
        
        EnemyUnit enemy = unit as EnemyUnit;
        if (enemy != null)
        {
            enemySpawner = enemy.EnemySpawner;
            enemySpawner.onUnitDead += SpawnUndead;

            //allySpawner = enemy.AllySpawner;
            allySpawner = enemy.EnemySpawner.AllySpawner;   // 적 스포너가 갖고 있는 아군 스포너 참조...?
            allySpawner.onUnitDead += SpawnUndead;
        }

    }

    public override void Remove()
    {
        enemySpawner.onUnitDead -= SpawnUndead;
        allySpawner.onUnitDead -= SpawnUndead;
    }

    public void SpawnUndead(Unit target)
    {
        if (target.Data.CampName == "pioneer_su")
            return;

        float distance = Vector3.Distance(unit.transform.position, target.transform.position);

        if (distance > range)
        {
            Debug.Log("범위 밖");
            return;
        }


        int randomIndex = Random.Range(0, undeadDatas.Length);
        EnemyUnitData enemyUnitData = undeadDatas[randomIndex];

        enemySpawner.SpawnEnemy(enemyUnitData, target.transform.position );

        Debug.Log($"{enemyUnitData.Id} : 소환");

        GameObject vfx = target.AddVFX(spawnVFX);
        vfx.transform.position = target.transform.position;
        vfx.SetActive(true);
    }
}
