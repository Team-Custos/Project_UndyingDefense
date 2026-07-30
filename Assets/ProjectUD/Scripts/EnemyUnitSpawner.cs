using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitSpawner : MonoBehaviour
{
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private UpgradeMenuUI upgradeMenuUI;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private WaveDataLoader waveDataLoader;

    [Header("■ Components")]
    [SerializeField] private Fortress fortress;
    [SerializeField] private UnitDataLoader unitDataLoader;
    [SerializeField] private DurationEffectPool durationEffectPool;
    [SerializeField] private InstantEffectPool instantEffectPool;
    [SerializeField] private VFXObjectPool hitVFXPool;
    [SerializeField] private VFXObjectPool skillVFXPool;
    [SerializeField] private EffectImagePool effectImagePool;


    [Header("■ Options")]
    [SerializeField] private float spawnInterval = 1.5f;
    private float spawnTimer = 1.5f;

    private WaveData curWaveData;

    [SerializeField] private Transform[] aSpawnPoints;
    [SerializeField] private Transform[] bSpawnPoints;

    [SerializeField] private Transform spawnDirection;
    [SerializeField] private AudioClip enmeySpawnSfx;
    [SerializeField] private AudioClip portalSfx;  // 스폰 진행되는 동안 루핑
    [SerializeField] private ParticleSystem enemySpawnVfx;

    private int spawnCount; // 총 스폰 횟수
    private int activateEnemyCount = 0;
    private int spawnDataIndex; // 현재 EnemySpawnData의 인덱스
    private int spawnDataEnemyCount; // 현재 EnemySpawnData의 스폰 횟수.
    private bool isSpawnEnd = true;
    private bool isSpawnWait = false;
    private float bonusGoldPercent = 0f; // 적 처치시 추가 골드 획득 퍼센트


    private Dictionary<EnemyUnitData, ObjectPoolWithList<EnemyUnit>> poolDic =
        new Dictionary<EnemyUnitData, ObjectPoolWithList<EnemyUnit>>();

    public bool IsSpawnEnd => isSpawnEnd;
    public int ActivateEnemyCount => activateEnemyCount;


    private void Update()
    {
        if(!isSpawnWait)
            return;

        if (!isSpawnEnd)
        {
            if (!inGameManager.IsGameStart) // 게임 종료시 스폰 정지
                return;

            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {

                //WaveData waveData = waveManager.GetWaveData(waveManager.CurWave);
                EnemyUnitData enemyData = waveDataLoader.GetEnemyUniData(waveManager.CurWave - 1, spawnDataEnemyCount);
                
                Vector3 pos = Vector3.zero;
                int randIndex = 0;

                if (waveDataLoader.WaveDataList[waveManager.CurWave - 1][spawnDataEnemyCount].pos == "A")
                {
                    randIndex = Random.Range(0, aSpawnPoints.Length);

                    pos = aSpawnPoints[randIndex].position;
                }
                else
                {
                    randIndex = Random.Range(0, bSpawnPoints.Length);
                    pos = bSpawnPoints[randIndex].position;
                }
                    

                SpawnEnemy(enemyData, pos);

                spawnDataEnemyCount++;
                spawnCount++;

                

                if (waveDataLoader.WaveDataList[waveManager.CurWave - 1].Count <= spawnDataEnemyCount)
                {
                    spawnDataEnemyCount = 0;
                    enemySpawnVfx.gameObject.SetActive(false);
                    isSpawnEnd = true;
                    spawnTimer = 0f;
                }
                else
                {
                    spawnTimer = waveDataLoader.WaveDataList[waveManager.CurWave - 1][spawnDataEnemyCount].interval;
                }



                //if (spawnDataEnemyCount >= curWaveData.MonsterSpawnInfos[spawnDataIndex].Count)
                //{
                //    spawnDataEnemyCount = 0;
                //    spawnDataIndex++;
                //    if (spawnDataIndex >= curWaveData.MonsterSpawnInfos.Count)
                //    {
                //        spawnDataIndex = 0;
                //        isSpawnEnd = true;
                //        enemySpawnVfx.gameObject.SetActive(false);
                //        //SoundManager.Instance.StopLoopSFX(portalSfx);

                //    }
                //}

            }
        }
        else
        {
            if (activateEnemyCount <= 0)
            {
                waveManager.SetWaveEnd();
                isSpawnWait = false;


            }
        }
    }


    private EnemyUnit CreateEnemyUnit(EnemyUnitData data)
    {
        GameObject obj = Instantiate(data.Prefab);
        obj.SetActive(false);
        if (obj.TryGetComponent(out EnemyUnit enemy))
        {
            Unit unit = enemy.GetComponent<Unit>();

            UnitStats unitStats = unitDataLoader.GetUnitDataById(unit.UnitId);
            unit.SetUnitStats(unitStats);

            enemy.Initialize(data, poolDic[data], fortress, this);
            enemy.SetDurationEffectPool(durationEffectPool);
            enemy.SetInstantEffectPool(instantEffectPool);
            enemy.SetEffectImagePool(effectImagePool);
            enemy.SetHitVFXPool(hitVFXPool, skillVFXPool);
            return enemy;
        }
        else
        {
            return null;
        }
    }

    public void SpawnEnemy(EnemyUnitData data, Vector3 pos)
    {
        if (!poolDic.ContainsKey(data))
            poolDic.Add(data, new ObjectPoolWithList<EnemyUnit>(() => CreateEnemyUnit(data)));

        EnemyUnit enemyUnit = poolDic[data].Pool.Get();
        poolDic[data].List.Add(enemyUnit);

        Unit unit = enemyUnit.GetComponent<Unit>();

        enemyUnit.transform.position = pos;
        enemyUnit.transform.forward = spawnDirection.forward;
        enemyUnit.Initialize(fortress.GetPosition(spawnCount));
        enemyUnit.gameObject.SetActive(true);
        SoundManager.Instance.PlaySFX(enmeySpawnSfx, enemyUnit.transform.position);


        enemySpawnVfx.transform.position = pos;
        enemySpawnVfx.gameObject.SetActive(true);
        enemySpawnVfx.Play();

        activateEnemyCount++;
    }

    public void OnEnemyDead(EnemyUnitData enmeyUnitData, EnemyUnit enemyUnit)
    {
        poolDic[enmeyUnitData].List.Remove(enemyUnit);
        activateEnemyCount--;


        float bonusGold = enmeyUnitData.Gold * (1f + bonusGoldPercent);

        inGameManager.SetGold(bonusGold, true);
        ingameScreenUI.SetspawnBtnPriceTextColor();
        upgradeMenuUI.UpdateUpgradeCostTxt();
        
        
    }

    public void OnEnemyDead()
    {
        activateEnemyCount--;

        //if (totalMonCount <= 0 && isSpawnEnd)
        //{
        //    isSpawnEnd = false;
        //    isWaveEnd = true;

        //    //inGameManager.SetGold(waveData[curWave - 1].Reward, true);

        //    curWave++;
        //}
    }


    public void StartSpawn(WaveData waveData)
    {
        curWaveData = waveData;
        isSpawnEnd = false;
        isSpawnWait = true;
        //SoundManager.Instance.PlayLoopSFX(portalSfx);
    }

    public void StopActivateEnemy()
    {
        foreach (var kvp in poolDic)
        {
            foreach (var enemyUnit in kvp.Value.List)
            {
                if (enemyUnit != null)
                    enemyUnit.StopUnit();
            }
        }
    }

    // 적 처치시 얻는 골드 변화량 적용 함수
    public void AddGoldBonusPercent(float percent)
    {
        bonusGoldPercent += percent * 0.01f;
    }

    public List<EnemyUnit> GetActivateEnemy(EnemyUnitData enemyData)
    {
        if (poolDic.TryGetValue(enemyData, out var pool))
            return pool.List;

        return null;
    }
}