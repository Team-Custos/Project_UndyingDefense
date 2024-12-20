using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 기본 데이터를 관리하기 위한 스크립트입니다. (스크립터블 오브젝트 사용.)

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Object/UnitData", order = int.MaxValue)]
public class Ingame_UnitData : ScriptableObject
{                                                                 
    [Header("====General====")]                                   
    public UnitType unitType;                                     
    public DefenseType defenseType;                               
    public TargetSelectType targetSelectType;                     
                                                                  
    public int modelType;
    public GameObject modelPrefab;
    public int maxHP;                                             
    public int mental = 1;                                        
    public float moveSpeed;                                       
                                                                  
    public AnimatorOverrideController overrideController;         
                                                                  
    [Header("====Attack====")]                                    
    public int attackPoint = 1;                                   
    public int critChanceRate;                                    
    public int generalSkillCode = 101;                            
    public int specialSkillCode = 101;                            
    public float attackSpeed = 0;                             
    public float skillCooldown = 0;                               
    public float sightRange = 0;                                  
    public float attackRange = 0;                   
    public AudioClip[] attackSound;

    public AnimationClip[] tauntAniClip;
                                                                  
    // 추가                                                        
    public float globalTime;                                       
    public string g_SkillName;                                    
    public string s_SkillName;                                    
    public string unitCode;                                       
    public int level;                                             
    public int cost;
    public int gold;
    public string name;
    public string g_SkillInfo;
    public string s_SkillInfo;
}                 
