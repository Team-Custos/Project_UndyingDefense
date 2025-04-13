using UnityEngine;
using ArmorType = Unit.ArmorType;

public class AttackSkill : SkillBase
{
    public enum AttackType
    {
        SLASH,
        PIERCE,
        CRUSH,
        NONE
    }

    [Header("■ Data")]
    [SerializeField] private AttackSkillData data;

    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 5;

    protected static Effect slashCritEffect;
    protected static Effect pierceCritEffect;
    protected static Effect crushCritEffect;

    public override SkillData Data => data;

    protected static Effect SlashCritEffect
    {
        get
        {
            if (slashCritEffect == null)
                slashCritEffect = Resources.Load<GameObject>("Prefabs/Effects/CriticalEffects/Bleed").GetComponent<Effect>();

            return slashCritEffect;
        }
    }
    protected static Effect PierceCritEffect
    {
        get
        {
            if (pierceCritEffect == null)
                pierceCritEffect = Resources.Load<GameObject>("Prefabs/Effects/CriticalEffects/Pain").GetComponent<Effect>();

            return pierceCritEffect;
        }
    }
    protected static Effect CrushCritEffect
    {
        get
        {
            if (crushCritEffect == null)
                crushCritEffect = Resources.Load<GameObject>("Prefabs/Effects/CriticalEffects/Shock").GetComponent<Effect>();

            return crushCritEffect;
        }
    }

    public AttackType GetAttackType() => data.AttackType;

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

        int targetCount = Physics.OverlapBoxNonAlloc(pivotTarget.transform.position + Vector3.forward * AreaZ * 0.5f,new Vector3(AreaX,AreaY,AreaZ),targets);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                Attack(unit, target);
            }
        }
    }

    public void ShootProjectile(Unit unit, Unit target, GameObject projectilePrefab)//투사체 발사
    {
        // 투사체 발사
        GameObject projectile = Instantiate(projectilePrefab, unit.transform.position, Quaternion.identity);
        ArrowCtrl arrowCtrl = projectile.GetComponent<ArrowCtrl>();
        float distance = Vector3.Distance(unit.transform.position, target.transform.position);
        projectile.transform.position = unit.transform.position;

        if (arrowCtrl != null)
        {
            arrowCtrl.SetTarget(target);
            arrowCtrl.SetEvent(() => {
                Attack(unit, target);
            });
            arrowCtrl.CalculateTime(distance);
        }

        // 람다식 Lambda Expression
        // 임시 메서드(무명 메서드)

        // ([인수]) => { [코드]들 }


        //projectile.GetComponent<Projectile>().Shoot(target.transform.position, () => Attack(unit, target));
    }


    public void Attack(Unit unit, Unit target)
    {
        float calcDamage = data.Damage;
        float calcCrit = (unit.CritChance + target.CritVulnerability + data.BonusCrit) * 0.01f;
        if (IsBlocked(target.Data.ArmorType))
        {
            float calcBlockRate = 1f - (0.3f * target.BlockRate);
            calcDamage *= calcBlockRate;
        }

        calcDamage += calcDamage * unit.AttackDamageMultiplier * 0.01f;
        calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;
        
        target.TakeDamage(calcDamage);
        if (Random.Range(0f, 1f) <= calcCrit)
            ActivateCriticalEffect(unit, target);
    }

    public void Attack(Unit unit, Fortress fortress)
    {
        float calcDamage = data.Damage;
        fortress.TakeDamage(calcDamage);
    }

    private void ActivateCriticalEffect(Unit unit, Unit target)
    {
        Effect critEffect = null;
        switch(data.AttackType)
        {
            case AttackType.SLASH:
                critEffect = SlashCritEffect;
                break;
            case AttackType.PIERCE:
                critEffect = PierceCritEffect;
                break;
            case AttackType.CRUSH:
                critEffect = CrushCritEffect;
                break;
        }

        target.AddEffect(unit, critEffect);
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
            (data.AttackType == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.AttackType == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.AttackType == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }
}
