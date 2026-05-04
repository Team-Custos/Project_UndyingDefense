using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using ArmorType = Unit.ArmorType;
using AttackType = AttackData.AttackType;

public class SpecialAbility : MonoBehaviour
{
    public enum ActiveType // 발동 조건
    {
        ALWAYS,     // 상시 ex) 금강불괴
        HP,         // HP에 따라 ex) 자폭
        KILL,       // 대상을 처치했을 때 ex) 흡혈
        DIE,        // 사망 시 ex) 독구름
        MENTAL      // 멘탈에 따라 ex)
    }

    [SerializeField] private new string name;
    [SerializeField] private string id;
    //private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private UltEvent<Unit> onActivate;
    [SerializeField] private ActiveType activeType;
    [SerializeField] private AudioClip audioClip; // 스킬 발동 시 재생할 오디오
    //[SerializeField] private SpecialAbilityData data;

    public string Name => name;
    public string Id => id;
    //public string Description => description;
    public Sprite Icon => icon;
    public ActiveType ActiveCondition => activeType;

    public void Activate(Unit unit)
    {
        if (onActivate != null)
        {
            onActivate.Invoke(unit);
        }
    }

    // 자폭
    public void SelfDestruct(Unit unit, float radius, float hpToTrigger, GameObject BoomEffectPrefab, float damage, AttackData attackData)
    {
        if (unit.IsDead)
            return;

        if (unit.Hp <= unit.UnitStats.maxHp * hpToTrigger * 0.01f && unit.Hp >= 0f)
        {
            if (BoomEffectPrefab != null)
            {
                unit.AddVFX(BoomEffectPrefab.GetComponent<ParticleSystem>());
            }

            Collider[] targets = null;
            int maxTargetCount = 10;

            if (targets == null)
                targets = new Collider[maxTargetCount];
            int targetCount = Physics.OverlapSphereNonAlloc(unit.transform.position, radius, targets, unit.EnemyLayer);
            for (int i = 0; i < targetCount; i++)
            {
                if (targets[i].TryGetComponent(out Unit target))
                {
                    Attack(unit, target, damage, attackData);
                }
            }

            unit.Die();
            Debug.Log("자폭");
        }
    }

    // 금강불괴
    public void DiamondBody(Unit unit, float value)
    {
        unit.AddDamageTakenMult(value);
        Debug.Log("금강불괴");
    }

   // 공격 특수 능력 용 데미지 계산
    private void Attack(Unit unit, Unit target, float damage, AttackData attackData)
    {

        float calcDamage = damage;
        float calcCrit = (unit.CritPercent + target.CritVulnerability) * 0.01f;

        if (IsBlocked(target.Armortype, attackData.Type))
        {
            float calcBlockRate = 1f - (0.5f * target.BlockPercent);    // 단위수정_AYO
            calcDamage *= calcBlockRate;

            calcCrit -= 0.5f; // 치명타율 감소
            if (calcCrit < 0f)
                calcCrit = 0f;

            //Debug.Log($"치명타 율 : {calcCrit}");
        }


        calcDamage *= Mathf.Max(0f, unit.AtkMult);      // 공격력 계산
        calcDamage *= Mathf.Max(0f, target.DamageTakenMult);    // 피해량 계산

        if (Random.Range(0f, 1f) <= calcCrit)
        {
            target.AddVFX(attackData.CritVFX, unit.transform);
            SoundManager.Instance.PlaySFX(attackData.CritSFXClip, target.transform.position);
            target.AddEffect(attackData.CritEffectPrefab, target, Vector3.zero);
        }
        else
        {
            target.AddVFX(attackData.HitVFX, unit.transform);
            int random = Random.Range(0, attackData.HitSFXClip.Length);
            SoundManager.Instance.PlaySFX(attackData.HitSFXClip[random], target.transform.position);
        }

        target.TakeDamage(calcDamage);
        //Debug.Log(target.Data.Name + ":" + calcDamage);
    }

    private bool IsBlocked(ArmorType armorType, AttackType attackType)
    {
        if (armorType == ArmorType.NONE)
            Debug.Log("방어 타입 없음");
        return
            (attackType == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (attackType == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (attackType == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }
}
