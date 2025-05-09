using UltEvents;
using UnityEngine;
using static AttackSkill;
using static Unit;

public abstract class CommandSkill : MonoBehaviour
{
    public enum TargetType
    {
        NONE,
        UNIT,
        MOUSEPOSAREA,
        AREA
    }

    [Header("■ Events")]
    [SerializeField] private UltEvent onActivate; // 발동했을 때 실행할 이벤트
    [SerializeField] private UltEvent<Transform> onActivateAtPos; // 발동했을 때 실행할 이벤트
    [SerializeField] private UltEvent<Unit> onActivateAtUnit; // 유닛에게 발동했을 때 실행할 이벤트

    public abstract CommandSkillData Data { get; }

    protected float coolTimeCheck;
    public bool IsCoolDown => coolTimeCheck >= Data.CoolTime; // IsCoolDown이 true면 스킬이 쿨타임이 차서 사용 가능하다는 의미.

    private void Start()
    {
        coolTimeCheck = Data.CoolTime;
    }

    private void Update()
    {
        if (coolTimeCheck < Data.CoolTime)
        {
            coolTimeCheck += Time.deltaTime;
        }
    }

    public void ApplyPassive()
    {
        if(onActivate != null)
            onActivate.Invoke();
    }

    public void Activate()
    {
        coolTimeCheck -= Data.CoolTime;
        if (onActivate != null)
            onActivate.Invoke();
    }

    public void Activate(Transform position)
    {
        coolTimeCheck -= Data.CoolTime;
        if (onActivate != null)
            onActivateAtPos.Invoke(position);
    }

    public void Activate(Unit target)
    {
        coolTimeCheck -= Data.CoolTime;
        if (onActivateAtUnit != null)
            onActivateAtUnit.Invoke(target);
    }
}
