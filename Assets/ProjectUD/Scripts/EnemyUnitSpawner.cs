using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyUnitSpawner : MonoBehaviour
{
    [Header("■ Components")]
    [SerializeField] private Fortress fortress;

    [Header("■ Options")]
    [SerializeField] private float spawnTime;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform spawnDirection;

    private float spawnTimeCheck;

    [SerializeField] private WaveData[] waveData;
    [SerializeField] private int curWave = 1;

    private int spawnDataIndex; // 현재 EnemySpawnData의 인덱스
    private int spawnDataEnemyCount; // 현재 EnemySpawnData의 스폰 횟수.
    private bool isSpawnEnd;

    [SerializeField] private bool isWaveEnd;
    [SerializeField] private float waveTimer = 20f;

    private int spawnCount; // 총 스폰 횟수

    [SerializeField] private int totalMonCount = 0;

    private Dictionary<EnemyUnitData, ObjectPoolWithList<EnemyUnit>> poolDic = 
        new Dictionary<EnemyUnitData, ObjectPoolWithList<EnemyUnit>>();

    [Header("■ UI")]
    [SerializeField] private IngameScreenUI ingameUI;

    private void Update()
    {
        if(isWaveEnd)
        {
            ingameUI.ShowNotice("웨이브 시작까지 " + waveTimer.ToString("F1") + "초.", false);
            waveTimer -= Time.deltaTime;
            if (waveTimer <= 0f)
            {
                waveTimer = 20f;
                isWaveEnd = false;
                waveTimer = 20f;
            }
        }
        else
        {
            if (isSpawnEnd)
                return;

            if (spawnTimeCheck < spawnTime) // Enemy 생성 쿨 타임
            {
                spawnTimeCheck += Time.deltaTime;
            }
            else // Enemy 생성
            {
                spawnTimeCheck -= spawnTime;

                EnemyUnitData data = waveData[curWave - 1].MonsterSpawnInfos[spawnDataIndex].Enemy;
                if (!poolDic.ContainsKey(data))
                    poolDic.Add(data, new ObjectPoolWithList<EnemyUnit>(() => CreateEnemyUnit(data)));

                totalMonCount++;
                Debug.Log("현재 몬스터 수 : " + totalMonCount);

                EnemyUnit enemyUnit = poolDic[data].Pool.Get();
                Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
                enemyUnit.transform.position = pos;
                enemyUnit.transform.forward = spawnDirection.forward;
                enemyUnit.gameObject.SetActive(true);
                enemyUnit.Initialize(fortress.GetPosition(spawnCount));

                spawnDataEnemyCount++;
                spawnCount++;

                if (spawnDataEnemyCount >= waveData[curWave - 1].MonsterSpawnInfos[spawnDataIndex].Count)
                {
                    spawnDataEnemyCount = 0;
                    spawnDataIndex++;
                    if (spawnDataIndex >= waveData[curWave - 1].MonsterSpawnInfos.Count)
                    {
                        spawnDataIndex = 0;
                        isSpawnEnd = true;
                        curWave++;
                    }
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

    public void OnEnemyDead()
    {
        totalMonCount--;

        Debug.Log("현재 몬스터 수 : " + totalMonCount);

        if (totalMonCount == 0 && isSpawnEnd)
        {
            isSpawnEnd = false;
            isWaveEnd = true;
        }
    }

}
