using UnityEngine;
using static Unit;
using AttackType = AttackData.AttackType;

public class CommandSkillAttackTrigger : MonoBehaviour
{
    public enum AttackTriggerType
    {
        Shpere,
        Box
    }

    private ActiveCommandSkillData data;

    protected static ParticleSystem slashHitVFX;
    protected static ParticleSystem pierceHitVFX;
    protected static ParticleSystem crushHitVFX;

    private LayerMask attackTargetLayer;

    private float AreaX, AreaY, AreaZ;
    private Vector3 incomingDirection = Vector3.zero; // 공격이 들어오는 방향

    [SerializeField] private float tickTime = 0.1f;
    private float tickTimeCheck = 0f;

    [SerializeField] private AudioClip loopSFX;

    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 100;

    private AttackTriggerType triggerType;

    [SerializeField] private IgniteEffect IgniteEffect;


    public void SetTriggerType(AttackTriggerType Type)
    {
        triggerType = Type;
    }

    public void SetArea(float X = 1f, float Y = 1f, float Z = 1f)
    {
        AreaX = X;
        AreaY = Y;
        AreaZ = Z;
    }

    public void SetData(ActiveCommandSkillData data)
    {
        this.data = data;
    }

    public void SetTickTime(float tickTime)
    {
        this.tickTime = tickTime;
    }

    public void SetTargetLayer(LayerMask targetLayer)
    {
        attackTargetLayer = targetLayer;
    }

    public void SetIncomingDirection(Vector3 direction)
    {
        incomingDirection = direction;
    }

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



    private void PlayVFX()  // 영역 연출
    {
        if (data.StartVFX != null)
        {
            GameObject VFXobj = Instantiate(data.StartVFX.gameObject);
            VFXobj.transform.SetParent(transform);
            VFXobj.transform.localPosition = Vector3.zero;// + Vector3.up * VFXobj.transform.localPosition.y;
            VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            Destroy(VFXobj, data.StartVFX.main.duration);
        }
        if (data.LoopVFX != null)
        {
            GameObject VFXobj = Instantiate(data.LoopVFX.gameObject);
            VFXobj.transform.SetParent(transform);
            VFXobj.transform.localPosition = Vector3.zero;// + Vector3.up * VFXobj.transform.localPosition.y;
            VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            SoundManager.Instance.PlaySFX(data.LoopSFX, VFXobj.transform.position);
        }

    }


    private void Start()
    {
        PlayVFX();
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position + Vector3.up * AreaY * 0.5f, new Vector3(AreaX, AreaY, AreaZ));
    }

    private void Update()
    {
        if (tickTimeCheck >= tickTime)
        {
            tickTimeCheck = 0f;
            switch (triggerType)
            {
                case AttackTriggerType.Shpere:
                    AreaAttack(transform, AreaX);
                    break;
                case AttackTriggerType.Box:
                    AreaAttack(transform, AreaX, AreaY, AreaZ);
                    break;
            }
        }
        else
        {
            tickTimeCheck += Time.deltaTime;
        }
    }

    public void AreaAttack(Transform pivotTarget, float radius) //원형 공격
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];
        int targetCount = Physics.OverlapSphereNonAlloc
            (pivotTarget.transform.position, radius, targets, attackTargetLayer);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                Attack(target);
            }
        }
    }


    public void AreaAttack(Transform pivotTarget, float AreaX, float AreaY, float AreaZ)//사각형 공격
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];

        int targetCount = Physics.OverlapBoxNonAlloc
            (pivotTarget.transform.position + Vector3.up * AreaY * 0.5f
            , new Vector3(AreaX, AreaY, AreaZ), targets, Quaternion.identity, attackTargetLayer);
        for (int i = 0; i < targetCount; i++)
        {
            if (targets[i].TryGetComponent(out Unit target))
            {
                Attack(target);

                if (data.CritEffectPrefab != null)
                {
                    target.AddEffect(data.CritEffectPrefab, target, Vector3.zero);
                }
            }
        }
    }

    public void Attack(Unit target)
    {
        float calcDamage = data.Damage;
        float calcCrit = (target.CritVulnerability + data.BonusCrit) * 0.01f;
        if (data.AttackData != null && IsBlocked(target.Data.ArmorType))
        {
            float calcBlockRate = 1f - (0.3f * target.BlockPercent * 0.01f);
            calcDamage *= calcBlockRate;
        }

        calcDamage *= target.DamageTakenMult;

        target.TakeDamage(calcDamage);
        //target.PlayHitSFX(data.AttackType);       // Unit 에서 주석처리한 메서드
        if (data.AttackData != null)
        {
            AddHitSFX(target.transform);
            AddHitVFX(target);
        }
        if (Random.Range(0f, 1f) <= data.InduseEffectSuccessRate * 0.01f)
        {
            //if (data.InduseEffectPrefab != null)
            //{
            //    target.AddEffect(data.InduseEffectPrefab);
            //}
        }
    }

    public void AddHitSFX(Transform transform)
    {
        AudioClip[] audios = data.AttackData.HitSFXClip;
        AudioClip audio = audios[Random.Range(0, audios.Length)];
        SoundManager.Instance.PlaySFX(audio, transform.position);
    }

    private void AddHitVFX(Unit target)     // 피격 연출
    {
        GameObject hitVFX = data.AttackData.HitVFX;
        if (hitVFX != null)
        {
            target.AddVFX(hitVFX, target.transform);
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

        //if (hitVFX != null)
        //{
        //    target.AddVFX(hitVFX, target.transform.position + incomingDirection);
        //}
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
            (data.AttackData.Type == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.AttackData.Type == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.AttackData.Type == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }

    


}
