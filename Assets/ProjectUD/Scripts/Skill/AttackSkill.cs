using UnityEngine;
using AttackType = AttackData.AttackType;
using ArmorType = Unit.ArmorType;
using Unity.VisualScripting;
using System.Collections.Generic;

public class AttackSkill : SkillBase
{
    [Header("■ Data")]
    [SerializeField] private AttackSkillData data;
    [SerializeField] private GameObject skillVfx;
    private VFXObjectPool vfxPool;

    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 100;

    //protected static Effect slashCritEffect;
    //protected static Effect pierceCritEffect;
    //protected static Effect crushCritEffect;

    protected static ParticleSystem slashHitVFX;
    protected static ParticleSystem pierceHitVFX;
    protected static ParticleSystem crushHitVFX;
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

    //------------------------------------------------------------------------------------------------------------------------
    protected static ParticleSystem SlashHitVFX
    {
        get
        {
            if (slashHitVFX == null)
                slashHitVFX = Resources.Load<GameObject>("Prefabs/VFX/AttackVFX/Prefeb/Attack/vfx_slashHit_New").GetComponent<ParticleSystem>();

            return slashHitVFX;
        }
    }

    protected static ParticleSystem PierceHitVFX
    {
        get
        {
            if (pierceHitVFX == null)
                pierceHitVFX = Resources.Load<GameObject>("Prefabs/VFX/AttackVFX/Prefeb/Attack/vfx_pierceHit").GetComponent<ParticleSystem>();

            return pierceHitVFX;
        }
    }

    protected static ParticleSystem CrushHitVFX
    {
        get
        {
            if (crushHitVFX == null)
                crushHitVFX = Resources.Load<GameObject>("Prefabs/VFX/AttackVFX/Prefeb/Attack/vfx_crushHit").GetComponent<ParticleSystem>();

            return crushHitVFX;
        }
    }
    //------------------------------------------------------------------------------------------------------------------------
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
        //if (targets == null)
        //    targets = new Collider[maxTargetCount];

        //int targetCount = Physics.OverlapSphereNonAlloc(pivotTarget.transform.position, radius, targets, unit.EnemyLayer);
        //for (int i = 0; i < targetCount; i++)
        //{
        //    if (targets[i].TryGetComponent(out Unit target))
        //    {
        //        // 각도 계산
        //        Attack(unit, target);

        //    }
        //}

        if (targets == null)
            targets = new Collider[maxTargetCount];

        // AoE 중심이 pivotTarget이면 그대로 두고,
        // unit을 중심으로 하고 싶다면 아래 한 줄을 unit.transform.position 으로 바꾸면 됨.
        int targetCount = Physics.OverlapSphereNonAlloc(pivotTarget.transform.position, radius, targets, unit.EnemyLayer);

        float half = angle * 0.5f;
        Vector3 forward = unit.transform.forward;

        for (int i = 0; i < targetCount; i++)
        {
            if (!targets[i].TryGetComponent(out Unit target))
                continue;

            // unit 기준 방향 벡터 (y 무시해 평면 각도만 계산 권장)
            Vector3 dir = target.transform.position - unit.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f)
                continue;

            // unit.forward와의 각도(0~180°)
            float ang = Vector3.Angle(forward, dir);

