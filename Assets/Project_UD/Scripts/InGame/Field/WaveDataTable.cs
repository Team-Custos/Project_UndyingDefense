    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "WaveDataTable", menuName = "Scriptable Object/WaveDataTable", order = 2)]
    public class WaveDataTable : ScriptableObject
    {
        public List<WaveData> waves; // 여러 웨이브 데이터를 담는 리스트

        // waveNumber로 웨이브 데이터 검색하는 메서드
        public WaveData GetWaveData(int waveNumber)
        {
            return waves.Find(w => w.waveNumber == waveNumber);
        }
    }
