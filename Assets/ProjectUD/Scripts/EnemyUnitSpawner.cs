using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitSpawner : MonoBehaviour
{
    [Header("■ Components")]
    [SerializeField] private Fortress fortress;

    [Header("■ Options")]
    [SerializeField] private float spawnTime;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform spawnDirection;

    private float spawnTimeCheck;

    [SerializeField] private WaveData waveData;

    private int spawnDataIndex; // 현재 EnemySpawnData의 인덱스
    private int spawnDataEnemyCount; // 현재 EnemySpawnData의 스폰 횟수.
    private bool isSpawnEnd;

    private int spawnCount; // 총 스폰 횟수

    private Dictionary<EnemyUnitData, ObjectPoolWithList<EnemyUnit>> poolDic = 
        new Dictionary<EnemyUnitData, ObjectPoolWithList<EnemyUnit>>();

    private void Update()
    {
        if (isSpawnEnd)
            return;

        if(spawnTimeCheck < spawnTime)
        {
            spawnTimeCheck += Time.deltaTime;
        }
        else
        {
            spawnTimeCheck -= spawnTime;

            EnemyUnitData data = waveData.MonsterSpawnInfos[spawnDataIndex].Enemy;
            if (!poolDic.ContainsKey(data))
                poolDic.Add(data, new ObjectPoolWithList<EnemyUnit>(() => CreateEnemyUnit(data)));

            EnemyUnit enemyUnit = poolDic[data].Pool.Get();
            Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
            enemyUnit.transform.position = pos;
            enemyUnit.transform.forward = spawnDirection.forward;
            enemyUnit.gameObject.SetActive(true);
            enemyUnit.Initialize(fortress.GetPosition(spawnCount));

            spawnDataEnemyCount++;
            spawnCount++;

            if (spawnDataEnemyCount >= waveData.MonsterSpawnInfos[spawnDataIndex].Count)
            {
                spawnDataEnemyCount = 0;
                spawnDataIndex++;
                if(spawnDataIndex >= waveData.MonsterSpawnInfos.Count)
                {
                    spawnDataIndex = 0;
                    isSpawnEnd = true;
                }
            }
        }
    }

    private EnemyUnit CreateEnemyUnit(EnemyUnitData data)
    {
        GameObject obj = Instantiate(data.Prefab);
        obj.SetActive(false);
        if(obj.TryGetComponent(out EnemyUnit enemy))
        {
            enemy.Initialize(data, poolDic[data], fortress);
            return enemy;
        }
        else
        {
            return null;
        }
    }
}
