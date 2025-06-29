using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    [SerializeField] private EnemyUnitData enemy;
    [SerializeField] private int count;

    public EnemyUnitData Enemy => enemy; 
    public int Count => count;
} 

[CreateAssetMenu(fileName = "WaveData", menuName = "ProjectUD/WaveData")]
public class WaveData : ScriptableObject
{
    [SerializeField] private int reward;          // 웨이브 클리어 시 보상(골드)
    [SerializeField] private List<EnemySpawnData> monsterSpawnInfos; // 여러 몬스터 타입 및 repeatNum 정보

    public int Reward => reward;
    public IReadOnlyList<EnemySpawnData> MonsterSpawnInfos => monsterSpawnInfos;
}
