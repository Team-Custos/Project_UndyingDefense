using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class ClearCurseEffectAbility : SpecialAbility
{
    [SerializeField] private float range = 5f; // 효과 작동 범위
    [SerializeField] private GameObject vfx;
    private UltEvent<Unit, DurationEffect> onClearCurse; // 저주 해제 시 이벤트
    private IReadOnlyList<Unit> appliedCurseEffectUnitList;

    // 능력 소유자 설정 및 이벤트 등록
    public override void Initialize(Unit unit)
    {
        base.Initialize(unit);

        appliedCurseEffectUnitList = null;

        if (unit is AllyUnit allyUnit && allyUnit.Spawner != null)
            appliedCurseEffectUnitList = allyUnit.Spawner.AppliedCurseEffectUnitList;

        // 저주 해제 이벤트 등록 및 중복 방지
        onClearCurse -= ClearCurseEffect;
        onClearCurse += ClearCurseEffect;
    }

    private void Update()
    {
        if (unit.IsDead)
            return;

        if (appliedCurseEffectUnitList.Count > 0)
        {
            for (int i = appliedCurseEffectUnitList.Count - 1; i >= 0; i--)
            {
                Unit target = appliedCurseEffectUnitList[i];

                if (target == null || target.IsDead)
                    continue;

                float distance = Vector3.Distance(unit.transform.position, target.transform.position);

                if (distance <= range)
                    ClearAllCurseEffect(target);

            }
        }
    }

    private void ClearCurseEffect(Unit target, DurationEffect effect)
    {
        target.RemoveEffect(effect);
        effect.RemoveEffect();
        Debug.Log($"{effect.Id} 해제");
    }


    // 대상에게 적용된 모든 저주 정화
    private void ClearAllCurseEffect(Unit target)
    {
        bool curseCleared = false;

        for (int i = target.EffectList.Count - 1; i >= 0; i--)
        {
            DurationEffect effect = target.EffectList[i];

            if (effect == null || effect.Type != EffectType.CURSE)
                continue;

            onClearCurse?.Invoke(target, effect);
            curseCleared = true;
        }

        // VFX는 한번만 재생
        if (curseCleared)
            target.AddVFX(vfx, target.transform);
    }

    // 능력 대상자 이벤트 등록
    //public override void Register(Unit target)
    //{
    //    if (target == null || target.IsDead)
    //        return;

    //    target.onGetCurseEffect -= ApplyAbility;
    //    target.onGetCurseEffect += ApplyAbility;
    //}


    //// 능력 대상자 이벤트 해제
    //public override void Unregister(Unit target)
    //{
    //    if (target == null)
    //        return;

    //    target.onGetCurseEffect -= ApplyAbility;
    //    appliedCurseEffectUnitList.Remove(target);
    //}





    // 발동할 능력 내용
    //private void ApplyAbility(Unit target, DurationEffect effect)
    //{
    //    if (target.IsDead) return;

    //    // 이펙트가 존재하는 지 확인(다른 유닛에 의해 이미 제거)
    //    if (effect == null || effect.Type != EffectType.CURSE)
    //        return;

    //    float distance = Vector3.Distance(unit.transform.position, target.transform.position);

    //    if (distance <= range)
    //    {
    //        onClearCurse?.Invoke(target, effect);   // 거리가 범위 내일 경우 저주 바로 해제
    //        target.AddVFX(vfx, target.transform);
    //    }
    //    else if (!appliedCurseEffectUnitList.Contains(target))
    //    {
    //        // 대상은 리스트에 한번만 추가
    //        appliedCurseEffectUnitList.Add(target);
    //    }
    //}


}
