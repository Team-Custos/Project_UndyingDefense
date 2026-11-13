using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class ClearCurseEffectAbility : SpecialAbility
{
    [SerializeField] private float range = 5f; // 효과 작동 범위
    [SerializeField] private GameObject vfx;
    private List<Unit> hasCurseEffectUnitList;

    private UltEvent<Unit, DurationEffect> onClearCurse;
    private AllyUnitSpawner allyUnitSpawner;

    // 능력 소유자 설정 및 이벤트 등록
    public override void Initialize(Unit unit)
    {
        base.Initialize(unit);

        if (unit is AllyUnit allyUnit)
        {
            allyUnitSpawner = allyUnit.Spawner;
            allyUnitSpawner.onUnitAddEffect -= AddList;
            allyUnitSpawner.onUnitAddEffect += AddList;
            hasCurseEffectUnitList = allyUnitSpawner.SearchHasCurseEffectUnits();
        }
    }

    public override void Remove(Unit unit)
    {
        base.Remove(unit);

        allyUnitSpawner.onUnitAddEffect -= AddList;
    }

    private void Update()
    {
        if (unit.IsDead)
            return;

        if (hasCurseEffectUnitList.Count > 0)
        {
            for (int i = hasCurseEffectUnitList.Count - 1; i >= 0; i--)
            {
                Unit target = hasCurseEffectUnitList[i];

                // 대상이 죽었으면 리스트에서 제거
                if (target.IsDead)
                {
                    hasCurseEffectUnitList.Remove(target);
                    continue;
                }

                if(!CheckUnitHasCurseEffect(target))    // 대상이 더 이상 저주 상태가 없으면 리스트에서 제거(시간 초과, 다른 유닛의 정화 능력)
                {
                    hasCurseEffectUnitList.Remove(target);
                }
                else
                {
                    float distance = Vector3.Distance(unit.transform.position, target.transform.position);

                    if (distance <= range)
                        ClearAllCurseEffect(target);
                }
            }
        }
    }

    public void AddList(Unit target, DurationEffect effect)
    {
        if (!hasCurseEffectUnitList.Contains(target) && effect.Type == EffectType.CURSE)
        {
            hasCurseEffectUnitList.Add(target);
        }
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

            Debug.Log($"{effect.name} : 제거");
            effect.Remove();
            curseCleared = true;
        }

        // 리스트 제걱 및 VFX 한번만 발생
        if (curseCleared)
        {
            hasCurseEffectUnitList.Remove(target);
            target.AddVFX(vfx, target.transform);
        }
    }

    private bool CheckUnitHasCurseEffect(Unit target)
    {
        for (int i = 0; i < target.EffectList.Count; i++)
        {
            DurationEffect effect = target.EffectList[i];

            if (effect != null && effect.Type == EffectType.CURSE)
                return true;
        }

        Debug.Log("저주 제거 되어 있음");
        return false;
    }
}
