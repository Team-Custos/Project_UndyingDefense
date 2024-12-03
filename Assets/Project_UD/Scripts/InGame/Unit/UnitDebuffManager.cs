using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//이 스크립트는 유닛의 현재 디버프를 관리하기 위한 스크립트입니다.

[System.Serializable]
public class UnitCurDebuff
{
    public UnitDebuff name;      // ����� �̸�
    public int stack;            // ����� ����
    public float duration;       // ����� ��ü ���� �ð�
    public float currentTime;    // ����� ���� ���� �ð�
    public int tickDamage;//ƽ ������
    public AudioClip StartSFX;
    public AudioClip EndSFX;
}

public class UnitDebuffManager : MonoBehaviour
{
    UnitDebuffData[] debuffData;
    public GameObject DebuffParticleParent;


    [SerializeField]
    public List<UnitCurDebuff> activeDebuffs = new List<UnitCurDebuff>(); // ���� ������ ����� ���
    public GameObject[] Debuff_OBJ;

    public Ingame_UnitCtrl unitCtrl;
    float tickInterval = 1;
    float tickInterval_cur = 0;

    private void Awake()
    {
        debuffData = InGameManager.inst.unitDebuffData.debuffDatas;
    }

    // �� ������ ����� ������Ʈ
    void Update()
    {
        for (int activeDebuffIdx = activeDebuffs.Count - 1; activeDebuffIdx >= 0; activeDebuffIdx--) // ����Ʈ �������� ��ȸ (���� �� ���� ����)
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

    //����� �� ���� �Լ�
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
                    case UnitDebuff.Dizzy://3�� ���� �̵� �ӵ� 20%, ���� �ӵ� 20% ����. �ִ� 3������ ���õȴ�. 4�� ���ý� ���� ȿ���� �ߵ��ȴ�.
                        if (activeDebuffs[debuffDataIdx].stack >= 4)
                        {
                            RemoveDebuff(activeDebuffs[debuffDataIdx]);
                            AddDebuff(UnitDebuff.Stun);
                        }
                        else
                        {
                            unitCtrl.cur_moveSpeed = unitCtrl.unitData.moveSpeed * 0.8f;
                            unitCtrl.cur_attackSpeed = unitCtrl.unitData.weaponCooldown * 0.8f;
                        }
                        break;
                    case UnitDebuff.Stun://5�� ���� �ൿ �Ұ�
                        unitCtrl.unActable = true;
                        break;
                    case UnitDebuff.Trapped://5�� ���� �̵� �Ұ�
                        unitCtrl.cur_moveSpeed = 0;
                        break;
                    case UnitDebuff.Poison: //3�� ���� �̵� �ӵ�, ���� �ӵ� 20% ����
                        unitCtrl.cur_moveSpeed = unitCtrl.unitData.moveSpeed * 0.8f;
                        unitCtrl.cur_attackSpeed = unitCtrl.unitData.weaponCooldown * 0.8f;
                        break;
                    case UnitDebuff.Bleed: //3�� ���� �ʴ� 1 �������� ������. ������ ���� ������ �������� 2�� ����� �����Ѵ�.
                        int finalDamage = activeDebuffs[debuffDataIdx].tickDamage + (2 * (activeDebuffs[debuffDataIdx].stack - 1));

                        if (activeDebuffs[debuffDataIdx].currentTime > 0)
                        {
                            tickInterval_cur -= Time.deltaTime;
                            if (tickInterval_cur <= 0)
                            {
                                unitCtrl.HP -= finalDamage;
                                tickInterval_cur = tickInterval;
                            }
                        }
                        break;
                    case UnitDebuff.Burn: //3�� ���� �ʴ� 3 �������� ������. �ִ� 3������ ���õȴ�. 4�� ���ý� ȭ�� ȿ���� �ߵ��ȴ�.
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
                    case UnitDebuff.Inferno: //5�� ���� �ʴ� 8 �������� ������. 50% Ȯ���� 10m �ݰ� ������ �ۿ� ȿ���� �߻��Ѵ�.
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
            else // ����� ���� �ð� ��. ����ȭ.
            {
                switch (activeDebuffs[debuffDataIdx].name)
                {
                    case UnitDebuff.Dizzy://3�� ���� �̵� �ӵ� 20%, ���� �ӵ� 20% ����. �ִ� 3������ ���õȴ�. 4�� ���ý� ���� ȿ���� �ߵ��ȴ�.
                        unitCtrl.cur_moveSpeed = unitCtrl.unitData.moveSpeed;
                        unitCtrl.cur_attackSpeed = unitCtrl.unitData.weaponCooldown;
                        break;
                    case UnitDebuff.Stun://5�� ���� �ൿ �Ұ�
                        unitCtrl.unActable = false;
                        break;
                    case UnitDebuff.Trapped://5�� ���� �̵� �Ұ�
                        unitCtrl.cur_moveSpeed = unitCtrl.unitData.moveSpeed;
                        break;
                    case UnitDebuff.Poison: //3�� ���� �̵� �ӵ�, ���� �ӵ� 20% ����
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

        //����� ���� �Լ� �����ؾ���.
    }

    // ����� �߰� �Լ�
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
                else //���� ��� ���ο� ����� �߰�
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
                    Debuff_OBJ[(int)debuff].SetActive(true);
                }    
            } 
        }
    }

            
    // ����� ����
    private void RemoveDebuff(UnitCurDebuff debuff)
    {
        unitCtrl.soundManager.PlaySFX(unitCtrl.soundManager.DEBUFF_SFX, debuff.EndSFX);

        switch (debuff.name)
        {
            case UnitDebuff.Dizzy://3�� ���� �̵� �ӵ� 20%, ���� �ӵ� 20% ����. �ִ� 3������ ���õȴ�. 4�� ���ý� ���� ȿ���� �ߵ��ȴ�.
                unitCtrl.cur_moveSpeed = unitCtrl.unitData.moveSpeed;
                unitCtrl.cur_attackSpeed = unitCtrl.unitData.weaponCooldown;
                break;
            case UnitDebuff.Stun://5�� ���� �ൿ �Ұ�
                unitCtrl.unActable = false;
                break;
            case UnitDebuff.Trapped://5�� ���� �̵� �Ұ�
                unitCtrl.cur_moveSpeed = unitCtrl.unitData.moveSpeed;
                break;
            case UnitDebuff.Poison: //3�� ���� �̵� �ӵ�, ���� �ӵ� 20% ����
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

        Debuff_OBJ[(int)debuff.name].SetActive(false);

        activeDebuffs.Remove(debuff);
    }

    // Ư�� ����� ã�� �Լ�
    public bool HasDebuff(UnitDebuff debuff)
    {
        return activeDebuffs.Exists(d => d.name == debuff);
    }
}