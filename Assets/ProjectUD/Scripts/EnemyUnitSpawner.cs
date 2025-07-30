using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitSpawner : MonoBehaviour
{
    public enum waveSfx
    {
        sfx_waveWin,                //  웨이브 방어 성공
        sfx_wavePrepare,            //  웨이브 준비 단계 알림
        sfx_waveStart,              //  웨이브 시작 알림
        sfx_battleLose,             //  전투 패배
        sfx_battleWin               //  전투 승리
    }

    [Header("■ Components")]
    [SerializeField] private Fortress fortress;
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private DollyCamera dollyCamera;
    [SerializeField] private UpgradeMenuUI upgradeMenuUI;
    [SerializeField] private UnitDataLoader unitDataLoader;
    [SerializeField] private DurationEffectPool durationEffectPool;
    [SerializeField] private ObjectPoolTest hitVFXPool;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;

    [Header("■ Wave Data")]
    [SerializeField] private WaveData[] waveData;


    [Header("■ Options")]
    [SerializeField] private float spawnTime;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform spawnDirection;
    [SerializeField] private int curWave = 1;
    [SerializeField] private bool isWaveEnd;
    [SerializeField] private float waveTimer = 20f;
    [SerializeField] private int totalMonCount = 0;
    [SerializeField] AudioClip[] waveSfxClip;
    [SerializeField] private bool infinitMode = false;
    [SerializeField] private int infinitWaveCount = 1;

    [SerializeField] private AudioClip enmeySpawnSfx;
    [SerializeField] private ParticleSystem enemySpawnVfx;

    private bool oneTime = false;
    private float spawnTimeCheck;
    private int spawnDataIndex; // 현재 EnemySpawnData의 인덱스
    private int spawnDataEnemyCount; // 현재 EnemySpawnData의 스폰 횟수.
    private bool isSpawnEnd;
    private float waveDelay = 4.0f; // 웨이브 시작간 대기 시간 1초
    private int spawnCount; // 총 스폰 횟수
    private bool isFortreessAttacked;
    private bool isWaveReady = true;
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;

    private Dictionary<EnemyUnitData, ObjectPoolWithList<EnemyUnit>> poolDic =
        new Dictionary<EnemyUnitData, ObjectPoolWithList<EnemyUnit>>();

    [Header("■ UI")]
    [SerializeField] private IngameScreenUI ingameScreenUI;


    private void Update()
    {
        if (dollyCamera.IsCamPanning)
            return;

        if(isGameOver)
            return;

        if (isWaveEnd) // 웨이브 종료 및 시작 대기
        {
           // 게임 성공
           if (curWave > waveData.Length && totalMonCount <= 0)
           {
               if(infinitMode)
               {
                    curWave = 1;
               }
               else
               {
                    SoundManager.Instance.StopBGM();
                    SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_battleWin]);
                    ingameScreenUI.ShowResult(100, true);
                    Time.timeScale = 0.0f;
                    isGameOver = true;
                    return;
               }
               
           }


           waveDelay -= Time.deltaTime;
           if (waveDelay <= 0f)
           {
               if(infinitMode)
               {
                    ingameScreenUI.SetWaveNumber(infinitWaveCount,0, true);
                }
               else
               {
                    ingameScreenUI.SetWaveNumber(curWave, waveData.Length, false);
               }
               

               if (!oneTime)
               {
                   SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_wavePrepare]);
                   oneTime = true;
               }

               ingameScreenUI.ShowTimer();

               ingameScreenUI.SetNoticeText("웨이브 시작까지 " + (int)waveTimer + "초");

               waveTimer -= Time.deltaTime;
               if (waveTimer <= 0f)
               {   // 타이머 종료 및 웨이브 시작
                   //ingameScreenUI.HideNotice();
                   isWaveEnd = false;
                   waveDelay = 1.0f;
                   waveTimer = 20f;

                   ingameScreenUI.HideTimer();
                   
                    if(infinitMode)
                        ingameScreenUI.ShowNotice(infinitWaveCount + "차 침공 시작");
                    else
                        ingameScreenUI.ShowNotice(curWave + "차 침공 시작");

                   SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_waveStart]);
               }
           }
        }
       else // 웨이브 시작
       {
           if (isSpawnEnd && totalMonCount <= 0) // 웨이브 종료 및 시작 대기
           {
               SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_waveWin]);
               ingameScreenUI.ShowNotice("방어 성공!");

               
    
               isSpawnEnd = false;
               isWaveEnd = true;

               inGameManager.SetGold(waveData[curWave - 1].Reward, true);
               ingameScreenUI.SetspawnBtnPriceTextColor();
               upgradeMenuUI.UpdateUpgradeCostTxt();

                waveDelay = 4.0f;

               curWave++;
               infinitWaveCount++;

                isFortreessAttacked = false;

            }
           else if (isSpawnEnd)
            {
                enemySpawnVfx.gameObject.SetActive(false);
                return;
            }
               

           if (waveDelay > 0f)
           {   // 웨이브 딜레이 1초
               waveDelay -= Time.deltaTime;
           }
           else     // 스폰 시작
           {

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


                   EnemyUnit enemyUnit = poolDic[data].Pool.Get();
                   poolDic[data].List.Add(enemyUnit);

                    Unit unit = enemyUnit.GetComponent<Unit>();

                    unitDataLoader.GetUnitDataById(unit.UnitId, unit);

                    Vector3 pos = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
                   enemySpawnVfx.transform.position = pos;
                   enemyUnit.transform.position = pos;
                   enemyUnit.transform.forward = spawnDirection.forward;
                   SoundManager.Instance.PlaySFX(enmeySpawnSfx);
                   enemySpawnVfx.gameObject.SetActive(true);
                   enemySpawnVfx.Play();
                   enemyUnit.Initialize(fortress.GetPosition(spawnCount));
                   enemyUnit.gameObject.SetActive(true);

                   totalMonCount++;
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

                            waveDelay = 1.0f;
                            
                        }
                   }

               }
           }
       }

    }

    private EnemyUnit CreateEnemyUnit(EnemyUnitData data)
    {
        GameObject obj = Instantiate(data.Prefab);
        obj.SetActive(false);
        if (obj.TryGetComponent(out EnemyUnit enemy))
        {
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

    public void OnEnemyDead(EnemyUnitData enmeyUnitData)
    {
        totalMonCount--;

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
        totalMonCount--;

        //if (totalMonCount <= 0 && isSpawnEnd)
        //{
        //    isSpawnEnd = false;
        //    isWaveEnd = true;

        //    //inGameManager.SetGold(waveData[curWave - 1].Reward, true);

        //    curWave++;
        //}
    }

    public void SetWaverTimerEnd()
    {
        waveTimer = 0.0f;

    }

    public void OnFortressAttacked()
    {
        if (!isFortreessAttacked)
        {
            ingameScreenUI.ShowPreWaveNotice();

            isFortreessAttacked = true;
        }

    }

    public void WaveEnd()
    {
        isWaveEnd = true;
    }

    public void GameLose()
    {
        SoundManager.Instance.StopBGM();
        SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_battleLose]);
        isGameOver = true;
        Time.timeScale = 0.0f;
    }
}