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

    [Header("■ SoundManager")]
    [SerializeField] private SoundManager soundManager;

    public abstract CommandSkillData Data { get; }

    public SoundManager SoundManager => soundManager;

    protected float coolTimeCheck;
    public bool IsCoolDown => coolTimeCheck >= Data.CoolTime; // IsCoolDown이 true면 스킬이 쿨타임이 차서 사용 가능하다는 의미.

    //ayo_0117
    protected bool isSkillActivated = false;

    private void Start()
    {
        coolTimeCheck = Data.CoolTime;
    }

    public void SetSkillState(bool state)
    {
        isSkillActivated = state;
    }

    protected void UpdateCoolDown()
    {
        if (coolTimeCheck < Data.CoolTime)
        {
            coolTimeCheck += Time.deltaTime;
            
            if (coolTimeCheck >= Data.CoolTime)
                Debug.Log($"스킬 사용 가능? {IsCoolDown}");
        }
    }

    public void ApplyPassive()
    {
        if(onActivate != null)
            onActivate.Invoke();
    }

    public void Activate()
    {
        //coolTimeCheck -= Data.CoolTime;
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
        if (target != null && onActivateAtUnit != null)
            onActivateAtUnit.Invoke(target);
    }
}