            // 좌/우 angle/2 범위 안이면 타격
            if (ang <= half)
            {
                Attack(unit, target);
            }
        }

        if (skillVfx != null)
        {
            float halfAngle = angle * 0.5f;

            List<Vector3> angles = new List<Vector3>();

            // 중심 방향(정면)
            Vector3 centerDir = unit.transform.forward;
            angles.Add(centerDir);

            // 좌측 끝 방향
            Vector3 leftDir = Quaternion.Euler(0, -halfAngle, 0) * centerDir;
            angles.Add(leftDir);

            // 우측 끝 방향
            Vector3 rightDir = Quaternion.Euler(0, halfAngle, 0) * centerDir;
            angles.Add(rightDir);

            for (int i = 0; i < 3; i++)
            {
                //VFX vfx = obj.GetComponent<VFX>();
                vfxPool = unit.SkillVfxPool;

                GameObject obj = vfxPool.GetVFX(skillVfx, unit);
                obj.transform.position = unit.transform.position;
                Debug.Log(obj.transform.position);
                obj.SetActive(true);

                VFX vfx = obj.GetComponent<VFX>();
                vfx.SetDirection(angles[i]);
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

                if(skillVfx != null)
                {
                    vfxPool = unit.SkillVfxPool;

                    GameObject obj = vfxPool.GetVFX(skillVfx, unit);
                    obj.transform.position = target.transform.position;
                    obj.SetActive(true);
                }
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

        if(skillVfx != null)
        {
            vfxPool = unit.SkillVfxPool;

            GameObject obj = vfxPool.GetVFX(skillVfx, unit);
            obj.transform.position = unit.transform.position;
            Debug.Log(obj.transform.position);
            obj.SetActive(true);

            Vector3 dircetion = unit.transform.forward;
            VFX vfx = obj.GetComponent<VFX>();
            vfx.SetDirection(dircetion);
        }
    }

    public void SelfDestruct(Unit unit, float radius, float hpToTrigger, GameObject BoomEffectPrefab)
    {
        if (unit.Hp <= unit.UnitStats.maxHp * hpToTrigger * 0.01f && unit.Hp >= 0f)
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
            //unit.TakeDamage(9999);
            unit.Die();
        }
    }

    public void ShootProjectile(Unit unit, Unit target, Fortress fortress, GameObject projectilePrefab, bool isUnit)//투사체 발사
    {
        // 투사체 발사
        GameObject projectile = Instantiate(projectilePrefab, unit.transform.position + Vector3.up, unit.transform.rotation);

        float distance;

        if (isUnit)
            distance = Vector3.Distance(unit.transform.position, target.transform.position);
        else
            distance = Vector3.Distance(unit.transform.position, fortress.transform.position);


        if (projectile.TryGetComponent<ArrowCtrl>(out ArrowCtrl arrowCtrl))
        {
            if(isUnit)
            {
                arrowCtrl.SetTarget(target);
                arrowCtrl.SetEvent(() => {
                    Attack(unit, target);
                });
                arrowCtrl.CalculateTime(distance);
                arrowCtrl.Shoot((target.transform.position - unit.transform.position).normalized);
            }
            else
            {
                arrowCtrl.SetTarget(fortress);
                arrowCtrl.SetEvent(() => {
                    AttackFortress(unit, fortress);
                });
                arrowCtrl.CalculateTime(distance);
                arrowCtrl.Shoot((fortress.transform.position - unit.transform.position).normalized);
            }
            

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
            float calcBlockRate = 1f - (0.5f * target.BlockPercent);    // 단위수정_AYO
            calcDamage *= calcBlockRate;
        }

        calcDamage *= Mathf.Max(0f, unit.AtkMult);      // 공격력 계산
        calcDamage *= Mathf.Max(0f, target.DamageTakenMult);    // 피해량 계산

        //calcDamage += calcDamage * unit.AttackDamageMultiplier * 0.01f;
        //calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;

        //if (target is EnemyUnit)
        //    Debug.Log(calcDamage);


        if (Random.Range(0f, 1f) <= calcCrit)
        {
            // target.PlayCritSFX(data.Info.Type);
            AddCritVFX(unit, target);
            AddCritSFX(target.transform.position);
            ActivateCriticalEffect(unit, target);
        }
        else
        {
            AddHitVFX(unit, target);
            AddHitSFX(target.transform.position);
        }

        target.TakeDamage(calcDamage);
    }

    public void AttackFortress(Unit unit, Fortress fortress)
    {
        fortress.TakeDamage(unit.Data.Tier);
    }


    private void ActivateCriticalEffect(Unit unit, Unit target)
    {
        target.AddEffect(data.Info.CritEffectPrefab, target);

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

        //if (target.HasEffect<StunEffect>())
        //{
        //    Debug.Log("Stun 적용중");
        //    return;
        //}
    }

    public void ProvokeUnits(Unit unit, Unit target)
    {

    }

    private void AddHitVFX(Unit unit, Unit target)
    {
        GameObject hitVFX = data.Info.HitVFX;
        if (hitVFX != null)
        {
            target.AddVFX(hitVFX, unit.transform);
        }

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
        GameObject critVFX = data.Info.CritVFX;
        if (critVFX != null)
        {
            target.AddVFX(critVFX, unit.transform);
        }

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
    public void AddHitSFX(Vector3 pos)
    {
        AudioClip[] audios = data.Info.HitSFXClip;
        AudioClip audio = audios[Random.Range(0, audios.Length)];
        SoundManager.Instance.PlaySFX(audio, pos);
    }

    public void AddCritSFX(Vector3 pos)
    {
        SoundManager.Instance.PlaySFX(data.Info.CritSFXClip, pos);
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
            (data.Info.Type == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.Info.Type == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.Info.Type == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }
}
