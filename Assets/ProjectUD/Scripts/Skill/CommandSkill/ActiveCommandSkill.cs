using InputEventInterface;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unit;
using AttackTriggerType = CommandSkillAttackTrigger.AttackTriggerType;
using AttackType = AttackData.AttackType;

public class ActiveCommandSkill : CommandSkill, IInputClick//ayo_0117
{
    [Header("■ Data")]
    [SerializeField] private ActiveCommandSkillData data;

    [Header("■ Target")]
    [SerializeField] private LayerMask attackTargetLayer;
    //ayo_0117
    [SerializeField] private LayerMask selectTargetLayer;
    private EnemyUnit prevMarkedTargetUnit;

    [Header("■ AreaTriggerObject")]
    [SerializeField] protected GameObject areaTriggerObject;
    [SerializeField] protected Vector3 incomingDirection = Vector3.zero;

    public override CommandSkillData Data => data;
    private GameObject executeEffect;

    public LayerMask AttackTargetLayer => attackTargetLayer;

    //ayo_0117
    [Header("■ 리팩토링중")]
    [SerializeField] private CommandSkillTargetingController targetingController;
    [SerializeField] private CommandSkill_FireOilCtrl BurningOilCtrl;
    [SerializeField] private CommandSkill_PoisonArea poisonArea;
    //[SerializeField] private Camera mainCamera;
    //[SerializeField] private PlayerInputEventManager inputEventManager; 
    //[SerializeField] private SelectedUnitManager SelectedUnitManager;   // 유닛 선택 스킬_집중포화스킬
    //[SerializeField] private InGameManager ingameManager;
    //[SerializeField] private GameObject circle;
    private Transform pivotTarget;
    
    private float AreaX, AreaY, AreaZ;
    private float tickTime = 0.1f;
    private float lifeTime = 0f;

    private Ray ray;
    private RaycastHit hit;

    void Update()
    {
        //ayo_0117
        UpdateCoolDown();
        //if (isSkillActivated)
        //{
        //    UpdateTargeting();
        //}
        //circle.SetActive(isSkillActivated);
    }

    private void UpdateTargeting()
    {
        /*ayo_0117
        if (inputEventManager.IsPointerOnUIElements())
            return;

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f, selectTargetLayer))
        {
            circle.transform.position = hit.point;
        }*/
    }

    public void ReadyToSelectUnit() //ayo_0117
    {
        //inputEventManager.OnClickTarget = this;
        isSkillActivated = true;
        targetingController.BeginTargeting(this);

    }
    public void ReadyToSelectLocation(float radius, float tickTime = 0.1f, float lifeTime = 0f) //ayo_0117
    {
        //inputEventManager.OnClickTarget = this;
        isSkillActivated = true;
        targetingController.BeginTargeting(this);

        AreaX = radius;
        this.tickTime = tickTime;
        this.lifeTime = lifeTime;
        //circle.SetActive(true);    // 집중 포화 원 표시
    }

    public void ReadyToSelectLocation(float radius, float lifeTime) // lopo_0122
    {
        isSkillActivated = true;
        targetingController.BeginTargeting(this);

        AreaX = radius;
        this.lifeTime = lifeTime;
    }

