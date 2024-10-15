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

    public GameObject Test_EnemyA;
    public GameObject Test_EnemyB;

    private int currentWave = 1;
    private int spawnedMonsterCount = 0;
    private bool isWaveInProgress = false;

    private List<GameObject> activeMonsters = new List<GameObject>();


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

        ObjectPool.Instance.Intialize(20);

        currentWave = 1;

        //StartCoroutine(WaveSystem());
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

        

        timer += Time.deltaTime;
        //level = Mathf.FloorToInt(Game_Manager.instance.gameTime / 10f);

        if (level >= spawnData.Length - 1)
        {
            level = spawnData.Length - 1;
        }

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            //EnemySpawn();
        }


        
    }

    //적 소환
    public GameObject EnemySpawn(int enemyType,float X, float Y)
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
        if (isWaveInProgress)
        {
            // 이미 웨이브가 진행 중이면, 중복 실행을 방지
            yield break;
        }

        isWaveInProgress = true; // 웨이브 진행 중 상태 설정
        spawnedMonsterCount = 0;

        Debug.Log($"Wave {currentWave} 시작");

        // 몬스터 스폰 루틴 실행
        yield return StartCoroutine(SpawnMonstersForWave());

        // 몬스터가 모두 죽을 때까지 기다림
        yield return StartCoroutine(CheckAllMonstersDead());

        isWaveInProgress = false; // 웨이브가 끝나면 진행 중 상태 해제
    }

    // 웨이브에 따른 몬스터 생성 코루틴
    IEnumerator SpawnMonstersForWave()
    {
        if (currentWave <= 2)
        {
            // 1, 2 웨이브에서는 몬스터 A만 생성
            for (int i = 0; i < monsterPerWave; i++)
            {
                SpawnEnemy(0); // 몬스터 A 스폰
                yield return new WaitForSeconds(spawnInterval);
            }
        }
        else
        {
            // 3~10 웨이브는 A, B 몬스터가 각각 5마리씩 생성되도록 수정
            int spawnedA = 0;
            int spawnedB = 0;

            while (spawnedA < monsterPerWave || spawnedB < monsterPerWave)
            {
                // 랜덤으로 A 또는 B 몬스터 생성
                int enemyType = Random.Range(0, 2);

                if (enemyType == 0 && spawnedA < monsterPerWave)
                {
                    SpawnEnemy(0); // A 몬스터 스폰
                    spawnedA++;
                }
                else if (enemyType == 1 && spawnedB < monsterPerWave)
                {
                    SpawnEnemy(1); // B 몬스터 스폰
                    spawnedB++;
                }

                // A와 B 모두 생성 완료 시 루프를 종료
                if (spawnedA >= monsterPerWave && spawnedB >= monsterPerWave)
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
        // 적 타입이 enemyDatas 범위를 넘지 않는지 확인
        if (enemyType >= 0 && enemyType < enemyDatas.Count)
        {
            // 스폰 위치 랜덤 선택
            Transform spawnPos = poolSapwnPoint[Random.Range(0, poolSapwnPoint.Length)];

            GameObject enemyObj = ObjectPool.GetObject(); // 오브젝트 풀에서 가져오기
            enemyObj.transform.position = spawnPos.position;
            enemyObj.transform.rotation = Quaternion.identity;

            // 적 초기화
            enemyObj.GetComponent<Ingame_UnitCtrl>().unitData = enemyDatas[enemyType];

            activeMonsters.Add(enemyObj); // 생성된 몬스터 목록에 추가
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

    // 몬스터가 모두 죽었는지 확인
    IEnumerator CheckAllMonstersDead()
    {
        // 몬스터가 모두 죽을 때까지 대기
        while (activeMonsters.Count > 0)
        {
            yield return null;
        }

        Debug.Log("모든 몬스터가 죽었습니다. 다음 웨이브를 준비합니다.");

        // 몬스터가 모두 죽었을 때, 다음 웨이브를 위해 카운트다운 시작
        StartCoroutine(StartNextWaveCountdown());
    }

    // 다음 웨이브 카운트다운 시작
    IEnumerator StartNextWaveCountdown()
    {
        Ingame_UIManager.instance.waveSuccessPanel.SetActive(true);
        Time.timeScale = 0.0f;


        Ingame_UIManager.instance.waveCount = 20f;
        Ingame_UIManager.instance.isCurrentWaveFinshed = true;

        while (Ingame_UIManager.instance.waveCount > 0)
        {
            Ingame_UIManager.instance.waveCount -= Time.deltaTime;
            Ingame_UIManager.instance.waveCountText.text = "적군 침공까지 " + Mathf.Ceil(Ingame_UIManager.instance.waveCount).ToString() + "초";
            yield return null;
        }

        Ingame_UIManager.instance.waveCountText.gameObject.SetActive(false);
        StartCoroutine(StartWaveWithDelay(1f)); // 1초 지연 후 웨이브 시작
    }

    public void NextWave()
    {
        if (isWaveInProgress)
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

        if (!isWaveInProgress && currentWave < waveCount)
        {
            currentWave++;
            StartCoroutine(WaveSystem());
        }
    }


    // 웨이브를 1초 딜레이 후 시작하는 코루틴
    public IEnumerator StartWaveWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 1초 지연
        EnemySpawner.inst.StartCoroutine(EnemySpawner.inst.WaveSystem());
    }
}
