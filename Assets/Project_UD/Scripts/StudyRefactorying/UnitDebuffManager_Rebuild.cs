using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDebuffManager_Rebuild : MonoBehaviour
{
    UnitDebuffData[] debuffData;
    public GameObject DebuffParticleParent;


    [SerializeField]
    public List<UnitCurDebuff> activeDebuffs = new List<UnitCurDebuff>(); // ���� ������ ����� ���
    public GameObject[] Debuff_OBJ;

    public UnitCtrl_ReBuild unitCtrl;
    float tickInterval = 1;
    float tickInterval_cur = 0;

    private void Awake()
    {
        debuffData = InGameManager.inst.unitDebuffData.debuffDatas;
    }

    // �� ������ ����� ������Ʈ
    void FixedUpdate()
    {
        if (unitCtrl.unitState == UnitState.Dead)
        {
            DebuffParticleParent.SetActive(false);
            return;
        }

        for (int activeDebuffIdx = activeDebuffs.Count - 1; activeDebuffIdx >= 0; activeDebuffIdx--)
        {
            activeDebuffs[activeDebuffIdx].currentTime -= Time.deltaTime;

            if (activeDebuffs.Count > 0)
            {
                // ����� �ð��� ���� ���
                if (activeDebuffs[activeDebuffIdx].currentTime <= 0)
                {
                    RemoveDebuff(activeDebuffs[activeDebuffIdx]);
                }
                else
                {
                    DebuffUpdate();
                }
            }
        }
    }

    void DebuffUpdate()
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
                    case UnitDebuff.Dizzy:
                        if (activeDebuffs[debuffDataIdx].stack >= 4)
                        {
                            RemoveDebuff(activeDebuffs[debuffDataIdx]);
                            AddDebuff(UnitDebuff.Stun);
                        }
                        else
                        {
                            unitCtrl.ChangeMoveSpeed(unitCtrl.unitData.baseMoveSpeed * 0.8f);
                            //unitCtrl.cur_attackSpeed = unitCtrl.unitData.attackSpeed * 0.8f;
                        }
                        break;
                    case UnitDebuff.Stun:
                        unitCtrl.GetComponent<UnitAnimationParaCtrl>().animator.SetBool(CONSTANT.ANIBOOL_STUNEND, false);
                        //unitCtrl.unActable = true;
                        break;
                    case UnitDebuff.Trapped:
                        unitCtrl.ChangeMoveSpeed(0);
                        break;
                    case UnitDebuff.Poison:
                        unitCtrl.ChangeMoveSpeed(unitCtrl.unitData.baseMoveSpeed * 0.8f);
                        //unitCtrl.cur_attackSpeed = unitCtrl.unitData.attackSpeed * 0.8f;
                        break;
                    case UnitDebuff.Bleed:
                        int finalDamage = activeDebuffs[debuffDataIdx].tickDamage + (2 * (activeDebuffs[debuffDataIdx].stack - 1));

                        if (activeDebuffs[debuffDataIdx].currentTime > 0)
                        {
                            tickInterval_cur -= Time.deltaTime;
                            if (tickInterval_cur <= 0)
                            {
                                unitCtrl.TakeDamage(finalDamage);
                                tickInterval_cur = tickInterval;
                            }
                        }
                        break;
                    case UnitDebuff.Burn:
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
                                unitCtrl.TakeDamage(activeDebuffs[debuffDataIdx].tickDamage);
                                tickInterval_cur = tickInterval;
                            }
                        }

                        break;
                    case UnitDebuff.Inferno:
                        while (activeDebuffs[debuffDataIdx].currentTime > 0)
                        {
                            tickInterval_cur -= Time.deltaTime;
                            if (tickInterval_cur <= 0)
                            {
                                unitCtrl.TakeDamage(activeDebuffs[debuffDataIdx].tickDamage);

                                tickInterval_cur = tickInterval;
                            }
                        }
                        break;
                }
            }
            else //디버프 종료시 처리
            {
                switch (activeDebuffs[debuffDataIdx].name)
                {
                    case UnitDebuff.Dizzy:
                        unitCtrl.ChangeMoveSpeed(unitCtrl.unitData.baseMoveSpeed);
                        //unitCtrl.cur_attackSpeed = unitCtrl.unitData.attackSpeed;
                        break;
                    case UnitDebuff.Stun:
                        //unitCtrl.unActable = false;
                        break;
                    case UnitDebuff.Trapped:
                        unitCtrl.ChangeMoveSpeed(unitCtrl.unitData.baseMoveSpeed);
                        break;
                    case UnitDebuff.Poison:
                        unitCtrl.ChangeMoveSpeed(unitCtrl.unitData.baseMoveSpeed);
                        //unitCtrl.cur_attackSpeed = unitCtrl.unitData.attackSpeed;
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

    // 디버프 추가
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
            if (debuff == debuffData[i].name) // �̹� �ش� ������� �ִ��� Ȯ��
            {
                UnitCurDebuff existingDebuff = activeDebuffs.Find(d => d.name == debuffData[i].name);
                if (existingDebuff != null) // ������� �̹� ������ �ð� �ʱ�ȭ �� ���� ����
                {
                    existingDebuff.currentTime = debuffData[i].duration;
                    if (debuffData[i].Stackable && existingDebuff.stack <= debuffData[i].stackLimit)
                    {
                        existingDebuff.stack++;
                    }
                }
                else
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
                    unitCtrl.soundManager.PlaySFX(StartSFX);
                    Debuff_OBJ[(int)debuff].SetActive(true);
                    if (debuff == UnitDebuff.Stun)
                    {
                        unitCtrl.GetComponent<UnitAnimationParaCtrl>().animator.SetTrigger(CONSTANT.ANITRIGGER_STUN);
                    }
                    //TODO : 기절시 애니메이션 파라미터 설정 필요. (IsStun)
                }
            }
        }
    }

    private void RemoveDebuff(UnitCurDebuff debuff)
    {
        unitCtrl.soundManager.PlaySFX(debuff.EndSFX);

        switch (debuff.name)
        {
            case UnitDebuff.Dizzy://3�� ���� �̵� �ӵ� 20%, ���� �ӵ� 20% ����. �ִ� 3������ ���õȴ�. 4�� ���ý� ���� ȿ���� �ߵ��ȴ�.
                unitCtrl.ChangeMoveSpeed(unitCtrl.unitData.baseMoveSpeed);
                //unitCtrl.cur_attackSpeed = unitCtrl.unitData.attackSpeed;
                break;
            case UnitDebuff.Stun://5�� ���� �ൿ �Ұ�
                //unitCtrl.unActable = false;
                unitCtrl.GetComponent<UnitAnimationParaCtrl>().animator.SetBool(CONSTANT.ANIBOOL_STUNEND, true);
                //TODO : 애니메이션 파라미터 초기화 필요. (StunEnd)
                break;
            case UnitDebuff.Trapped://5�� ���� �̵� �Ұ�
                unitCtrl.ChangeMoveSpeed(unitCtrl.unitData.baseMoveSpeed);
                break;
            case UnitDebuff.Poison: //3�� ���� �̵� �ӵ�, ���� �ӵ� 20% ����
                unitCtrl.ChangeMoveSpeed(unitCtrl.unitData.baseMoveSpeed);
                //unitCtrl.cur_attackSpeed = unitCtrl.unitData.attackSpeed;
                break;
            case UnitDebuff.Bleed:
                break;
            case UnitDebuff.Burn:
                break;
            case UnitDebuff.Inferno:
                break;
        }

        Debuff_OBJ[(int)debuff.name].SetActive(false);

        activeDebuffs.Remove(debuff);
        unitCtrl.GetComponent<UnitAnimationParaCtrl>().animator.SetBool(CONSTANT.ANIBOOL_STUNEND, true);
    }

    // Ư�� ����� ã�� �Լ�
    public bool HasDebuff(UnitDebuff debuff)
    {
        return activeDebuffs.Exists(d => d.name == debuff);
    }
}
