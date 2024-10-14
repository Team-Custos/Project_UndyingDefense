using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitCurDebuff
{
    public UnitDebuff name;      // 디버프 이름
    public int stack;            // 디버프 스택
    public float duration;       // 디버프 전체 지속 시간
    public float currentTime;    // 디버프 현재 남은 시간
    public int tickDamage;//틱 데미지
    public AudioClip StartSFX;
    public AudioClip EndSFX;
}

public class UnitDebuffManager : MonoBehaviour
{
    UnitDebuffData[] debuffData;

    [SerializeField]
    public List<UnitCurDebuff> activeDebuffs = new List<UnitCurDebuff>(); // 현재 유닛의 디버프 목록

    public Ingame_UnitCtrl unitCtrl;
    float tickInterval = 1;
    float tickInterval_cur = 0;

    private void Awake()
    {
        debuffData = InGameManager.inst.unitStatusCtrl.debuffDatas;
    }

    // 매 프레임 디버프 업데이트
    void Update()
    {
        for (int activeDebuffIdx = activeDebuffs.Count - 1; activeDebuffIdx >= 0; activeDebuffIdx--) // 리스트 역순으로 순회 (삭제 시 오류 방지)
        {
            activeDebuffs[activeDebuffIdx].currentTime -= Time.deltaTime;

            // 디버프 시간이 끝난 경우
            if (activeDebuffs[activeDebuffIdx].currentTime <= 0)
            {
                RemoveDebuff(activeDebuffs[activeDebuffIdx]);
            }
            else
            {
                if (activeDebuffs.Count > 0)
                {
                    debuffUpdate();
                }
            }
        }        
    }

    //디버프 실 적용 함수
    void debuffUpdate()
    {
        for (int debuffDataIdx = 0; debuffDataIdx < debuffData.Length - 1; debuffDataIdx++)
        {
            if (debuffDataIdx >= activeDebuffs.Count)
            {
                break;
            }

            if (activeDebuffs[debuffDataIdx].currentTime > 0)
            {
                switch (activeDebuffs[debuffDataIdx].name)
                {
                    case UnitDebuff.Dizzy://3초 동안 이동 속도 20%, 공격 속도 20% 감소. 최대 3번까지 스택된다. 4번 스택시 기절 효과가 발동된다.
                        if (activeDebuffs[debuffDataIdx].stack >= 4)
                        {
                            RemoveDebuff(activeDebuffs[debuffDataIdx]);
                            AddDebuff(UnitDebuff.Stun);
                        }
                        else
                        {
                            unitCtrl.cur_moveSpeed = unitCtrl.cur_moveSpeed * 0.8f;
                            unitCtrl.cur_attackSpeed = unitCtrl.cur_attackSpeed * 0.8f;
                        }
                        break;
                    case UnitDebuff.Stun://5초 동안 행동 불가
                        unitCtrl.unActable = true;
                        break;
                    case UnitDebuff.Trapped://5초 동안 이동 불가
                        unitCtrl.cur_moveSpeed = 0;
                        break;
                    case UnitDebuff.Poison: //3초 동안 이동 속도, 공격 속도 20% 감소
                        unitCtrl.cur_moveSpeed = unitCtrl.cur_moveSpeed * 0.8f;
                        unitCtrl.cur_attackSpeed = unitCtrl.cur_attackSpeed * 0.8f;
                        break;
                    case UnitDebuff.Bleed: //3초 동안 초당 1 데미지를 입힌다. 스택이 쌓일 때마다 데미지는 2의 배수로 증가한다.
                        int finalDamage = activeDebuffs[debuffDataIdx].tickDamage + (2 * (activeDebuffs[debuffDataIdx].stack - 1));

                        if(activeDebuffs[debuffDataIdx].currentTime > 0)
                        {
                            tickInterval_cur -= Time.deltaTime;
                            if (tickInterval_cur <= 0)
                            {
                                unitCtrl.HP -= finalDamage;
                                tickInterval_cur = tickInterval;
                            }
                        }
                        break;
                    case UnitDebuff.Burn: //3초 동안 초당 3 데미지를 입힌다. 최대 3번까지 스택된다. 4번 스택시 화염 효과가 발동된다.
                        if (activeDebuffs[debuffDataIdx].stack >= 4)
                        {
                            RemoveDebuff(activeDebuffs[debuffDataIdx]);
                            AddDebuff(UnitDebuff.Inferno);
                        }
                        while (activeDebuffs[debuffDataIdx].currentTime > 0)
                        {
                            tickInterval_cur -= Time.deltaTime;
                            if (tickInterval_cur <= 0)
                            {
                                unitCtrl.HP -= activeDebuffs[debuffDataIdx].tickDamage;
                                tickInterval_cur = tickInterval;
                            }
                        }

                        break;
                    case UnitDebuff.Inferno: //5초 동안 초당 8 데미지를 입힌다. 50% 확률로 10m 반경 적에게 작열 효과가 발생한다.
                        while (activeDebuffs[debuffDataIdx].currentTime > 0)
                        {
                            tickInterval_cur -= Time.deltaTime;
                            if (tickInterval_cur <= 0)
                            {
                                unitCtrl.HP -= activeDebuffs[debuffDataIdx].tickDamage;
                                tickInterval_cur = tickInterval;
                            }
                        }
                        break;
                }
            }
            else // 디버프 지속 시간 끝. 정상화.
            {
                switch (activeDebuffs[debuffDataIdx].name)
                {
                    case UnitDebuff.Dizzy://3초 동안 이동 속도 20%, 공격 속도 20% 감소. 최대 3번까지 스택된다. 4번 스택시 기절 효과가 발동된다.
                        unitCtrl.cur_moveSpeed = unitCtrl.unitData.moveSpeed;
                        unitCtrl.cur_attackSpeed = unitCtrl.unitData.weaponCooldown;
                        break;
                    case UnitDebuff.Stun://5초 동안 행동 불가
                        unitCtrl.unActable = false;
                        break;
                    case UnitDebuff.Trapped://5초 동안 이동 불가
                        unitCtrl.cur_moveSpeed = unitCtrl.unitData.moveSpeed;
                        break;
                    case UnitDebuff.Poison: //3초 동안 이동 속도, 공격 속도 20% 감소
                        unitCtrl.cur_moveSpeed = unitCtrl.unitData.moveSpeed;
                        unitCtrl.cur_attackSpeed = unitCtrl.unitData.weaponCooldown;
                        break;
                    case UnitDebuff.Bleed:
                        break;
                    case UnitDebuff.Burn:
                        break;
                    case UnitDebuff.Inferno:
                        break;
                }
            }
        }
    }

