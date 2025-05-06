using UltEvents;
using UnityEngine;
using UnityEngine.UIElements;
using static AttackSkill;
using static Unit;

public class CommandSkill : MonoBehaviour
{
    public enum TargetType
    {
        NONE,
        UNIT,
        AREA
    }

    public enum CommandSkillType
    {
        ACTIVE,
        PASSIVE
    }

    [Header("■ Events")]
    [SerializeField] private UltEvent onActivate; // 발동했을 때 실행할 이벤트
    [SerializeField] private UltEvent<Transform> onActivateAtPos; // 발동했을 때 실행할 이벤트
    [SerializeField] private UltEvent<Unit> onActivateAtUnit; // 유닛에게 발동했을 때 실행할 이벤트

    [Header("■ Target")]
    [SerializeField] private LayerMask TargetLayer;

    [Header("■ Data")]
    [SerializeField] private CommandSkillData data;
    

    public CommandSkillData Data => data;

    protected float coolTimeCheck;
    public bool IsCoolDown => coolTimeCheck >= data.CoolTime; // IsCoolDown이 true면 스킬이 쿨타임이 차서 사용 가능하다는 의미.

    public void Activate()
    {
        coolTimeCheck -= data.CoolTime;
        if (onActivate != null)
            onActivate.Invoke();
    }

    public void Activate(Transform position)
    {
        coolTimeCheck -= data.CoolTime;
        if (onActivate != null)
            onActivateAtPos.Invoke(position);
    }

    public void Activate(Unit target)
    {
        coolTimeCheck -= data.CoolTime;
        if (onActivateAtUnit != null)
            onActivateAtUnit.Invoke(target);
    }


    // 범위를 가진 스킬
    protected Collider[] targets;
    protected const int maxTargetCount = 5;

    public void AreaAttack(Transform pivotTarget, float radius) //원형 공격
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];
        int targetCount = Physics.OverlapSphereNonAlloc
            (pivotTarget.transform.position, radius, targets, TargetLayer);
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
    }

    public void ApplyEffect(Unit target, Effect effect)
    {
        target.AddEffect(target, effect);
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
            (data.AttackType == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.AttackType == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.AttackType == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }
}
