using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//이 스크립트는 적군을 스폰시키기 위한 스크립트입니다.

public class EnemySpawner : MonoBehaviour
{
    public InGame_BGMManager bgmManager;

    public WaveDataTable waveDataTable;


    public static EnemySpawner inst;
    public List<Ingame_UnitData> enemyDatas;

    GridManager gridManager;

    int mobSpawnPosX = 0;
    int gridHeight = 0;

    int pointidx = 0;

    public Transform[] spawnPoint;

    bool isMobSpawnerPosSet = false;

    int level;
    float timer;

    public GameObject Test_Enemy;

    public int enemyToSpawn = 0;

    public Transform[] poolSapwnPoint;

    public int waveCount = 10;
    public int monsterPerWave = 5;
    public float spawnInterval = 2.0f;

    public int currentWave = 1;
    public bool isWaveing = false;
    public int waveRewardGold = 0;

    public List<GameObject> activeMonsters = new List<GameObject>();

    int enemypriority = 0;

    public bool isBaseAttackPerWave;  // 웨이브에 Base가 공격당했는지 확인

    private void Awake()
    {
        inst = this;
    }

    private void Start()
    {
        gridManager = InGameManager.inst.gridManager;


        mobSpawnPosX = gridManager._width;
        gridHeight = gridManager._height;

        spawnPoint = new Transform[gridHeight];

        Ingame_WaveUIManager.instance.StartNextWaveCountdown(20.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMobSpawnerPosSet)
        {
            for (int idx = 0; idx < gridManager.Tiles_Obj.Length; idx++)
            {
                //Debug.Log("Idx : " + idx);
                if (gridManager.Tiles_Obj[idx].GetComponent<GridTile>().GridPos.x == mobSpawnPosX - 1)
                {

                    //Debug.Log("PointIdx : " + pointidx);
                    spawnPoint[pointidx] = gridManager.Tiles_Obj[idx].transform;
                    if (pointidx >= gridHeight - 1)
                    {
                        isMobSpawnerPosSet = true;
                    }
                    else
                    {
                        pointidx++;
                    }
                    continue;
                }
            }
        }
    }

    public IEnumerator RunWave()
    {
        yield return new WaitForSeconds(1.0f);

        isWaveing = true;
        isBaseAttackPerWave = false;

        WaveData currentData = waveDataTable.GetWaveData(currentWave);
        if (currentData == null)
        {
            Debug.LogError("해당 웨이브 데이터를 찾을 수 없습니다: " + currentWave);
            yield break;
        }

        // 웨이브 시작 UI 표시
        Ingame_WaveUIManager.instance.ShowUI(Ingame_WaveUIManager.instance.waveStartPanel, 3.0f);
        Ingame_WaveUIManager.instance.waveStepText.text = "웨이브 " + currentWave;
        Ingame_WaveUIManager.instance.curWaveStepText.text = "웨이브 " + currentWave;
        Ingame_WaveUIManager.instance.waveStepText.gameObject.SetActive(true);
        Debug.Log($"Wave {currentWave} 시작");

        SoundManager.instance.PlayWaveSFX(SoundManager.waveSfx.sfx_waveStart);

        if(bgmManager  == null)
        {
            Debug.Log("fefe");
        }

        // 몬스터 스폰 순서 결정
        List<int> spawnOrder = new List<int>();
        foreach (var info in currentData.monsterSpawnInfos)
        {
            for (int i = 0; i < info.repeatNum; i++)
            {
                spawnOrder.Add(info.monsterType);
            }
        }

        // 랜덤 선택
        for (int i = 0; i < spawnOrder.Count; i++)
        {
            int rand = Random.Range(0, spawnOrder.Count);
            int temp = spawnOrder[i];
            spawnOrder[i] = spawnOrder[rand];
            spawnOrder[rand] = temp;
        }

        // interval 간격으로 스폰
        foreach (int monsterTypeId in spawnOrder)
        {
            SpawnEnemy(monsterTypeId);
            yield return new WaitForSeconds(currentData.interval);
        }

        // 모든 몬스터 처치 대기
        yield return StartCoroutine(CheckAllMonstersDead());

        

        // 웨이브 클리어 UI
        if(currentData.waveNumber == 10)
        {
            yield return null;
        }
        else
        {
            Ingame_WaveUIManager.instance.ShowUI(Ingame_WaveUIManager.instance.waveStepSuccessPanel, 3.0f);
            Ingame_WaveUIManager.instance.waveStepText.gameObject.SetActive(false);
        }

        SoundManager.instance.PlayWaveSFX(SoundManager.waveSfx.sfx_waveWin);

        // 웨이브 클리어 보상 지급
        InGameManager.inst.gold += currentData.reward;
        Ingame_UIManager.instance.goldTxt.text = InGameManager.inst.gold.ToString();
        Debug.Log($"[Wave {currentWave}] 클리어! 보상 {currentData.reward} 획득");

        if(currentData.waveNumber == 10)
        {
            if(bgmManager != null) 
            {
                bgmManager.PauseBGM();      // 웨이브 종료시 bgm은 끔
            }
            Ingame_WaveUIManager.instance.waveResultWinPanel.SetActive(true);
            SoundManager.instance.PlayWaveSFX(SoundManager.waveSfx.sfx_battleWin);
            InGameManager.inst.isGamePause = true;
        }

        yield return new WaitForSeconds(4.0f);

        isWaveing = false;
        currentWave++;

        // 다음 웨이브 확인
        WaveData nextWaveData = waveDataTable.GetWaveData(currentWave);


        if (nextWaveData != null && nextWaveData)
        {
            Ingame_WaveUIManager.instance.StartNextWaveCountdown(20f);
            Ingame_WaveUIManager.instance.waveCountTextPanel.SetActive(true);
            SoundManager.instance.PlayWaveSFX(SoundManager.waveSfx.sfx_wavePrepare);
        }
    }


