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

    public int level;
    public GameObject modelPrefab;
    public int maxHP;                                             
    public int mental = 1;                                        
    public float moveSpeed;                                       
                                                                  
    public AnimatorOverrideController overrideController;         
                                                                  
    [Header("====Attack====")]                                    
    public int critChanceRate;                                  
    public UnitSkillData generalSkill;                            
    public UnitSkillData specialSkill;                                                          
    public float sightRange = 0;                                  
    public float attackRange = 0;                   
    public AudioClip[] attackSound;

    public GameObject attackVFX;

    // 추가
    [Header("============")]
    public string name;                                           
    public string unitCode;                                       
    public int cost;
    public int gold;
    public string g_SkillName;                                    
    public string s_SkillName;                                    
    public string g_SkillInfo;
    public string s_SkillInfo;
    public Sprite g_SkillImage;
    public Sprite s_SkillImage;
    public Sprite unitImage;

    [Header("====Upgrade Options====")]
    public List<string> upgradePaths;  // 업그레이드 가능 코드 리스트
    public int upgrade1Cost;
    public int upgrade2Cost;
    public Sprite uupgrade1Image;
    public Sprite uupgrade2Image;
}                 
