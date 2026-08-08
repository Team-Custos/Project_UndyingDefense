using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;
using ArmorType = Unit.ArmorType;
using AttackType = AttackData.AttackType;

public class SpecialAbility : MonoBehaviour
{
    public enum ActiveType // 발동 조건
    {
        ALWAYS,     // 상시 ex) 금강불괴
        HP,         // HP에 따라 ex) 자폭
        KILL,       // 대상을 처치했을 때 ex) 흡혈
        ATTACK,     // 공격 시 ex) 
        TAKE_DAMAGE, // 피해를 입었을 때 ex) 원한
        DEAD   // 적이 죽었을 때 ex) 영생
    }

    [SerializeField] private string id;
    //private string description;
    [SerializeField] private Sprite icon;
    [SerializeField] private UltEvent<Unit, Unit> onActivate;
    [SerializeField] private ActiveType activeType;
    [SerializeField] private AudioClip audioClip; // 능력 발동 시 재생할 오디오
    [SerializeField] private GameObject auraVFX;
    [SerializeField] private Unit unit;

    private Collider[] targets;
    private float damage;

    public string Name => name;
    public string Id => id;
    //public string Description => description;
    public Sprite Icon => icon;
    public ActiveType ActiveCondition => activeType;
    public float Damage { get; set; }

    private void Start()
    {
        AuraVFXScaleMult();
    }

    public void Activate(Unit unit, Unit target)
    {
        if (onActivate != null)
        {
            onActivate.Invoke(unit, target);
        }
    }

    // 자폭
    public void SelfDestruct(Unit unit, float radius, float hpToTrigger, GameObject BoomEffectPrefab, float damage, AttackData attackData)
    {
        if (unit.IsDead)
        {
            Debug.Log("자폭 미발동");
            return;
        }
            

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
        }
    }

    // 금강불괴
    public void DiamondBody(Unit unit, float value)
    {
        unit.AddDamageTakenMult(value);
        Debug.Log("금강불괴");
    }

    // 흡혈
    public void LifeSteal(Unit unit, float percent, GameObject vfx)
    {
        unit.RecoveryHp(Damage * percent, null);
        unit.AddVFX(vfx, unit.transform.position);
    }

    // 원한
    public void Resent(Unit target, float percent, GameObject effect, GameObject vfx)   
    {
        float randomValue = Random.value; // 0 ~1 사이의 랜덤 값 생성 ex) 0.01, 0.3 0.9

        if (randomValue <= percent)
        {
            Debug.Log("원한 발동 :" + randomValue);
            target.AddEffect(effect, target, Vector3.zero);
            target.AddVFX(vfx, target.transform.position, true, target.VfxScaleMult(target.Data.Tier));
            SoundManager.Instance.PlaySFX(audioClip, target.transform.position);
        }
    }

    // 영생
    public void Immortality(Unit unit, Unit target, float range, float percent, GameObject vfx)
    {
        float distance = Vector3.Distance(unit.transform.position, target.transform.position);

        if (distance <= range)
        {
            float hpAmount = target.Maxhp * percent;

            unit.RecoveryHp(hpAmount, null);
            Debug.Log("회복량 : " + hpAmount);
            unit.AddVFX(vfx, unit.transform.position);
            SoundManager.Instance.PlaySFX(audioClip, unit.transform.position);
        }
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

        target.TakeDamage(calcDamage, unit);
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

    private void AuraVFXScaleMult()
    {
        if (auraVFX == null)
            return;

        auraVFX.transform.localScale = Vector3.one * unit.VfxScaleMult(unit.Data.Tier);
    }    
}
