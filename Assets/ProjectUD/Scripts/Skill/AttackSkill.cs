using UnityEngine;
using AttackType = AttackData.AttackType;
using ArmorType = Unit.ArmorType;

public class AttackSkill : SkillBase
{
    [Header("■ Data")]
    [SerializeField] private AttackSkillData data;

    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 5;

    //protected static Effect slashCritEffect;
    //protected static Effect pierceCritEffect;
    //protected static Effect crushCritEffect;

    //protected static ParticleSystem slashHitVFX;
    //protected static ParticleSystem pierceHitVFX;
    //protected static ParticleSystem crushHitVFX;
    //protected static ParticleSystem slashCritVFX;
    //protected static ParticleSystem pierceCritVFX;
    //protected static ParticleSystem crushCritVFX;

    public override SkillData Data => data;

    //protected static Effect SlashCritEffect
    //{
    //    get
    //    {
    //        if (slashCritEffect == null)
    //            slashCritEffect = Resources.Load<GameObject>("Prefabs/Effects/CriticalEffects/Bleed").GetComponent<Effect>();

    //        return slashCritEffect;
    //    }
    //}
    //protected static Effect PierceCritEffect
    //{
    //    get
    //    {
    //        if (pierceCritEffect == null)
    //            pierceCritEffect = Resources.Load<GameObject>("Prefabs/Effects/CriticalEffects/Pain").GetComponent<Effect>();

    //        return pierceCritEffect;
    //    }
    //}
    //protected static Effect CrushCritEffect
    //{
    //    get
    //    {
    //        if (crushCritEffect == null)
    //            crushCritEffect = Resources.Load<GameObject>("Prefabs/Effects/CriticalEffects/Shock").GetComponent<Effect>();

    //        return crushCritEffect;
    //    }
    //}


    //protected static ParticleSystem SlashHitVFX
    //{
    //    get
    //    {
    //        if (slashHitVFX == null)
    //            slashHitVFX = Resources.Load<GameObject>("Prefabs/VFX/AttackVFX/Prefeb/Attack/vfx_slashHit_New").GetComponent<ParticleSystem>();

    //        return slashHitVFX;
    //    }
    //}

    //protected static ParticleSystem PierceHitVFX
    //{
    //    get
    //    {
    //        if (pierceHitVFX == null)
    //            pierceHitVFX = Resources.Load<GameObject>("Prefabs/VFX/AttackVFX/Prefeb/Attack/vfx_pierceHit").GetComponent<ParticleSystem>();

    //        return pierceHitVFX;
    //    }
    //}

    //protected static ParticleSystem CrushHitVFX
    //{
    //    get
    //    {
    //        if (crushHitVFX == null)
    //            crushHitVFX = Resources.Load<GameObject>("Prefabs/VFX/AttackVFX/Prefeb/Attack/vfx_crushHit").GetComponent<ParticleSystem>();

    //        return crushHitVFX;
    //    }
    //}

    //protected static ParticleSystem SlashCritVFX
    //{
    //    get
    //    {
    //        if (slashCritVFX == null)
    //            slashCritVFX = Resources.Load<GameObject>("Prefabs/VFX/AttackVFX/Prefeb/Attack/vfx_slashCrit_New").GetComponent<ParticleSystem>();
    //        return slashCritVFX;
    //    }
    //}

    //protected static ParticleSystem PierceCritVFX
    //{
    //    get
    //    {
    //        if (pierceCritVFX == null)
    //            pierceCritVFX = Resources.Load<GameObject>("Prefabs/VFX/AttackVFX/Prefeb/Attack/vfx_pierceCrit").GetComponent<ParticleSystem>();
    //        return pierceCritVFX;
    //    }
    //}

    //protected static ParticleSystem CrushCritVFX
    //{
    //    get
    //    {
    //        if (crushCritVFX == null)
    //            crushCritVFX = Resources.Load<GameObject>("Prefabs/VFX/AttackVFX/Prefeb/Attack/vfx_crushCrit").GetComponent<ParticleSystem>();
    //        return crushCritVFX;
    //    }
    //}

