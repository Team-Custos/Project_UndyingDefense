using UnityEngine;
using UltEvents;

public abstract class SkillBase : MonoBehaviour // 모든 스킬의 부모 클래스
{
    public enum TargetType  // 스킬 발동 대상
    {
        ENEMY,
        ALLY,
        NONE,
        SELF
    }

    public enum TargetRule  // 타겟 선정 방식
    {
        NEAR,
        LOWHP,
        RANDOM
    }

    [Header("■ Events")]
    [SerializeField] private UltEvent<Unit, Unit> onActivate; // 발동했을 때 실행할 이벤트
    [SerializeField] private UltEvent<Unit, Fortress> onAttackFortress; // 발동했을 때 실행할 이벤트
    [SerializeField] private TargetRule targetRule; // 타겟 선정 방식

    protected float coolTimeCheck;


    public abstract SkillData Data { get; }

    //public float AnimationStateTime => animationStateTime;

    private bool isCoolTimeOn = true;
    public bool IsCoolDown => coolTimeCheck >= Data.CoolTime; // IsCoolDown이 true면 스킬이 쿨타임이 차서 사용 가능하다는 의미.

    //public bool isAnimationOK => animationStateTimeCheck >= animationStateTime; // 애니메이션이 끝났는지 체크하는 변수

    public TargetType GetTargetType() => Data.TargetType;
    public TargetRule GetTargetRule() => targetRule;

    public void ActivateCoolTime(bool OnOff)
    { 
        isCoolTimeOn = OnOff; // 쿨타임을 사용할지 말지 결정하는 변수
    }

    public void Activate(Unit unit)
    {
        coolTimeCheck -= Data.CoolTime;
        if (onActivate != null)
        {
            onActivate.Invoke(unit, null);
            PlaySkillSFX();
        }
            
    }

    public void Activate(Unit unit, Unit target)
    {
        coolTimeCheck -= Data.CoolTime;
        if (onActivate != null)
        {
            onActivate.Invoke(unit, target);
            PlaySkillSFX();
        }
            
    }

    public void Activate(Unit unit, Fortress fortress)
    {
        coolTimeCheck -= Data.CoolTime;
        if (onAttackFortress != null)
            onAttackFortress.Invoke(unit, fortress);
    }

    public void ActivateFortressSkill(Fortress fortress)
    {
        if (onAttackFortress != null)
            onAttackFortress.Invoke(null, fortress);
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

    protected void PlaySkillSFX()
    {
        if (Data == null || Data.AttackSFX == null || Data.AttackSFX.Length == 0)
        {
            return;
        }

        int random = Random.Range(0, Data.AttackSFX.Length);
        SoundManager.Instance.PlaySFX(Data.AttackSFX[random], transform.position);
    }

    // 계산된 확률로 상태 부여
    protected void ActivateEffect(Unit unit, Unit target)
    {
        float random = Random.value;

        if (random < CalculateAddEffectPercent(unit, target) * 0.01f)
            target.AddEffect(Data.EffectPrefab, target, Vector3.zero);
    }

    // 스킬 발동 시 상태 부여 확률 계산
    protected float CalculateAddEffectPercent(Unit unit, Unit target)
    {
        float percent = 0f;

        DurationEffect effect = Data.EffectPrefab.GetComponent<DurationEffect>();

        if (effect.Type == EffectType.CURSE)
            percent = CalculateAddCurseEffectPercent(unit.Mental, target.Mental);     // 저주 상태는 멘탈 차이에 따라 발동  
        else
            percent = Data.AddEffectPercent;       // 저주 상태가 아닌 경우는 스킬 자체 확률에 따라 발동

        Debug.Log($"{Data.name} 상태 부여 확률 : {percent}%");

        return Mathf.Clamp(percent, 0f, 100f);
    }

    // 저주 상태는 유닛 간의 멘탈 차이에 따른 확률로 발동
    protected float CalculateAddCurseEffectPercent(int unitMental, int targetMental)
    {
        int mentalDifference = unitMental - targetMental;

        Debug.Log($"멘탈 차이 : {mentalDifference}");

        switch (mentalDifference)
        {
            case 4: return 100f;
            case 3: return 75f;
            case 2: return 50f;
            case 1: return 25f;
            case 0: return 0f;
            default:
                if (mentalDifference > 4) return 100f;
                else return 0f;
        }
    }
}