    //public void AreaAttack(Transform pivotTarget, float radius, float tickTime = 0.1f, float lifeTime = 0f) //원형 공격
    public void AreaAttack(Transform pivotTarget, float radius, float tickTime = 0.1f, float lifeTime = 0f) //원형 공격
    {
        CommandSkillAttackTrigger trigger = 
            Instantiate(areaTriggerObject).GetComponent<CommandSkillAttackTrigger>();
        trigger.transform.position = pivotTarget.position;
        trigger.transform.rotation = pivotTarget.rotation;
        if (lifeTime > 0)
        {
            Destroy(trigger.gameObject, lifeTime);
        }
        if (tickTime > 0)
        {
            trigger.SetTickTime(tickTime);
        }

        trigger.SetData(data);
        trigger.SetTargetLayer(attackTargetLayer);
        trigger.SetTriggerType(AttackTriggerType.Shpere); //원형 공격
        trigger.SetArea(AreaX);
        trigger.SetIncomingDirection(incomingDirection);

        //ayo_0117
        SoundManager.Instance.PlaySFX(Data.StartSFX, pivotTarget.position);
        coolTimeCheck -= Data.CoolTime;
    }
    public void AreaAttack(Transform pivotTarget, float tickTime) //원형 공격
    {
        CommandSkillAttackTrigger trigger =
            Instantiate(areaTriggerObject).GetComponent<CommandSkillAttackTrigger>();
        trigger.transform.position = pivotTarget.position;
        trigger.transform.rotation = pivotTarget.rotation;
        if (lifeTime > 0)
        {
            Destroy(trigger.gameObject, lifeTime);

        }
        if (tickTime > 0)
        {
            trigger.SetTickTime(tickTime);
        }

        trigger.SetData(data);
        trigger.SetTargetLayer(attackTargetLayer);
        trigger.SetTriggerType(AttackTriggerType.Shpere); //원형 공격
        trigger.SetArea(AreaX);
        trigger.SetIncomingDirection(incomingDirection);

        //ayo_0117
        SoundManager.Instance.PlaySFX(Data.StartSFX, pivotTarget.position);
        coolTimeCheck -= Data.CoolTime;
    }

    public void AreaAttack(Transform pivotTarget) // lopo_0122 
    {
        GameObject PosionArea = Instantiate(poisonArea.gameObject, pivotTarget.position, Quaternion.identity);
        //poisonArea.Activate();
        Destroy(PosionArea, lifeTime);

        coolTimeCheck -= Data.CoolTime;

        CommandSkillAttackTrigger trigger =
            Instantiate(areaTriggerObject).GetComponent<CommandSkillAttackTrigger>();

        //trigger.transform.position = pivotTarget.position;
        //trigger.transform.rotation = pivotTarget.rotation;
        if (lifeTime > 0)
        {
            Destroy(trigger.gameObject, lifeTime);
        }

        trigger.SetData(data);
        //trigger.SetTargetLayer(attackTargetLayer);
        //trigger.SetTriggerType(AttackTriggerType.Shpere); //원형 공격
        //trigger.SetArea(AreaX);
        //trigger.SetIncomingDirection(incomingDirection);


    }






    public void AreaAttack(Transform pivotTarget, float AreaX, float AreaY, float AreaZ, float tickTime = 0.1f, float lifeTime = 0f)//사각형 공격
    {
        //ayo_0117
        BurningOilCtrl.SpawnStart();
        CommandSkillAttackTrigger trigger =
            Instantiate(areaTriggerObject).GetComponent<CommandSkillAttackTrigger>();
        trigger.transform.position = pivotTarget.position;
        trigger.transform.rotation = pivotTarget.rotation;
        if (lifeTime > 0)
        {
            Destroy(trigger.gameObject, lifeTime);
        }
        if (tickTime > 0)
        {
            trigger.SetTickTime(tickTime);
        }

        trigger.SetData(data);
        trigger.SetTargetLayer(attackTargetLayer);
        trigger.SetTriggerType(AttackTriggerType.Box); //사각형 공격
        trigger.SetArea(AreaX, AreaY, AreaZ);

        //ayo_0117
        SoundManager.Instance.PlaySFX(Data.StartSFX, pivotTarget.position);
        coolTimeCheck -= Data.CoolTime;

    }



    public void Attack(Unit target)
    {
        float calcDamage = data.Damage;
        float calcCrit = (target.CritVulnerability + data.BonusCrit) * 0.01f;
        if (IsBlocked(target.Data.ArmorType))
        {
            float calcBlockRate = 1f - (0.3f * target.BlockPercent * 0.01f);
            calcDamage *= calcBlockRate;
        }

        calcDamage *= target.DamageTakenMult;

        target.TakeDamage(calcDamage);
        if (Random.Range(0f, 1f) <= data.InduseEffectSuccessRate * 0.01f)
        {
            if (data.InduseEffectPrefab != null)
            {
                ApplyEffect(target, data.InduseEffectPrefab);
            }
        }
    }

