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
            //VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            Destroy(VFXobj, data.StartVFX.main.duration);
        }
        if (data.LoopVFX != null)
        {
            GameObject VFXobj = Instantiate(data.LoopVFX.gameObject);
            VFXobj.transform.SetParent(transform);
            VFXobj.transform.localPosition = Vector3.zero;// + Vector3.up * VFXobj.transform.localPosition.y;
            //VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

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
                float distance = Vector3.Distance(target.transform.position, pivotTarget.position);
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
        float calcCrit = (data.BonusCrit + target.CritVulnerability) * 0.01f;


        calcDamage *= Mathf.Max(0f, target.DamageTakenMult);    // 피해량 계산

        if (IsBlocked(target.Armortype))
        {
            float calcBlockRate = 1f - (0.5f * target.BlockPercent);    // 단위수정_AYO
            calcDamage *= calcBlockRate;

            calcCrit -= 0.5f; // 치명타율 감소
            if (calcCrit < 0f)
                calcCrit = 0f;

            //Debug.Log($"치명타 율 : {calcCrit}");
        }

        if (Random.Range(0f, 1f) <= calcCrit)
        {
            AddCritVFX(target);
            AddCritSFX(target.transform.position);
            ActivateCriticalEffect(target);
            Debug.Log("적용");
        }
        else
        {
            AddHitVFX(target);
            AddHitSFX(target.transform.position);
            Debug.Log("미적용");
        }

        target.TakeDamage(calcDamage, null);

        Debug.Log(calcDamage);
    }

    public void AddHitSFX(Transform transform)
    {
        AudioClip[] audios = data.AttackData.HitSFXClip;
        AudioClip audio = audios[Random.Range(0, audios.Length)];
        SoundManager.Instance.PlaySFX(audio, transform.position);
    }

    private void AddHitVFX(Unit target)     // 피격 연출
    {
        if (data == null || data.AttackData == null)
            return;

        GameObject hitVFX = data.AttackData.HitVFX;
        if (hitVFX != null)
        {
            target.AddVFX(hitVFX, target.transform);
        }
    }

    private bool IsBlocked(ArmorType armorType)
    {
        if (data == null || data.AttackData == null)
            return false;

        return
            (data.AttackData.Type == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.AttackData.Type == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.AttackData.Type == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }



    public void AddCritSFX(Vector3 pos)
    {
        if (data == null || data.AttackData == null)
            return;

        SoundManager.Instance.PlaySFX(data.AttackData.CritSFXClip, pos);
    }


    private void ActivateCriticalEffect(Unit target)
    {
        if (data == null ||data.AttackData == null)
            return;

        target.AddEffect(data.AttackData.CritEffectPrefab, target, Vector3.zero);

    }


    public void AddHitSFX(Vector3 pos)
    {
        if (data == null || data.AttackData == null)
            return;

        AudioClip[] audios = data.AttackData.HitSFXClip;

        if (audios.Length > 0)
        {
            AudioClip audio = audios[Random.Range(0, audios.Length)];
            SoundManager.Instance.PlaySFX(audio, pos);
        }

    }

    private void AddCritVFX(Unit target)
    {
        if (data == null || data.AttackData == null)
            return;

        GameObject critVFX = data.AttackData.CritVFX;
        if (critVFX != null)
        {
            //target.AddVFX(critVFX, unit.transform);
            target.AddVFX(critVFX, target);
        }

    }
}
