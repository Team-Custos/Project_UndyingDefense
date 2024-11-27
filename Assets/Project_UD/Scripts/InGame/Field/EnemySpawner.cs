using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Pool;

//이 스크립트는 적군을 스폰시키기 위한 스크립트입니다.

[System.Serializable]

public class EnemySpawnData
{
    public int modelType;
    public float spawnTime;
    public int HP;
    public float moveSpeed;
    public int attackPoint;
    public float attackSpeed;
    public int critChanceRate;

    public float sightRange;
    public float attackRange;

    public int generalSkill;
    public int specialSkill;
    public UnitType unitType;
    public DefenseType defenseType;
    public TargetSelectType targetSelectType;

}

//public enum EnemyType
//{
//    Warrior,
//    Archer,
//}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner inst;
    public List<Ingame_UnitData> enemyDatas;

    GridManager gridManager;

    int mobSpawnPosX = 0;
    int gridHeight = 0;

    int pointidx = 0;

    public Transform[] spawnPoint;
    public EnemySpawnData[] spawnData;

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

        StartCoroutine(StartWaveWithDelay(1f));
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

    public IEnumerator WaveSystem()
    {
        if (isWaveing)
        {
            Ingame_WaveUIManager.instance.ShowUI(Ingame_WaveUIManager.instance.waveStartPanel, 3.0f);
            Debug.Log($"Wave {currentWave} 시작");

            Ingame_WaveUIManager.instance.waveStepText.text = "웨이브 " + currentWave;
            Ingame_WaveUIManager.instance.curWaveStepText.text = "웨이브 " + currentWave;
            Ingame_WaveUIManager.instance.waveStepText.gameObject.SetActive(true);

            yield return StartCoroutine(SpawnMonstersForWave());
            yield return StartCoroutine(CheckAllMonstersDead());

            currentWave++;
            isWaveing = false;
        }
    }

    // 웨이브에 따른 몬스터 생성 코루틴
    IEnumerator SpawnMonstersForWave()
    {
        int monstersToSpawn;

        waveRewardGold = 500;

        // 현재 웨이브가 1~2일 때는 5마리의 기본 몬스터 생성
        if (currentWave <= 2)
        {
            monstersToSpawn = 5;
            for (int i = 0; i < monstersToSpawn; i++)
            {
                SpawnEnemy(0); // 기본 몬스터 타입으로 생성
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        else
        {
            int monsterType0Count = 0;
            int monsterType1Count = 0;

            while (monsterType0Count < 5 || monsterType1Count < 5)
            {
                int randomType = Random.Range(0, 2);

                Debug.Log(randomType);

                if (randomType == 0 && monsterType0Count < 5)
                {
                    SpawnEnemy(0);
                    monsterType0Count++;
                }
                else if (randomType == 1 && monsterType1Count < 5)
                {
                    SpawnEnemy(1);
                    monsterType1Count++;
                }
                // 두 타입이 모두 5마리에 도달한 경우 루프를 종료
                if (monsterType0Count >= 5 && monsterType1Count >= 5)
                {
                    break;
                }

                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    // 몬스터 생성
    void SpawnEnemy(int enemyType)
    {
        Transform spawnPos = poolSapwnPoint[Random.Range(0, poolSapwnPoint.Length)];
        GameObject enemyObj = Instantiate(Test_Enemy, spawnPos.position, Quaternion.identity);

        if (enemyType == 0)
        {
            enemyObj.GetComponent<Ingame_UnitCtrl>().unitData = enemyDatas[0];
            enemyObj.GetComponent<Ingame_UnitCtrl>().unitData.modelType = 0;
        }
        else if (enemyType == 1)
        {
            enemyObj.GetComponent<Ingame_UnitCtrl>().unitData = enemyDatas[1];
            enemyObj.GetComponent<Ingame_UnitCtrl>().unitData.modelType = 1;
        }

        activeMonsters.Add(enemyObj);
        enemyObj.GetComponent<NavMeshAgent>().avoidancePriority = enemypriority % 50;

        enemypriority++;
        enemyObj.name = Test_Enemy.name + enemypriority.ToString();
    }


    //몬스터 죽음 처리
    public void OnMonsterDead(GameObject monster)
    {
        if (activeMonsters.Contains(monster))
        {
            activeMonsters.Remove(monster);
            //Destroy(monster);
        }
    }

    public IEnumerator StartWaveWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(WaveSystem());
    }

    // 몬스터가 모두 죽었는지 확인
    IEnumerator CheckAllMonstersDead()
    {
        while (activeMonsters.Count > 0)
        {
            yield return null;
        }

        if (currentWave < waveCount)
        {
            Ingame_WaveUIManager.instance.ShowUI(Ingame_WaveUIManager.instance.waveStepSuccessPanel, 3.0f);
            InGameManager.inst.gold += waveRewardGold;
            Ingame_UIManager.instance.goldTxt.text = InGameManager.inst.gold.ToString();
            Ingame_WaveUIManager.instance.waveStepText.gameObject.SetActive(false);
            yield return new WaitForSeconds(3.0f);
            Debug.Log("모든 몬스터가 죽었습니다. 다음 웨이브를 준비합니다.");
            
            
        }

        yield return new WaitForSeconds(1.0f);

        Ingame_WaveUIManager.instance.isCountDownIng = true;

        if (currentWave == waveCount && activeMonsters.Count <= 0)
        {
            Ingame_WaveUIManager.instance.waveResultWinPanel.SetActive(true);
            Debug.Log("웨이브 종료");
            Time.timeScale = 0.0f;
        }
    }


    // 다음 웨이브 카운트다운 시작
    public void NextWave()
    {
        if (isWaveing)
        {
            Debug.LogWarning("웨이브가 이미 진행 중입니다. 새로운 웨이브 시작을 중단합니다.");
            return;
        }

        Debug.Log("다음 웨이브로 넘어갑니다.");

        foreach (var monster in activeMonsters)
        {
            Destroy(monster);
        }

        activeMonsters.Clear();

        if (!isWaveing && currentWave < waveCount)
        {
            StartCoroutine(WaveSystem());
        }
    }
}

