using UnityEngine;
using AttackType = AttackData.AttackType;
using ArmorType = Unit.ArmorType;
using Unity.VisualScripting;

public class AttackSkill : SkillBase
{
    [Header("■ Data")]
    [SerializeField] private AttackSkillData data;

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
    public void SectorAttack(Unit unit, float radius, float angle) //부채꼴 공격
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];

        // 1. [수정] 원형 탐색의 기준점을 완벽하게 시전자(unit)의 위치로 설정
        int targetCount = Physics.OverlapSphereNonAlloc(unit.transform.position, radius, targets, unit.EnemyLayer);

        float half = angle * 0.5f;
        Vector3 forward = unit.transform.forward;

        for (int i = 0; i < targetCount; i++)
        {
            if (!targets[i].TryGetComponent(out Unit target))
                continue;

            // unit을 기준으로 적이 있는 방향 계산
            Vector3 dir = target.transform.position - unit.transform.position;
            dir.y = 0f;

            // unit의 정면(forward)과 적을 향한 방향(dir) 사이의 각도를 측정
            float ang = Vector3.Angle(forward, dir);

            // 좌/우 angle/2 (half) 범위 안이면 타격
            if (ang <= half)
            {
                Attack(unit, target);
            }
        }

        
    }

    


    public void AreaAttack(Unit unit, Unit pivotTarget, float radius, float angle, GameObject vfx) //부채꼴 공격
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];

        int targetCount = Physics.OverlapSphereNonAlloc(pivotTarget.transform.position, radius, targets, unit.EnemyLayer);

        float half = angle * 0.5f;
        Vector3 forward = unit.transform.forward;

        for (int i = 0; i < targetCount; i++)
        {
            if (!targets[i].TryGetComponent(out Unit target))
                continue;

            Vector3 dir = target.transform.position - unit.transform.position;
            dir.y = 0f;

            float ang = Vector3.Angle(forward, dir);

            // 좌/우 angle/2 범위 안이면 타격
            if (ang <= half)
            {
                Attack(unit, target);
                target.AddVFX(vfx, target.transform);
            }
        }
    }

    public void AreaAttack(Unit unit, float radius) // 자기 중심 원형 공격
    {
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
    }

    public void AreaAttack(Unit unit, Unit pivotTarget, float radius) //원거리 원형 공격
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

    

    

    //public void AreaAttack(Unit unit, Unit pivotTarget, float radius, GameObject vfxPrefab)
    //{

    //}





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

    // 유닛이라보는 방향으로 사각형 공격
    public void AreaAttack(Unit unit, float AreaX, float AreaZ)
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];

        // 1. 사각형 중심 계산 (사거리 AreaX의 절반만큼 전방으로)
        Vector3 center = unit.transform.position + unit.transform.forward * (AreaX * 0.5f);

        // 2. 사각형의 절반 크기 설정 (X에 좌우폭 AreaZ, Z에 사거리 AreaX)
        Vector3 half = new Vector3(AreaZ * 0.5f, 0.5f, AreaX * 0.5f);

        int targetCount = Physics.OverlapBoxNonAlloc(center, half, targets, unit.transform.rotation, unit.EnemyLayer);
        Debug.Log($"타겟 수 : {targetCount}");

        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                Attack(unit, target);
            }
        }

        // 3. 실시간으로 변하는 값을 기즈모 변수에 저장
        //gizmoUnit = unit;
        //gizmoX = AreaZ; // 기즈모의 가로폭 (X축)
        //gizmoZ = AreaX; // 기즈모의 세로폭 (Z축)
    }

    // 기즈모 디버깅을 위한 변수들
    //private Unit gizmoUnit;
    //private float gizmoX;
    //private float gizmoZ;

    //private void OnDrawGizmosSelected()
    //{
    //    // 시전자 유닛이 없으면 그리지 않음
    //    if (gizmoUnit == null)
    //        return;

    //    // 바뀐 규칙에 맞게 중심점 재계산 (Y축 높이는 살짝 띄워줌)
    //    Vector3 center = gizmoUnit.transform.position + gizmoUnit.transform.forward * (gizmoZ * 0.5f);
    //    center.y += 0.1f;

    //    // DrawWireCube는 '절반(half)'이 아니라 '전체 크기(size)'를 요구하므로 2를 곱하지 않고 그대로 씁니다.
    //    Vector3 size = new Vector3(gizmoX, 0.5f, gizmoZ);

    //    // 기즈모 색상을 붉은색으로 설정
    //    Gizmos.color = Color.red;

    //    // [중요] 유닛이 회전할 때 기즈모 상자도 같이 회전하도록 매트릭스 정렬
    //    Gizmos.matrix = Matrix4x4.TRS(center, gizmoUnit.transform.rotation, Vector3.one);

    //    // 매트릭스가 중심(center)을 기준으로 잡혀있으므로, 로컬 좌표인 Vector3.zero 위치에 상자를 그립니다.
    //    Gizmos.DrawWireCube(Vector3.zero, size);
    //}

    public void ShowVFX(Unit unit, Unit target, GameObject vfxPrefab)
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        unit.AddVFX(vfxPrefab, dir);
    }





    public void SelfDestruct(Unit unit, float radius, float hpToTrigger, GameObject BoomEffectPrefab)
    {
        if (unit.IsDead)
            return;

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
        float calcDamage = data.Damage;
        float calcCrit = (unit.CritPercent + target.CritVulnerability + data.BonusCritPercent) * 0.01f;

        if(!data.IgnoreDefenseType)
        {
            if (IsBlocked(target.Armortype))
            {
                float calcBlockRate = 1f - (0.5f * target.BlockPercent);    // 단위수정_AYO
                calcDamage *= calcBlockRate;

                calcCrit -= 0.5f; // 치명타율 감소
                if (calcCrit < 0f)
                    calcCrit = 0f;

                //Debug.Log($"치명타 율 : {calcCrit}");
            }
        }

        calcDamage *= Mathf.Max(0f, unit.AtkMult);      // 공격력 계산
        calcDamage *= Mathf.Max(0f, target.DamageTakenMult);    // 피해량 계산

        if (Random.Range(0f, 1f) <= calcCrit)
        {
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
        //Debug.Log("데미지 : " + calcDamage);


        // 스킬 별 효과 발동 확률 계산 후 효과 발동
        if (data.InduseEffectPrefab != null && Random.Range(0f, 1f) <= CalculateEffectPercent(unit, target))
        {
            target.AddEffect(data.InduseEffectPrefab, target, Vector3.zero);
        }

        // 적 처치 특수 능력 발동
        if (target.IsDead)
        {
            //Debug.Log("회복 전 체력 : " + unit.Hp);
            unit.ActivateSpecialAbility(SpecialAbility.ActiveType.KILL);
            //Debug.Log("회복 후 체력 :" + unit.Hp);
        }
    }

    public void AttackFortress(Unit unit, Fortress fortress)
    {
        fortress.TakeDamage(unit.Data.Tier);
    }


    private void ActivateCriticalEffect(Unit unit, Unit target)
    {
        if (data.Info.CritEffectPrefab == null)
            return;

        target.AddEffect(data.Info.CritEffectPrefab, target, Vector3.zero);

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
        if(data.Info == null)
            return;

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

        if(audios.Length > 0)
        {
            AudioClip audio = audios[Random.Range(0, audios.Length)];
            SoundManager.Instance.PlaySFX(audio, pos);
        }
        
    }

    public void AddCritSFX(Vector3 pos)
    {
        SoundManager.Instance.PlaySFX(data.Info.CritSFXClip, pos);
    }

    private bool IsBlocked(ArmorType armorType)
    {
        if(armorType == ArmorType.NONE)
            Debug.Log("방어 타입 없음");

        if (data.Info == null)
            return false;

        return
            (data.Info.Type == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.Info.Type == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.Info.Type == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }

    
    private float CalculateEffectPercent(Unit unit, Unit target)
    {
        float finalPercent = 0f;

        if (data.InduseEffectPrefab == null)
            return finalPercent;
        else
        {
           DurationEffect effect = data.InduseEffectPrefab.GetComponent<DurationEffect>();

            if (effect.Type == EffectType.CURSE)
            {
                // 저주 효과는 추가 계산
                finalPercent = CalculateCurseEffectPercent(unit.Mental, target.Mental);
            }

            finalPercent += data.InduseEffectSuccessRate;

            Debug.Log($"유닛 멘탈 : {unit.Mental},  타겟 멘탈 : {target.Mental}");
            Debug.Log($"최종 확률 : {Mathf.Clamp01(finalPercent)}%");

            return Mathf.Clamp01(finalPercent);
        }
    }


    // 소환(설치) 형 스킬 -> 생성 위치, 생성 할 객체
    public void GenerateSkill(Vector3 pos, GameObject obj)
    {
        GameObject skillObj = Instantiate(obj, pos, Quaternion.identity);
    }
    public void GenerateSkill(Unit unit, Unit pivotUnit, GameObject obj)
    {
        GameObject skillObj = Instantiate(obj, pivotUnit.transform.position, Quaternion.identity);
        ThunderCloud thunderCloud = skillObj.GetComponent<ThunderCloud>();
        if(thunderCloud != null)
        {
            thunderCloud.Initialize(unit);
        }
    }
}
