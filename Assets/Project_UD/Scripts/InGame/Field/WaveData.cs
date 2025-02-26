using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MonsterSpawnInfo
{
    public int monsterType;    // 몬스터 타입 -> 게임오브젝트로 변경 예정
    public int count;      // 이 몬스터 타입을 몇 마리 소환할지
}


[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Object/WaveData", order = 1)]
public class WaveData : ScriptableObject
{
    public int waveNumber;      // 웨이브 번호 -> 삭제 예정
    //public float waveStartTime; // 이전 웨이브 종료 후 다음 웨이브 시작까지 대기 시간(초 단위) -> 삭제 예정
    //public float interval;      // 몬스터 스폰 간격(초 단위) -> 삭제 예정

    public int reward;          // 웨이브 클리어 시 보상(골드)

    [SerializeField] private List<MonsterSpawnInfo> monsterSpawnInfos; // 여러 몬스터 타입 및 repeatNum 정보
    public IReadOnlyList<MonsterSpawnInfo> MonsterSpawnInfos => monsterSpawnInfos;
}