    //적 소환
    public GameObject EnemySpawn(int enemyType, float X, float Y)
    {
        Debug.Log(new Vector3(X, 0, Y));
        GameObject Obj = Instantiate(Test_Enemy);
        Obj.transform.position = new Vector3(X, 0, Y);
        Obj.GetComponent<Ingame_UnitCtrl>().unitPos = new Vector2(X, Y);
        Obj.GetComponent<Ingame_UnitCtrl>().unitData = enemyDatas[enemyType];
        return Obj;
    }

    // 몬스터 생성
    void SpawnEnemy(int enemyType)
    {
        Transform spawnPos = poolSapwnPoint[Random.Range(0, poolSapwnPoint.Length)];
        Vector3 newPosition = spawnPos.position;
        newPosition.y = -0.9f;
        spawnPos.position = newPosition;
        //Ingame_ParticleManager.Instance.PlaySummonParticleEffect(spawnPos, false);
        GameObject enemyObj = Instantiate(Test_Enemy, spawnPos.position, Quaternion.identity);

        enemyObj.GetComponent<Ingame_UnitCtrl>().unitData = enemyDatas[enemyType];

        activeMonsters.Add(enemyObj);
        enemyObj.GetComponent<NavMeshAgent>().avoidancePriority = enemypriority % 50;

        enemypriority++;
        enemyObj.name = Test_Enemy.name + enemypriority.ToString();

        SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_enemySpawn);
    }


    //몬스터 죽음 처리
    public void OnMonsterDead(GameObject monster)
    {
        if (activeMonsters.Contains(monster))
        {
            activeMonsters.Remove(monster);
        }
    }

    IEnumerator CheckAllMonstersDead()
    {
        while (activeMonsters.Count > 0)
        {
            yield return null;
        }
    }

    // Base 공격 확인 함수
    public void OnBaseAttacked()
    {
        if (!isBaseAttackPerWave) // 현재 웨이브에서 Base가 공격 당했다면
        {
            Ingame_WaveUIManager.instance.ShowUI(Ingame_WaveUIManager.instance.waveWarnningPanel, 3.0f);
            isBaseAttackPerWave = true;
        }
    }

}