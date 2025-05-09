using UnityEngine;
using static AttackSkill;
using static Unit;

public class CommandSkillAttackTrigger : MonoBehaviour
{
    public enum AttackTriggerType
    {
        Shpere,
        Box
    }

    private ActiveCommandSkillData data;

    private LayerMask attackTargetLayer;

    private float AreaX, AreaY, AreaZ;

    [SerializeField] private float tickTime = 0.1f;
    private float tickTimeCheck = 0f;

    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 5;

    private AttackTriggerType triggerType;

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

    public void SetTargetLayer(LayerMask targetLayer)
    {
        attackTargetLayer = targetLayer;
    }

    private void PlayVFX()
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
            }
        }
    }

    public void Attack(Unit target)
    {
        float calcDamage = data.Damage;
        float calcCrit = (target.CritVulnerability + data.BonusCrit) * 0.01f;
        if (IsBlocked(target.Data.ArmorType))
        {
            float calcBlockRate = 1f - (0.3f * target.BlockRate);
            calcDamage *= calcBlockRate;
        }

        calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;

        target.TakeDamage(calcDamage);
        if (Random.Range(0f, 1f) <= data.InduseEffectSuccessRate * 0.01f)
        {
            if (data.InduseEffct != null)
            {
                target.AddEffect(target, data.InduseEffct.GetComponent<Effect>());
            }
        }
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
            (data.AttackType == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.AttackType == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.AttackType == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }


}