    // 디버프 추가 함수
    public void AddDebuff(UnitDebuff debuff)
    {
        AudioClip StartSFX;

        if (debuffData == null)
        {
            Debug.LogError("debuffData is null. Check the source of debuffData.");
            return;
        }

        for (int i = 0; i < debuffData.Length; i++)
        {
            if (debuff == debuffData[i].name) // 이미 해당 디버프가 있는지 확인
            {
                UnitCurDebuff existingDebuff = activeDebuffs.Find(d => d.name == debuffData[i].name);
                                               
                if (existingDebuff != null) // 디버프가 이미 있으면 시간 초기화 및 스택 증가
                {
                    existingDebuff.currentTime = debuffData[i].duration;
                    if (existingDebuff.stack < debuffData[i].stackLimit)
                    {
                        existingDebuff.stack++;
                    }
                }
                else //없을 경우 새로운 디버프 추가
                {
                    activeDebuffs.Add(new UnitCurDebuff
                    {
                        name = debuffData[i].name,
                        stack = 1,
                        duration = debuffData[i].duration,
                        currentTime = debuffData[i].duration,
                        tickDamage = debuffData[i].tickDamage,
                        StartSFX = debuffData[i].StartSFX,
                        EndSFX = debuffData[i].EndSFX,
                    });
                    StartSFX = debuffData[i].StartSFX;
                    unitCtrl.soundManager.PlaySFX(unitCtrl.soundManager.DEBUFF_SFX, StartSFX);
                }
            }
            
        }
    }

    // 디버프 제거
    private void RemoveDebuff(UnitCurDebuff debuff)
    {
        unitCtrl.soundManager.PlaySFX(unitCtrl.soundManager.DEBUFF_SFX, debuff.EndSFX);
        activeDebuffs.Remove(debuff);
    }

    // 특정 디버프 찾기 함수
    public bool HasDebuff(UnitDebuff debuff)
    {
        return activeDebuffs.Exists(d => d.name == debuff);
    }
}

