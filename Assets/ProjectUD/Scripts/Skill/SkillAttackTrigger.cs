<<<<<<< HEAD
=======
<<<<<<< Updated upstream
>>>>>>> KimJK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AttackSkill;
using static Unit;
<<<<<<< HEAD
=======
=======
using UnityEngine;
using static Unit;
using AttackType = AttackData.AttackType;
>>>>>>> Stashed changes
>>>>>>> KimJK

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
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
>>>>>>> KimJK
        if (data.StartVFX != null)
        {
            GameObject VFXobj = Instantiate(data.StartVFX.gameObject);
            VFXobj.transform.SetParent(transform);
            VFXobj.transform.localPosition = Vector3.zero;// + Vector3.up * VFXobj.transform.localPosition.y;
            VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            Destroy(VFXobj, data.StartVFX.main.duration);
        }
<<<<<<< HEAD
=======
=======
        //if (data.StartVFX != null)
        //{
        //    GameObject VFXobj = Instantiate(data.StartVFX.gameObject);
        //    VFXobj.transform.SetParent(transform);
        //    VFXobj.transform.localPosition = Vector3.zero;// + Vector3.up * VFXobj.transform.localPosition.y;
        //    VFXobj.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        //    Destroy(VFXobj, data.StartVFX.main.duration);
        //}
>>>>>>> Stashed changes
>>>>>>> KimJK
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
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
>>>>>>> KimJK
        float calcCrit = (target.CritVulnerability + data.BonusCrit) * 0.01f;
        if (IsBlocked(target.Data.ArmorType))
        {
            float calcBlockRate = 1f - (0.3f * target.BlockRate);
            calcDamage *= calcBlockRate;
        }

        calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;
<<<<<<< HEAD
=======
=======
        float calcCrit = (target.CritVulnerability + data.BonusCritPercent) * 0.01f;
        if (IsBlocked(target.Data.ArmorType))
        {
            float calcBlockRate = 1f - (0.3f * target.BlockPercent);
            calcDamage *= calcBlockRate;
        }

        calcDamage *= target.DamageTakenMult;
>>>>>>> Stashed changes
>>>>>>> KimJK

        target.TakeDamage(calcDamage);
        if (Random.Range(0f, 1f) <= data.InduseEffectSuccessRate * 0.01f)
        {
<<<<<<< HEAD
            if (data.InduseEffect != null)
            {
                target.AddEffect(target, data.InduseEffect.GetComponent<Effect>());
=======
<<<<<<< Updated upstream
            if (data.InduseEffect != null)
            {
                target.AddEffect(target, data.InduseEffect.GetComponent<Effect>());
=======
            if (data.InduseEffectPrefab != null)
            {
                target.AddEffect(data.InduseEffectPrefab);
>>>>>>> Stashed changes
>>>>>>> KimJK
            }
        }
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
<<<<<<< HEAD
            (data.AttackType == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.AttackType == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.AttackType == AttackType.CRUSH && armorType == ArmorType.PADDED);
=======
<<<<<<< Updated upstream
            (data.AttackType == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.AttackType == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.AttackType == AttackType.CRUSH && armorType == ArmorType.PADDED);
=======
            (data.Info.Type == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.Info.Type == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.Info.Type == AttackType.CRUSH && armorType == ArmorType.PADDED);
>>>>>>> Stashed changes
>>>>>>> KimJK
    }
}