    public void AreaAttack(Unit unit, Unit pivotTarget, float radius, float angle) //부채꼴 공격
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];

        int targetCount = Physics.OverlapSphereNonAlloc(pivotTarget.transform.position, radius, targets, unit.EnemyLayer);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                // 각도 계산
                Attack(unit, target);
            }
        }
    }

    public void AreaAttack(Unit unit, Unit pivotTarget, float radius) //원형 공격
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];
        int targetCount = Physics.OverlapSphereNonAlloc(pivotTarget.transform.position, radius, targets, unit.EnemyLayer);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                Attack(unit, target);
            }
        }
    }

    public void AreaAttack(Unit unit, Unit pivotTarget, float AreaX, float AreaY, float AreaZ)//사각형 공격
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];

        int targetCount = 
            Physics.OverlapBoxNonAlloc(pivotTarget.transform.position + Vector3.forward * AreaZ * 0.5f
                                        ,new Vector3(AreaX,AreaY,AreaZ), targets, Quaternion.identity, unit.EnemyLayer);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                Attack(unit, target);
            }
        }
    }

    public void SelfDestruct(Unit unit, float radius, float hpToTrigger, GameObject BoomEffectPrefab)
    {
        if (unit.Hp <= unit.UnitStats.maxHp * hpToTrigger * 0.01f && unit.Hp > 0)
        {
            if(BoomEffectPrefab != null)
            {
                unit.AddVFX(BoomEffectPrefab.GetComponent<ParticleSystem>());
            }

            if (targets == null)
                targets = new Collider[maxTargetCount];
            int targetCount = Physics.OverlapSphereNonAlloc(unit.transform.position, radius, targets, unit.EnemyLayer);
            for (int i = 0; i < targetCount; i++)
            {
                if (targets[i].TryGetComponent(out Unit target))
                {
                    Attack(unit, target);
                }
            }
            unit.TakeDamage(9999);
        }
    }

    public void ShootProjectile(Unit unit, Unit target, GameObject projectilePrefab)//투사체 발사
    {
        // 투사체 발사
        GameObject projectile = Instantiate(projectilePrefab, unit.transform.position + Vector3.up, unit.transform.rotation);
        float distance = Vector3.Distance(unit.transform.position, target.transform.position);

        if (projectile.TryGetComponent<ArrowCtrl>(out ArrowCtrl arrowCtrl))
        {
            arrowCtrl.SetTarget(target);
            arrowCtrl.SetEvent(() => {
                Attack(unit, target);
            });
            arrowCtrl.CalculateTime(distance);
            arrowCtrl.Shoot((target.transform.position - unit.transform.position).normalized);

        }
        if (projectile.TryGetComponent<GranadeCtrl>(out GranadeCtrl granadeCtrl))
        {
            Vector3 targetPos = target.transform.position;
            projectile.transform.position = this.transform.position;

            float durationTime = 1f;
            granadeCtrl.SetData(data);
            granadeCtrl.SetTargetLayer(unit.EnemyLayer);
            granadeCtrl.JumpTowards(targetPos, durationTime);
        }

        // 람다식 Lambda Expression
        // 임시 메서드(무명 메서드)

        // ([인수]) => { [코드]들 }


        //projectile.GetComponent<Projectile>().Shoot(target.transform.position, () => Attack(unit, target));
    }


    public void Attack(Unit unit, Unit target)
    {
        //if (data.StartVFX != null)
        //{
        //    unit.AddVFX(data.StartVFX);
        //}
        //if (data.StartSFX.Length > 0)
        //{
        //    int randomSoundIdx = Random.Range(0, data.StartSFX.Length);
        //    if (data.StartSFX[randomSoundIdx] != null)
        //    {
        //        SoundManager.Instance.PlaySFX(data.StartSFX[randomSoundIdx]);
        //    }
        //}

        float calcDamage = data.Damage;
        float calcCrit = (unit.CritPercent + target.CritVulnerability + data.BonusCritPercent) * 0.01f;
        if (IsBlocked(target.Data.ArmorType))
        {
            float calcBlockRate = 1f - (0.3f * target.BlockPercent * 0.01f);
            calcDamage *= calcBlockRate;
        }

        calcDamage *= Mathf.Max(0f, unit.AtkMult);
        calcDamage *= Mathf.Max(0f, target.DamageTakenMult);

        //calcDamage += calcDamage * unit.AttackDamageMultiplier * 0.01f;
        //calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;

        target.TakeDamage(calcDamage);
        if (Random.Range(0f, 1f) <= calcCrit)
        {
            // target.PlayCritSFX(data.Info.Type);
            AddCritVFX(unit, target);
            ActivateCriticalEffect(unit, target);
        }
        else
        {
            //target.PlayHitSFX(data.AttackType);
            AddHitVFX(unit, target);
        }
        
        //if (data.InduseEffect != null)
        //{
        //    if (Random.Range(0f, 1f) <= data.InduseEffectSuccessRate * 0.01f)
        //    {
        //        target.AddEffect(unit, data.InduseEffect);
        //    }
        //}
    }

    public void AttackFortress(Unit unit, Fortress fortress, UnitData data)
    {
        int damage = data.Tier;

        fortress.TakeDamage(damage);
    }

    private void ActivateCriticalEffect(Unit unit, Unit target)
    {
        //Effect critEffect = null;
        //switch(data.AttackType)
        //{
        //    case AttackType.SLASH:
        //        critEffect = SlashCritEffect;
        //        break;
        //    case AttackType.PIERCE:
        //        critEffect = PierceCritEffect;
        //        break;
        //    case AttackType.CRUSH:
        //        critEffect = CrushCritEffect;
        //        break;
        //}

        target.AddEffect(data.Info.CritEffectPrefab);
    }

    private void AddHitVFX(Unit unit, Unit target)
    {
        //ParticleSystem hitVFX = null;
        //switch (data.AttackType)
        //{
        //    case AttackType.SLASH:
        //        hitVFX = SlashHitVFX;
        //        break;
        //    case AttackType.PIERCE:
        //        hitVFX = PierceHitVFX;
        //        break;
        //    case AttackType.CRUSH:
        //        hitVFX = CrushHitVFX;
        //        break;
        //}

        // target.AddVFX(hitVFX, unit.transform.position);
    }

    private void AddCritVFX(Unit unit, Unit target)
    {
        //ParticleSystem critVFX = null;
        //switch (data.AttackType)
        //{
        //    case AttackType.SLASH:
        //        critVFX = SlashCritVFX;
        //        break;
        //    case AttackType.PIERCE:
        //        critVFX = PierceCritVFX;
        //        break;
        //    case AttackType.CRUSH:
        //        critVFX = CrushCritVFX;
        //        break;
        //}
        //target.AddVFX(critVFX, unit.transform.position);
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
            (data.Info.Type == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.Info.Type == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.Info.Type == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }
}
