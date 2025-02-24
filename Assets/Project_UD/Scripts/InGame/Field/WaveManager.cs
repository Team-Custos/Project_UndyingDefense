using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WaveManager : MonoBehaviour
{
    public static WaveManager inst;
    [SerializeField] private WaveCanvasController waveCanvasController;

    public WaveDataTable waveDataTable;                 // WaveData를 담고 있는 ScriptableObject
    public List<Ingame_UnitData> enemyDatas;            // 적 데이터 리스트

    public Transform[] waveSapwnPoint;                  // 적 스폰 지점
    public GameObject Test_Enemy;                       // 적 게임 오브젝트
    private int enemypriority = 0;                      // 적 NavMeshAgent Priority

    public int currentWave = 1;                             // 현재 웨이브 단계 

    [SerializeField] private List<GameObject> activeMonsters = new List<GameObject>();      // 생성된 몬스터 리스트

    public bool isWaveing;  // 웨이브가 진행중인지 확인
    [SerializeField] private bool isSpawning; // 몬스터 스폰 중인지 확인

    public int maxWave = 10;

    [Header("웨이브에 필요한 시간 변수")]
    [SerializeField] private float waveTimer = 20.0f;   // 웨이브 종료 후 대기 시간 (현재는 20초로 고정)
    [SerializeField] private float spawnTimer = 3.0f;   // 몬스터 스폰 간격 (현재는 3초로 고정)
    [SerializeField] private float curSpawnTimer = 0.0f;   // 웨이브 시작 후 대기 시간 (현재는 20초로 고정)
    [SerializeField] private float waveDelay = 1.0f;    // 웨이브, ui 간 텀

    private WaveData waveData;

    public bool isBaseAttackPerWave;  // 웨이브에 Base가 공격당했는지 확인

    [Header("Monster Spawn Info")]
    [SerializeField] private int monSpawnIndex;     // 몬스터 스폰 인덱스
    [SerializeField] private int monSpawnCount;     // 인덱스의 몬스터 소환 횟수
    [SerializeField] private int totalMonCount;     // 현재 생성되어있는 모든 몬스터 수


    private void Awake()
    {
        inst = this;

        // Resources 폴더에서 프리팹 로드
        GameObject canvasWavePrefab = Resources.Load<GameObject>("Canvas_wave");
        if (canvasWavePrefab != null)
        {
            // 프리팹을 인스턴스화하고 현재 오브젝트의 자식으로 추가
            GameObject canvasInstance = Instantiate(canvasWavePrefab, transform);

            waveCanvasController = canvasInstance.GetComponent<WaveCanvasController>();

        }
        else
        {
            Debug.Log("프리팹을 찾을 수 없습니다: Canvas_wave");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (waveCanvasController.waveCountSkipBtn != null)
        {
            waveCanvasController.waveCountSkipBtn.onClick.AddListener(() =>
            {
                waveTimer = 0.0f;
            });
        }

        curSpawnTimer = spawnTimer;
    }

    // Update is called once per frame
    void Update()
    {
        if (isWaveing)
        {
            if (!isSpawning && totalMonCount == 0)
            {
                if (currentWave <= maxWave)
                {
                    waveDelay -= Time.deltaTime;

                    if (waveDelay <= 0.0f)
                    {
                        // 웨이브 대기 시작
                        waveCanvasController.waveCountTextPanel.SetActive(true);
                        waveTimer -= Time.deltaTime;

                        // 웨이브 대기 시간을 표시하는 타이머
                        waveCanvasController.waveCountText.text = "적군 침공까지 " + Mathf.Ceil(waveTimer) + "초";

                        if (waveTimer <= 0.0f) // && waveTimer > -1.0f) // 카운트가 끝나면 웨이브 시작
                        {
                            waveCanvasController.waveCountTextPanel.SetActive(false);

                            waveCanvasController.waveStartPanel.SetActive(true);
                            StartWave();
                            waveTimer = 20.0f;
                        }


                    }
                }

            }
            else if (isSpawning)
            {
                curSpawnTimer -= Time.deltaTime;
                if (curSpawnTimer <= 0)
                {
                    curSpawnTimer = spawnTimer;
                    SpawnMonster();
                    totalMonCount++;
                    Debug.Log("총 몬스터 수 " + totalMonCount);
                }
            }





        }
        else
        {
            //if (waveCanvasController.waveDefenseSuccessPaenl.activeSelf == true)
            //{
            //    uiDuration -= Time.deltaTime;
            //    {
            //        if (uiDuration <= 0.0f)
            //        {
            //            waveCanvasController.waveDefenseSuccessPaenl.SetActive(false);
            //        }
            //    }
            //}

            // 웨이브 종료후 대기 시간 
            waveDelay -= Time.deltaTime;
            if (waveDelay <= 0.0f)
            {
                EndWave();
                waveCanvasController.waveDefenseSuccessPaenl.SetActive(false);
                waveDelay = 1.0f;
            }
        }
    }

    private void StartWave()
    {
        waveDelay = 4.0f;

        waveCanvasController.waveStepText.text = "웨이브 " + currentWave;

        waveCanvasController.waveStartText.text = currentWave + "차 침공 시작!";

        // WaveData를 가져옴
        waveData = waveDataTable.waves[currentWave - 1];


        if (waveData == null)
        {
            Debug.LogError("해당 웨이브 데이터를 찾을 수 없습니다: " + currentWave);
            return;
        }

        Debug.Log($"Wave {currentWave} 시작");

        monSpawnCount = waveData.MonsterSpawnInfos[monSpawnIndex].count; // 웨이브가 시작 될때 첫번째 몬스터 카운트 가져옴

        isSpawning = true;
    }

    private void SpawnMonster()
    {
        if (waveData.MonsterSpawnInfos.Count > 0)  // 소환할 몬스터가 있는지 확인
        {
            SpawnEnemy(waveData.MonsterSpawnInfos[monSpawnIndex].monsterType);

            monSpawnCount--;

            if (monSpawnCount == 0) // 해당 인덱스의 마지막 몬스터까지 소환이 되었으면  
            {
                // 인덱스가 마지막이 아니면
                if (monSpawnIndex < waveData.MonsterSpawnInfos.Count - 1)
                {
                    monSpawnIndex++;
                }
                else // 마지막 이면
                {
                    Debug.Log("모든 몬스터 소환 완료!");
                    isSpawning = false;
                }

                // 스폰 카운트 새 인덱스의 카운트로 설정
                monSpawnCount = waveData.MonsterSpawnInfos[monSpawnIndex].count;
            }

        }
    }

    private void EndWave()
    {
        monSpawnIndex = 0;

        if (waveData.waveNumber == maxWave)
        {
            // 게임 종료 ui 생성, bgm 재생

            //PlayerPrefs.SetInt("PlayerGold", InGameManager.inst.gold);
            //PlayerPrefs.Save();

            //// 저장된 값 확인
            //Debug.Log("저장된 골드: " + PlayerPrefs.GetInt("PlayerGold", -1));
        }
        else
        {
            // 웨이브 클리어 ui
        }

        SoundManager.instance.PlayWaveSFX(SoundManager.waveSfx.sfx_waveWin);

        InGameManager.inst.gold += waveData.reward;
        Ingame_UIManager.instance.goldTxt.text = InGameManager.inst.gold.ToString();
        Debug.Log($"[Wave {currentWave}] 클리어! 보상 {waveData.reward} 획득");

        currentWave++;

        WaveData nextWaveData = waveDataTable.GetWaveData(currentWave);

        //if(waveTimer >= 5.0f) // 웨이브간 대기 시간
        //{
        //    if (nextWaveData != null)
        //    {
        //        SoundManager.instance.PlayWaveSFX(SoundManager.waveSfx.sfx_wavePrepare);
        //    }

        //    waveTimer = 0f;

        //}



        isWaveing = true;


    }


    void SpawnEnemy(int enemyType)
    {
        Transform spawnPos = waveSapwnPoint[Random.Range(0, waveSapwnPoint.Length)];
        Vector3 newPosition = spawnPos.position;
        newPosition.y = -0.9f;
        spawnPos.position = newPosition;
        GameObject enemyObj = Instantiate(Test_Enemy, spawnPos.position, Quaternion.identity);
        enemyObj.GetComponent<Ingame_UnitCtrl>().unitData = enemyDatas[enemyType];
        activeMonsters.Add(enemyObj);
        enemyObj.GetComponent<NavMeshAgent>().avoidancePriority = enemypriority % 50;
        enemypriority++;
        enemyObj.name = Test_Enemy.name + enemypriority.ToString();
        SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_enemySpawn);
    }

    public void MonsterDead(GameObject monster, float destroyDelay)
    {
        Destroy(monster, destroyDelay);
        totalMonCount--;
        Debug.Log("총 몬스터 수 " + totalMonCount);

        if (totalMonCount == 0 && !isSpawning)
        {
            if (currentWave == maxWave)
            {
                waveCanvasController.waveResultWinPanel.SetActive(true);

                PlayerPrefs.SetInt("PlayerGold", InGameManager.inst.gold);
                PlayerPrefs.Save();

                // 저장된 값 확인
                Debug.Log("저장된 골드: " + PlayerPrefs.GetInt("PlayerGold", -1));

                Time.timeScale = 0.0f;

            }
            else
            {
                // 웨이브 종료시 성공 판넬 생성
                waveCanvasController.waveDefenseSuccessPaenl.SetActive(true);

                isWaveing = false;


            }
        }



        //if (activeMonsters.Contains(monster))
        //{
        //    activeMonsters.Remove(monster);
        //    Destroy(monster, destroyDelay);

        //    if(activeMonsters.Count == 0)
        //    {
        //        isWaveing = false;
        //    }
        //}
    }

    public void OnBaseAttacked()
    {
        if (!isBaseAttackPerWave)
        {
            //Ingame_WaveUIManager.instance.ShowUI(Ingame_WaveUIManager.instance.waveWarnningPanel, 3.0f);
            isBaseAttackPerWave = true;
        }
    }

}