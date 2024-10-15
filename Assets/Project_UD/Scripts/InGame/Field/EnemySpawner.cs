using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

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
    ObjectPool objectPool;
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
    private int spawnedMonsterCount = 0;
    public bool isWaveing = false;

    public List<GameObject> activeMonsters = new List<GameObject>();


    private void Awake()
    {
        inst = this;

        ObjectPool.Instance.Intialize(100);
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



        //timer += Time.deltaTime;
        ////level = Mathf.FloorToInt(Game_Manager.instance.gameTime / 10f);

        //if (level >= spawnData.Length - 1)
        //{
        //    level = spawnData.Length - 1;
        //}

        //if (timer > spawnData[level].spawnTime)
        //{
        //    timer = 0;
        //    //EnemySpawn();
        //}



    }

    //적 소환
    public GameObject EnemySpawn(int enemyType, float X, float Y)
    {
        Debug.Log(new Vector3(X, 0, Y));
        GameObject Obj = Instantiate(Test_Enemy);
        Obj.transform.position = new Vector3(X, 0, Y);
        //Obj.GetComponent<Ingame_UnitCtrl>().unitPos = new Vector2(X, Y);
        Obj.GetComponent<Ingame_UnitCtrl>().unitData = enemyDatas[enemyType];
        return Obj;
    }

    public IEnumerator WaveSystem()
    {
        if (isWaveing)
        {
            Ingame_UIManager.instance.ShowUI(Ingame_UIManager.instance.waveStartPanel, 1.5f);

            Debug.Log($"Wave {currentWave} 시작");

            // 몬스터 스폰
            yield return StartCoroutine(SpawnMonstersForWave());

            //OnMonsterDead(monster)

            // 모든 몬스터가 죽었는지 확인
            yield return StartCoroutine(CheckAllMonstersDead());

            currentWave++;


            isWaveing = false;

            isWaveing = true;

            //StartCoroutine(StartNextWaveCountdown()); // 다음 웨이브 카운트다운 시작
        }
    }

    // 웨이브에 따른 몬스터 생성 코루틴
    IEnumerator SpawnMonstersForWave()
    {
        int monstersToSpawn;

        if (currentWave <= 2)
        {
            monstersToSpawn = 5; // 현재 웨이브가 1 또는 2일 경우, 5마리의 몬스터를 생성
        }
        else
        {
            monstersToSpawn = 10; // 현재 웨이브가 3 이상일 경우, 10마리의 몬스터를 생성
        }

        for (int i = 0; i < monstersToSpawn; i++)
        {
            SpawnEnemy(1);
            yield return new WaitForSeconds(spawnInterval);  // 한 마리씩 생성
        }

        //if (currentWave <= 2)
        //{
        //    // 1, 2 웨이브에서는 몬스터 A만 생성
        //    for (int i = 0; i < monsterPerWave; i++)
        //    {
        //        SpawnEnemy(Random.Range(0, 2)); // 몬스터 A 스폰
        //        yield return new WaitForSeconds(spawnInterval);
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < monsterPerWave * 2; i++)
        //    {
        //        SpawnEnemy(0);
        //        // 3~10 웨이브는 A, B 몬스터가 각각 5마리씩 생성되도록 수정
        //        //int spawnedA = 0;
        //        //int spawnedB = 0;

        //        //while (spawnedA < monsterPerWave || spawnedB < monsterPerWave)
        //        //{
        //        //    // 랜덤으로 A 또는 B 몬스터 생성
        //        //    int enemyType = Random.Range(0, 2);

        //        //    if (enemyType == 0 && spawnedA < monsterPerWave)
        //        //    {
        //        //        SpawnEnemy(0); // A 몬스터 스폰
        //        //        spawnedA++;
        //        //    }
        //        //    else if (enemyType == 1 && spawnedB < monsterPerWave)
        //        //    {
        //        //        SpawnEnemy(1); // B 몬스터 스폰
        //        //        spawnedB++;
        //        //    }

        //        yield return new WaitForSeconds(spawnInterval);
        //    }
        //}
    }

    // 몬스터 생성
    void SpawnEnemy(int enemyType)
    {
        Transform spawnPos = poolSapwnPoint[Random.Range(0, poolSapwnPoint.Length)];
        GameObject enemyObj = ObjectPool.GetObject();

        if (enemyObj != null)
        {
            enemyObj.transform.position = spawnPos.position;
            enemyObj.transform.rotation = Quaternion.identity;
            enemyObj.GetComponent<Ingame_UnitCtrl>().unitData = enemyDatas[enemyType];
            activeMonsters.Add(enemyObj);
        }

    }

    // 몬스터 죽음 처리
    public void OnMonsterDead(GameObject monster)
    {
        if (activeMonsters.Contains(monster))
        {
            activeMonsters.Remove(monster);
            ObjectPool.ReturnObject(monster); // 오브젝트를 풀로 반환
        }
    }

    public IEnumerator StartWaveWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 1초 대기
        StartCoroutine(WaveSystem());  // 웨이브 시작
    }

    // 몬스터가 모두 죽었는지 확인
    IEnumerator CheckAllMonstersDead()
    {
        // 몬스터가 모두 죽을 때까지 대기
        while (activeMonsters.Count > 0)
        {
            yield return null;
        }

        
        // 10차 웨이브 이전까지만 성공 패널을 표시
        if (currentWave < waveCount)
        {
            Ingame_UIManager.instance.ShowUI(Ingame_UIManager.instance.waveStepSuccessPanel, 3.0f);

            yield return new WaitForSeconds(3.0f);

            Debug.Log("모든 몬스터가 죽었습니다. 다음 웨이브를 준비합니다.");
        }

        yield return new WaitForSeconds(1.0f);

        Ingame_UIManager.instance.isCountDownIng = true;

        // 마지막 웨이브(10차 웨이브)가 끝났을 때
        if (currentWave == waveCount && activeMonsters.Count <= 0)
        {
            Ingame_UIManager.instance.waveResultImage.sprite = Ingame_UIManager.instance.waveWinImage;

            Ingame_UIManager.instance.waveResultPanel.SetActive(true);

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

        // 현재 활성화된 모든 몬스터 제거
        foreach (var monster in activeMonsters)
        {
            Destroy(monster);
        }

        // 몬스터 리스트 초기화
        activeMonsters.Clear();

        if (!isWaveing && currentWave < waveCount)
        {
            StartCoroutine(WaveSystem());
        }
    }

}