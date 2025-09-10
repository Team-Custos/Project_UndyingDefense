using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class WaveInfo
{
    public int number;
    public int reward;
    public int type;
    public Dictionary<string, List<int>> enemyInfo = new Dictionary<string, List<int>>();
}

public class WaveDataLoader : MonoBehaviour
{
    [SerializeField] private TextAsset excelWaveData;

    private List<WaveInfo> waveDataList = new List<WaveInfo>();

    private void Start()
    {
        LoadWaveData();
    }

    public List<WaveInfo> LoadWaveData()
    {
        string[] lines = excelWaveData.text.Split('\n');

        for (int lineIndex = 1; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex];
            if (string.IsNullOrWhiteSpace(line))
                continue;
            
            string[] parts = line.Split(',');

            int waveNumber = int.Parse(parts[0].Trim());
            int reward = int.Parse(parts[1].Trim());
            int type = int.Parse(parts[2].Trim());

            WaveInfo waveInfo = new WaveInfo
            {
                number = waveNumber,
                reward = reward,
                type = type,
            };

            for (int i = 3; i < 3 + type * 2; i += 2)
            {
                string id = parts[i].Trim();
                int count = int.Parse(parts[i + 1].Trim());

                if (!waveInfo.enemyInfo.ContainsKey(id))
                {
                    waveInfo.enemyInfo[id] = new List<int>();
                }
                waveInfo.enemyInfo[id].Add(count);
            }

            waveDataList.Add(waveInfo);
        }

        foreach (var wave in waveDataList)
        {
            Debug.Log($"웨이브 {wave.number} / 보상 {wave.reward} / 적 종류 {wave.type}");

            foreach (var enemy in wave.enemyInfo)
            {
                Debug.Log($"적 ID: {enemy.Key}, 스폰 수: [{string.Join(", ", enemy.Value)}]");
            }
        }

        return waveDataList;
    }
}
