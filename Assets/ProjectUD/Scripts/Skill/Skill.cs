using UnityEngine;
using UltEvents;

public abstract class SkillBase : MonoBehaviour // 모든 스킬의 부모 클래스
{
    // 스킬 대상 타입.
    public enum TargetType
    {
        ENEMY,
        ALLY
    }

    [Header("■ Events")]
    [SerializeField] private UltEvent<Unit, Unit> onActivate; // 발동했을 때 실행할 이벤트
    [SerializeField] private UltEvent<Unit, Fortress> onAttackFortress; // 발동했을 때 실행할 이벤트

    protected float coolTimeCheck;

    public abstract SkillData Data { get; }
    public bool IsCoolDown => coolTimeCheck >= Data.CoolTime;

    public TargetType GetTargetType() => Data.TargetType;

    public void Activate(Unit unit, Unit target)
    {
        coolTimeCheck -= Data.CoolTime;
        if (onActivate != null)
            onActivate.Invoke(unit, target);
    }

    public void Activate(Unit unit, Fortress fortress)
    {
        coolTimeCheck -= Data.CoolTime;
        if (onAttackFortress != null)
            onAttackFortress.Invoke(unit, fortress);
    }

    public virtual void Initialize()
    {
        coolTimeCheck = Data.CoolTime;
    }

    protected virtual void Update()
    {
        if(!IsCoolDown)
            coolTimeCheck += Time.deltaTime;
    }
}