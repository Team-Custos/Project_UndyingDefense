using UnityEngine;
using UltEvents;

public abstract class SkillBase : MonoBehaviour // 모든 스킬의 부모 클래스
{
    // 스킬 대상 타입.
    public enum TargetType
    {
        ENEMY,
        ALLY,
        SELF
    }

<<<<<<< HEAD


=======
<<<<<<< Updated upstream


=======
>>>>>>> Stashed changes
>>>>>>> KimJK
    [Header("■ Events")]
    [SerializeField] private UltEvent<Unit, Unit> onActivate; // 발동했을 때 실행할 이벤트
    [SerializeField] private UltEvent<Unit, Fortress> onAttackFortress; // 발동했을 때 실행할 이벤트

    protected float coolTimeCheck;


    [Header("■ AnimationStateTime")]
    [SerializeField] private float animationStateTime; // 애니메이션의 상태를 체크하는 시간

    public abstract SkillData Data { get; }

    public float AnimationStateTime => animationStateTime;

    private bool isCoolTimeOn = true;
    public bool IsCoolDown => coolTimeCheck >= Data.CoolTime; // IsCoolDown이 true면 스킬이 쿨타임이 차서 사용 가능하다는 의미.

    //public bool isAnimationOK => animationStateTimeCheck >= animationStateTime; // 애니메이션이 끝났는지 체크하는 변수

    public TargetType GetTargetType() => Data.TargetType;

    public void ActivateCoolTime(bool OnOff)
    { 
        isCoolTimeOn = OnOff; // 쿨타임을 사용할지 말지 결정하는 변수
    }

    public void Activate(Unit unit)
    {
        coolTimeCheck -= Data.CoolTime;
        if (onActivate != null)
            onActivate.Invoke(unit, null);
    }

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
        if (!IsCoolDown && isCoolTimeOn)
            coolTimeCheck += Time.deltaTime;
    }
}