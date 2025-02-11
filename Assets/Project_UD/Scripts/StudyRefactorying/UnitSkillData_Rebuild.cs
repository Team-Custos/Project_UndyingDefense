using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "UnitSkillDataReBuild", menuName = "Scriptable Object/UnitSkillDataReBuild", order = int.MaxValue - 1)]
public class UnitSkillData_Rebuild : ScriptableObject
{
    [Header("====General====")]
    public string skillName;
    public float skillCoolDown;
    [Header("====Attack====")]
    public AttackType attackType;
    public int skillDamage; //스킬 데미지 공격 스킬이 아닐 경우 0.
    public float skillCritChanceRate; //스킬 치명타 확률
    public UnitDebuff debuff2Apply;
    //다른 것도 추가할 예정
}
