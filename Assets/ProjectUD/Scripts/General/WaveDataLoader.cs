using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveDataLoader : MonoBehaviour
{
    [SerializeField] private TextAsset waveDataTable;   // 웨이브 데이터가 저장된 CSV 파일
    [SerializeField] private string path = "UnitData/Enemy";

    private List<List<EnemySpawnData>> waveDataList = new List<List<EnemySpawnData>>();

    private Dictionary<string, EnemyUnitData> spawnDataDic = new Dictionary<string, EnemyUnitData>();
    public List<List<EnemySpawnData>> WaveDataList => waveDataList;


    private void Start()
    {
        LoadSpawnData();

        //for(int i = 0; i < spawnDataList.Count; i++)
        //{
        //    Debug.Log($"웨이브 {spawnDataList[i].waveNum} - ID: {spawnDataList[i].id}, 위치: {spawnDataList[i].pos}, 간격: {spawnDataList[i].interval}");
        //}
    }


    private void LoadSpawnData()
    {
        if (waveDataTable == null)
        {
            Debug.LogError("WaveDataTable 없음");
            return;
        }

        string[] lines = waveDataTable.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 첫 줄은 헤더
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] values;

            if (line.Contains("\t"))
                values = line.Split('\t');
            else
                values = line.Split(',');
            

            if (values.Length < 4)
            {
                Debug.LogWarning($"잘못된 라인 데이터: {line}");
                continue;
            }

            int waveNum = int.Parse(values[0].Trim());
            string id = values[1].Trim();
            string pos = values[2].Trim();
            float interval = float.Parse(values[3].Trim());

            EnemySpawnData data = new EnemySpawnData
            {
                waveNum = waveNum,
                id = id,
                pos = pos,
                interval = interval
            };
            

            //spawnDataList.Add(data);

            while (waveDataList.Count < data.waveNum)
            {
                waveDataList.Add(new List<EnemySpawnData>());
            }

            waveDataList[data.waveNum - 1].Add(data);

        }

    }




    public EnemyUnitData GetEnemyUniData(int curWave, int number)
    {
        string id = waveDataList[curWave][number].id;

        if (spawnDataDic.TryGetValue(id, out EnemyUnitData cachedData))
            return cachedData;

        List<EnemyUnitData> enemyDataList = new List<EnemyUnitData>();
        enemyDataList.AddRange(Resources.LoadAll<EnemyUnitData>("UnitData/Enemy"));
        enemyDataList.AddRange(Resources.LoadAll<EnemyUnitData>("UnitData/Enemy/empire"));
        enemyDataList.AddRange(Resources.LoadAll<EnemyUnitData>("UnitData/Enemy/moor"));
        enemyDataList.AddRange(Resources.LoadAll<EnemyUnitData>("UnitData/Enemy/pioneer"));
        enemyDataList.AddRange(Resources.LoadAll<EnemyUnitData>("UnitData/Enemy/summon"));

        EnemyUnitData loadedData = enemyDataList.Find(data => data.name == id);

        if (loadedData == null)
        {
            Debug.LogError($"[GetEnemyUniData] EnemyUnitData not found: {id}");
            return null;
        }

        spawnDataDic.Add(id, loadedData);

        return loadedData;
    }

}
