using UnityEngine;
using static Unit;
using AttackType = AttackData.AttackType;

public class SkillAttackTrigger : MonoBehaviour
{
    private AttackSkillData data;

    private LayerMask attackTargetLayer;

    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 5;

    public void SetData(AttackSkillData data)
    {
        this.data = data;
    }

    public void SetTargetLayer(LayerMask targetLayer)
    {
        attackTargetLayer = targetLayer;
    }

    private void PlayVFX()
    {
        //if (data.StartVFX != null)
        //{
        //    GameObject VFXobj = Instantiate(data.StartVFX.gameObject);
        //    VFXobj.transform.SetParent(transform);
        //    VFXobj.transform.localPosition = Vector3.zero;// + Vector3.up * VFXobj.transform.localPosition.y;
        //    VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        //    Destroy(VFXobj, data.StartVFX.main.duration);
        //}
    }

    private void Start()
    {
        PlayVFX();
        Destroy(gameObject, 0.5f);
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
                float distance = Vector3.Distance(pivotTarget.transform.position, targets[i].transform.position);
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
        float calcCrit = (target.CritVulnerability + data.BonusCritPercent) * 0.01f;
        if (IsBlocked(target.Armortype))
        {
            float calcBlockRate = 1f - (0.3f * target.BlockPercent);
            calcDamage *= calcBlockRate;
        }

        calcDamage *= target.DamageTakenMult;


        target.TakeDamage(calcDamage);
        if (Random.Range(0f, 1f) <= data.InduseEffectSuccessRate * 0.01f)
        {
            if (data.InduseEffectPrefab != null)
            {
                target.AddEffect(data.InduseEffectPrefab, target, Vector3.zero);
            }
        }
        
        
        
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
            (data.Info.Type == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.Info.Type == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.Info.Type == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }
}
