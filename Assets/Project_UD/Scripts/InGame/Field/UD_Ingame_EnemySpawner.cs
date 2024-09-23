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

public class UD_Ingame_EnemySpawner : MonoBehaviour
{
    public static UD_Ingame_EnemySpawner inst;
    //UD_Ingame_ObjectPool objectPool;

    UD_Ingame_GridManager gridManager;

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
    bool isCurrentWaveFinshed = false;


    private void Awake()
    {
       inst = this;
        
    }

    private void Start()
    {
        gridManager = UD_Ingame_GameManager.inst.gridManager;

        mobSpawnPosX = gridManager._width;
        gridHeight = gridManager._height;

        spawnPoint = new Transform[gridHeight];

        UD_Ingame_ObjectPool.Instance.Intialize(20);

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
                if (gridManager.Tiles_Obj[idx].GetComponent<UD_Ingame_GridTile>().GridPos.x == mobSpawnPosX - 1)
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
        //Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2(X, Y);
        Obj.GetComponent<UD_Ingame_UnitCtrl>().EnemyInit(spawnData[enemyType]);
        return Obj;
    }

    public IEnumerator WaveSystem()
    {
        isWaveInProgress = true;
        spawnedMonsterCount = 0;

        Debug.Log($"Wave {currentWave} 시작");

        UD_Ingame_UIManager.instance.isCurrentWaveFinshed = true;

        yield return StartCoroutine(SpawnMonstersForWave());

        yield return StartCoroutine(CheckAllMonstersDead());

        Debug.Log("모든 몬스터 처치 완료. 다음 웨이브로...");

        isWaveInProgress = false;
    }

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
            // 3~10 웨이브는 A, B 몬스터가 각각 5마리씩 생성
            int spawnedA = 0;
            int spawnedB = 0;

            while (spawnedA < monsterPerWave || spawnedB < monsterPerWave)
            {
                int enemyType = Random.Range(0, 2);

                if (enemyType == 0 && spawnedA < monsterPerWave)
                {
                    SpawnEnemy(0);
                    spawnedA++;
                }
                else if (enemyType == 1 && spawnedB < monsterPerWave)
                {
                    SpawnEnemy(1);
                    spawnedB++;
                }

                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    void SpawnEnemy(int enemyType)
    {
        Transform spawnPos = poolSapwnPoint[Random.Range(0, poolSapwnPoint.Length)];

        GameObject enemyObj = UD_Ingame_ObjectPool.GetObject(); 
        enemyObj.transform.position = spawnPos.position;
        enemyObj.transform.rotation = Quaternion.identity;

        if (enemyType == 0)
        {
            enemyObj.GetComponent<UD_Ingame_UnitCtrl>().EnemyInit(spawnData[enemyType]); // A 몬스터 초기화
        }
        else
        {
            enemyObj.GetComponent<UD_Ingame_UnitCtrl>().EnemyInit(spawnData[enemyType]); // B 몬스터 초기화
        }

        activeMonsters.Add(enemyObj); 
    }

    // 몬스터 죽음 처리
    public void OnMonsterDead(GameObject monster)
    {
        if (activeMonsters.Contains(monster))
        {
            activeMonsters.Remove(monster);
            UD_Ingame_ObjectPool.ReturnObject(monster); 
        }
    }

    // 몬스터가 모두 죽었는지 확인
    IEnumerator CheckAllMonstersDead()
    {
        while (activeMonsters.Count > 0) 
        {
            yield return null;
        }

        UD_Ingame_UIManager.instance.isCurrentWaveFinshed = true;
    }

    public void NextWave()
    {
        UD_Ingame_UIManager.instance.isCurrentWaveFinshed = true;

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
}
