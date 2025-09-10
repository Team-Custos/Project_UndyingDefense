using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitSpawner : MonoBehaviour
{
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private UpgradeMenuUI upgradeMenuUI;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private WaveManager waveManager;
    private int priority = 1;

    [Header("■ Components")]
    [SerializeField] private Fortress fortress;
    [SerializeField] private UnitDataLoader unitDataLoader;
    [SerializeField] private DurationEffectPool durationEffectPool;
    [SerializeField] private VFXObjectPool hitVFXPool;


    [Header("■ Options")]
    [SerializeField] private float spawnInterval = 1.5f;
    private float spawnTimer = 1.5f;

    private WaveData curWaveData;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform spawnDirection;
    [SerializeField] private AudioClip enmeySpawnSfx;
    [SerializeField] private ParticleSystem enemySpawnVfx;

    private int spawnCount; // 총 스폰 횟수
    private int activateEnemyCount = 0;
    private int spawnDataIndex; // 현재 EnemySpawnData의 인덱스
    private int spawnDataEnemyCount; // 현재 EnemySpawnData의 스폰 횟수.
    private bool isSpawnEnd = true;
    private bool isSpawnWait = false;


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
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                //WaveData waveData = waveManager.GetWaveData(waveManager.CurWave);
                EnemyUnitData enemyData = curWaveData.MonsterSpawnInfos[spawnDataIndex].Enemy;
                SpawnEnemy(enemyData);

                spawnDataEnemyCount++;
                spawnCount++;

                if (spawnDataEnemyCount >= curWaveData.MonsterSpawnInfos[spawnDataIndex].Count)
                {
                    spawnDataEnemyCount = 0;
                    spawnDataIndex++;
                    if (spawnDataIndex >= curWaveData.MonsterSpawnInfos.Count)
                    {
                        spawnDataIndex = 0;
                        isSpawnEnd = true;
                        enemySpawnVfx.gameObject.SetActive(false);

                    }
                }

                spawnTimer = spawnInterval;
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
            enemy.SetHitVFXPool(hitVFXPool);
            return enemy;
        }
        else
        {
            return null;
        }
    }

    public void SpawnEnemy(EnemyUnitData data)
    {
        if (!poolDic.ContainsKey(data))
            poolDic.Add(data, new ObjectPoolWithList<EnemyUnit>(() => CreateEnemyUnit(data)));

        EnemyUnit enemyUnit = poolDic[data].Pool.Get();
        poolDic[data].List.Add(enemyUnit);

        Unit unit = enemyUnit.GetComponent<Unit>();

        Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        enemyUnit.transform.position = pos;
        enemyUnit.transform.forward = spawnDirection.forward;
        enemyUnit.Initialize(fortress.GetPosition(spawnCount));
        enemyUnit.gameObject.SetActive(true);
        SoundManager.Instance.PlaySFX(enmeySpawnSfx);

        enemySpawnVfx.transform.position = pos;
        enemySpawnVfx.gameObject.SetActive(true);
        enemySpawnVfx.Play();

        activateEnemyCount++;
    }

    public void OnEnemyDead(EnemyUnitData enmeyUnitData, EnemyUnit enemyUnit)
    {
        poolDic[enmeyUnitData].List.Remove(enemyUnit);
        activateEnemyCount--;


        inGameManager.SetGold(enmeyUnitData.Gold, true);
        ingameScreenUI.SetspawnBtnPriceTextColor();
        upgradeMenuUI.UpdateUpgradeCostTxt();
        
        

        //if (totalMonCount <= 0 && isSpawnEnd) // 스폰 상태가 아닐때 몬스터 수가 0 이면 웨이브 종료
        //{
        //    isSpawnEnd = false;
        //    isWaveEnd = true;

        //    inGameManager.SetGold(waveData[curWave - 1].Reward, true);

        //    curWave++;
        //}
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

    public void WaveEnd()
    {
        //isWaveEnd = true;
    }

    public void GameLose()
    {
        SoundManager.Instance.StopBGM();
        //SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_battleLose]);
        //isGameOver = true;
        Time.timeScale = 0.0f;
    }

    // if (dollyCamera.IsCamPanning)
    //     return;

    // if(isGameOver)
    //     return;

    // if (isWaveEnd) // 웨이브 종료 및 시작 대기
    // {
    //    // 게임 성공
    //    if (curWave > waveData.Length && totalMonCount <= 0)
    //    {
    //        if(infinitMode)
    //        {
    //             curWave = 1;
    //        }
    //        else
    //        {
    //             SoundManager.Instance.StopBGM();
    //             SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_battleWin]);
    //             ingameScreenUI.ShowResult(100, true);
    //             Time.timeScale = 0.0f;
    //             isGameOver = true;
    //             return;
    //        }

    //    }


    //    waveDelay -= Time.deltaTime;
    //    if (waveDelay <= 0f)
    //    {
    //        if(infinitMode)
    //        {
    //             ingameScreenUI.SetWaveNumber(infinitWaveCount,0, true);
    //         }
    //        else
    //        {
    //             ingameScreenUI.SetWaveNumber(curWave, waveData.Length, false);
    //        }


    //        if (!oneTime)
    //        {
    //            SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_wavePrepare]);
    //            oneTime = true;
    //        }

    //        ingameScreenUI.ShowTimer();

    //        ingameScreenUI.SetNoticeText("웨이브 시작까지 " + (int)waveTimer + "초");

    //        waveTimer -= Time.deltaTime;
    //        if (waveTimer <= 0f)
    //        {   // 타이머 종료 및 웨이브 시작
    //            //ingameScreenUI.HideNotice();
    //            isWaveEnd = false;
    //            waveDelay = 1.0f;
    //            waveTimer = 20f;

    //            ingameScreenUI.HideTimer();

    //             if(infinitMode)
    //                 ingameScreenUI.ShowNotice(infinitWaveCount + "차 침공 시작");
    //             else
    //                 ingameScreenUI.ShowNotice(curWave + "차 침공 시작");

    //            SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_waveStart]);
    //        }
    //    }
    // }
    //else // 웨이브 시작
    //{
    //    if (isSpawnEnd && totalMonCount <= 0) // 웨이브 종료 및 시작 대기
    //    {
    //        SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_waveWin]);
    //        ingameScreenUI.ShowNotice("방어 성공!");



    //        isSpawnEnd = false;
    //        isWaveEnd = true;

    //        inGameManager.SetGold(waveData[curWave - 1].Reward, true);
    //        ingameScreenUI.SetspawnBtnPriceTextColor();
    //        upgradeMenuUI.UpdateUpgradeCostTxt();

    //         waveDelay = 4.0f;

    //        curWave++;
    //        infinitWaveCount++;

    //         isFortreessAttacked = false;

    //     }
    //    else if (isSpawnEnd)
    //     {
    //         enemySpawnVfx.gameObject.SetActive(false);
    //         return;
    //     }


    //    if (waveDelay > 0f)
    //    {   // 웨이브 딜레이 1초
    //        waveDelay -= Time.deltaTime;
    //    }
    //    else     // 스폰 시작
    //    {

    //        if (spawnTimer < spawnInterval) // Enemy 생성 쿨 타임
    //        {
    //             spawnTimer += Time.deltaTime;
    //        }
    //        else // Enemy 생성
    //        {
    //             spawnTimer -= spawnInterval;

    //            EnemyUnitData data = waveData[curWave - 1].MonsterSpawnInfos[spawnDataIndex].Enemy;
    //            if (!poolDic.ContainsKey(data))
    //                poolDic.Add(data, new ObjectPoolWithList<EnemyUnit>(() => CreateEnemyUnit(data)));


    //            EnemyUnit enemyUnit = poolDic[data].Pool.Get();
    //            poolDic[data].List.Add(enemyUnit);

    //             Unit unit = enemyUnit.GetComponent<Unit>();


    //             unitDataLoader.GetUnitDataById(unit.UnitId, unit);

    //             Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    //            enemyUnit.transform.position = pos;
    //            enemyUnit.transform.forward = spawnDirection.forward;
    //            SoundManager.Instance.PlaySFX(enmeySpawnSfx);
    //            enemySpawnVfx.transform.position = pos;
    //            enemySpawnVfx.gameObject.SetActive(true);
    //            enemySpawnVfx.Play();
    //            enemyUnit.Initialize(fortress.GetPosition(spawnCount));
    //            enemyUnit.gameObject.SetActive(true);
    //             //enemyUnit.Setpriority(priority);
    //             //if(priority > 50)
    //             //    priority = 1;
    //             //else
    //             //    priority++;


    //             //enemyUnit.SetAgentPriority(enemyPriority);
    //             //enemyPriority++;

    //             //if (enemyPriority > 50)
    //             //    enemyPriority = 0;


    //            totalMonCount++;
    //            spawnDataEnemyCount++;
    //            spawnCount++;

    //            if (spawnDataEnemyCount >= waveData[curWave - 1].MonsterSpawnInfos[spawnDataIndex].Count)
    //            {
    //                spawnDataEnemyCount = 0;
    //                spawnDataIndex++;
    //                if (spawnDataIndex >= waveData[curWave - 1].MonsterSpawnInfos.Count)
    //                {
    //                    spawnDataIndex = 0;
    //                    isSpawnEnd = true;

    //                     waveDelay = 1.0f;

    //                 }
    //            }

    //        }
    //    }
    //}
}