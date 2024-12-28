using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MonsterSpawnInfo
{
    public int monsterType; 
    public int repeatNum;      // 이 몬스터 타입을 몇 마리 소환할지
    public bool isBoss;
}


[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Object/WaveData", order = 1)]
public class WaveData : ScriptableObject
{
    public int waveNumber;      // 웨이브 번호

    public float waveStartTime; // 이전 웨이브 종료 후 다음 웨이브 시작까지 대기 시간(초 단위)
    public float interval;      // 몬스터 스폰 간격(초 단위)
    public int reward;          // 웨이브 클리어 시 보상(골드)

    public List<MonsterSpawnInfo> monsterSpawnInfos; // 여러 몬스터 타입 및 repeatNum 정보


    public int TotalMonsterCount
    {
        get
        {
            int count = 0;
            foreach (var info in monsterSpawnInfos)
            {
                count += info.repeatNum;
            }
            return count;
        }
    }
}