    public void ApplyEffect(Unit target, GameObject effectPrefab)
    {
        target.AddEffect(effectPrefab, target, Vector3.zero);
    }

    public void GetExecutionMark(Unit target)   // 척살명령 지정
    {
        if(executeEffect == null)
        {
            executeEffect = Instantiate(data.CritEffectPrefab);
            executeEffect.SetActive(false);
        }
            

        ExecutionEffect executionEffect = executeEffect.GetComponent<ExecutionEffect>();

        if (prevMarkedTargetUnit != null)       // 척살 삭제
        {
            prevMarkedTargetUnit.SetExecution(executionEffect, false, executeEffect);
        }

        if (target.GetComponent<EnemyUnit>() != null)       // 척살 적용
        {
            EnemyUnit LastMarkEnemy = target.GetComponent<EnemyUnit>();
            LastMarkEnemy.SetExecution(executionEffect, true, executeEffect);
            prevMarkedTargetUnit = LastMarkEnemy;
        }

        //ayo_0117
        SoundManager.Instance.PlaySFX(Data.StartSFX, target.transform.position);
        coolTimeCheck -= Data.CoolTime;
    }

    private bool IsBlocked(ArmorType armorType)
    {
        return
            (data.AttackData.Type == AttackType.SLASH && armorType == ArmorType.STEELPLATED) ||
            (data.AttackData.Type == AttackType.PIERCE && armorType == ArmorType.ANTIPIERCING) ||
            (data.AttackData.Type == AttackType.CRUSH && armorType == ArmorType.PADDED);
    }

    //----------------------------------------------------- ayo_0117
    public LayerMask GetSelectTargetLayer()
    {
        return selectTargetLayer;
    }

    public void OnTargetSelected(RaycastHit hit)    // lopol 0122 수정
    {
        switch (Data.TargetType)
        {
            case TargetType.UNIT:
                if (hit.collider.TryGetComponent<Unit>(out var unit))
                    GetExecutionMark(unit);
                break;

            case TargetType.MOUSEPOSAREA:
                if (hit.collider.CompareTag(CONSTANT.TAG_TILE))
                {
                    switch(data.Id)
                    {
                        case "CSkillA001":  // 집중 포화 명령
                            {
                                AreaAttack(hit.transform, tickTime);
                                break;
                            }
                        case "CSkillA006":  // 맹독 살포
                            {
                                AreaAttack(hit.transform);
                                break;
                            }
                    }

                }
                    
                break;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        /*ayo_0117
        if (context.performed)
        {
            ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (inputEventManager.IsPointerOnUIElements())
                return;
            if (!isSkillActivated)
                return;

            if (Physics.Raycast(ray, out hit, float.MaxValue, selectTargetLayer))
            {
                if (hit.collider.GetComponent<Unit>() != null)
                {
                    Unit selectedTargetUnit = hit.collider.GetComponent<Unit>();
                    GetMark(selectedTargetUnit);          //skill[activatedSkillButtonIdx], hit.transform);
                    inputEventManager.OnClickTarget = SelectedUnitManager;
                    inputEventManager.OnESCTarget = ingameManager;
                    isSkillActivated = false;
                }

                else if (hit.collider.CompareTag(CONSTANT.TAG_TILE))
                {
                    pivotTarget = hit.collider.transform;
                    //AreaAttack();
                    inputEventManager.OnClickTarget = SelectedUnitManager;
                    inputEventManager.OnESCTarget = ingameManager;
                    circle.SetActive(false);    // 집중 포화 원 표시 해제
                    isSkillActivated = false;
                }
            }
        }*/

    }

}
