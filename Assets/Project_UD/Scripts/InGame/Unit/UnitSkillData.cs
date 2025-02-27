using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 기본 데이터를 관리하기 위한 스크립트입니다. (스크립터블 오브젝트 사용.)

[CreateAssetMenu(fileName = "UnitSkillData", menuName = "Scriptable Object/UnitSkillData", order = int.MaxValue)]
public class UnitSkillData : ScriptableObject
{                                                                 
    [Header("====General====")]
    public AttackSkillType skillType; //스킬 타입
    public int damage = 0; //데미지
    public float coolTime = 0; //쿨타임
    public float bounsCrit = 0; //보너스 크리티컬
    public AttackType attackType = AttackType.UnKnown; //공격 타입
    public UnitDebuff debuff = UnitDebuff.None; //디버프
    public bool AttackTargetPoint = true;

}                 
