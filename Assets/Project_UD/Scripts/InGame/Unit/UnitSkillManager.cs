using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnitDataManager;

//이 스크립트는 유닛의 스킬들을 관리하기 위한 스크립트입니다. (아군, 적군 통합.)

public enum AttackSkillType
{
    가로베기,
    검격,
    참격,
    참수,
    회전격,
    선풍격,
    휘둘러치기,
    난타,
    충격파,
    역린,
    강타,
    경천동지,
    찌르기,
    절명,
    관통일격,
    막쏘기,
    겨누어쏘기,
    발포,
    속사포,
    후려치기,
    회전난타,
    유탄투척,
    곰덫설치,
}

public class UnitSkillManager : MonoBehaviour
{
    //필요한 프리팹들
    public GameObject Bow; //활
    public GameObject Granade; //수류탄
    public GameObject Trap; //곰덫

    public GameObject AttackTrigger; //공격 트리거

    public bool attackStop = false;
    public bool SpecialSkillStop = false;
    public float weaponCooldown_Cur = 0;//현재 일반공격 쿨타임
    public float skillCooldown_Cur = 0;//현재 특수 스킬 쿨타임

    public Ingame_UnitCtrl UnitCtrl; //자기 자신 스크립트 불러올 때 사용.
    GridManager GridManager;//설치형 스킬에 필요한 위치 불러올 그리드.
    Animator unitAnimator;

    public GameObject SetObject = null; //현재 설치한 오브젝트
    public Dictionary<int, bool> TargetCellPlaceable = new Dictionary<int, bool> { };
    public List<int> TargetCellIdx = new List<int>(); //
    int TargetCellIdxFinal = 0;//최종적으로 설치될 위치의 셀.

    private void Awake()
    {
        GridManager = GridManager.inst;
    }
        

    private void Update()
    {
        unitAnimator = UnitCtrl.GetComponent<UnitAnimationParaCtrl>().animator;
    }

    public void UnitGeneralSkillCooldownInit()
    {
        weaponCooldown_Cur = 0;
    }


    //아군 병사 일반 스킬
    public void UnitGeneralSkill(UnitSkillData Skill, GameObject TargetEnemy, float weaponCooldown, bool isEnemyAttack)
    {
        Vector3 TargetPos = TargetEnemy.transform.position;
        

        //공격 정보 관리 (데미지, 치명타 확률, 공격 타입, 디버프(필요한 경우))
        int damage = Skill.damage;
        AttackType attackType = Skill.attackType;
        UnitDebuff debuff = Skill.debuff;
        bool AttackTargetPoint = Skill.AttackTargetPoint;
        float bounsCrit = Skill.bounsCrit;

        if (weaponCooldown_Cur <= 0)
        {
            UnitCtrl.VisualModel.transform.LookAt(TargetPos);
            Animator animator = UnitCtrl.GetComponent<UnitAnimationParaCtrl>().animator;
            animator.SetTrigger(CONSTANT.ANITRIGGER_ATTACK);//쿨타임이 돌면 공격 애니메이션 실행.

            //공격의 상세 스텟 설정.
            switch (Skill.skillType)
            {
                case AttackSkillType.막쏘기:
                    if (Bow != null && Bow.GetComponent<BowCtrl>() != null)
                    {
                        Bow.transform.LookAt(TargetPos);
                        Bow.GetComponent<BowCtrl>().ArrowShoot(UnitCtrl.gameObject.CompareTag(CONSTANT.TAG_ENEMY));
                    }
                    break;
                case AttackSkillType.회전격:
                    break;
            }

            if (AttackTargetPoint)
            {
                //타겟이 적인가?
                if (TargetEnemy.gameObject.CompareTag(CONSTANT.TAG_ENEMY) && TargetEnemy.activeSelf)
                {
                    Ingame_UnitCtrl EnemyCtrl = TargetEnemy.GetComponent<Ingame_UnitCtrl>();
                    int HitSoundRandomNum = Random.Range(0, UnitCtrl.unitData.attackSound.Length);
                    AudioClip SFX2Play = UnitCtrl.unitData.attackSound[HitSoundRandomNum];

                    UnitCtrl.soundManager.PlaySFX(UnitCtrl.soundManager.ATTACK_SFX, SFX2Play);
                    EnemyCtrl.ReceivePhysicalDamage(damage, UnitCtrl.unitData.critChanceRate + bounsCrit, attackType, debuff);
                }
            }
            else
            {
                if (AttackTrigger != null)
                {
                    GameObject AttackTriggerObj = Instantiate(AttackTrigger,UnitCtrl.transform);
                    AttackCtrl attackCtrl = AttackTriggerObj.GetComponent<AttackCtrl>();
                    attackCtrl.Damage = damage;
                    attackCtrl.Crit = UnitCtrl.unitData.critChanceRate + bounsCrit;
                    attackCtrl.Type = attackType;
                    attackCtrl.Debuff2Add = debuff;
                }
            }
            weaponCooldown_Cur = weaponCooldown; //쿨타임 초기화.
        }
        else 
        {
            if (!attackStop)
            {
                weaponCooldown_Cur -= Time.deltaTime;
            } 
        }
    }

    //스킬 데미지 초기화.
    int SpecialSkillDamageInit()
    {
        //스킬 데미지 데이터 가져오기.
        return UnitCtrl.unitData.specialSkill.damage;
    }

    //아군 병사 특별 스킬
    public void UnitSpecialSkill(UnitSkillData Skill, float skillCooldown)
    {
        if (Skill == null)
        { return;}

        int SkillDamage = SpecialSkillDamageInit();
        Ingame_UnitCtrl TargetEnemy = null;

        if (UnitCtrl.targetEnemy != null)
        {
            TargetEnemy = UnitCtrl.targetEnemy.GetComponent<Ingame_UnitCtrl>();
            SpecialSkillStop = false;
        }
        else
        {
            SpecialSkillStop = true;
        }

        if (skillCooldown_Cur <= 0)
        {
            skillCooldown_Cur = skillCooldown;
            switch (Skill.skillType)
            {
                //연사
                case AttackSkillType.겨누어쏘기:
                    //활을 쏘아 앞에 있는 한 명의 적에게 5 데미지의 관통 공격을 가한다. 치명타율 5% 증가. 치명타 발동 시 충격 효과
                    //활 쏘기와 같은 처리. 스탯만 다르게.
                    if (TargetEnemy != null && UnitCtrl.isEnemyInRange)
                    {
                        unitAnimator.SetTrigger(CONSTANT.ANITRIGGER_SPECIAL);
                        Bow.transform.LookAt(TargetEnemy.transform.position);
                        Bow.GetComponent<BowCtrl>().ArrowShoot(false);
                        TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + Skill.bounsCrit, AttackType.Pierce, UnitDebuff.None);
                    }
                    break;
                //case AttackSkillType.난타:

                    break;
                //수류탄 투척
                case AttackSkillType.유탄투척:
                    //수류탄을 던져 범위 내에 있는 모든 적에게 10 데미지의 분쇄 공격을 가한다. 치명타율 10% 증가. 치명타 발동 시 기절 효과
                    //덫 설치와 비슷하게 구현. 서있는 칸을 기준으로 바라보는 방향 몇칸 앞에 공격 트리거 오브젝트를 설치하는 식으로.
                    //단, 미리 배치 되어있는지는 고려하지 않음.
                    if (TargetEnemy != null && UnitCtrl.isEnemyInRange)
                    {
                        Debug.Log("수류탄 투척");
                        unitAnimator.SetTrigger(CONSTANT.ANITRIGGER_SPECIAL);
                        GameObject Granade_Obj = Instantiate(Granade);
                        Granade_Obj.transform.position = UnitCtrl.transform.position;
                        GranadeCtrl granade = Granade_Obj.GetComponent<GranadeCtrl>();
                        granade.targetPos = TargetEnemy.transform.position;
                    }
                    break;
                //곰덫 설치
                case AttackSkillType.곰덫설치:
                
                    //서있는 칸을 기준으로 바라보는 방향 2칸 앞에 꿩덫을 설치하여 지나가는 한 명의 적에게 5 데미지의 베기 공격을 가한다. 치명타율이 5% 증가. 치명타 발동 시 속박 효과.
                    //이미 설치된 꿩덫이 발동되기 전에 해당 스킬을 다시 사용한다면, 이전에 설치된 꿩덫이 부서지고 해당 스킬이 발동되어 새로 꿩덫을 설치한다.
                    //-> 덫 설치 관련해서 기획쪽이랑 이야기를 해야할 필요가 있어보임.

                    if (SetObject != null)
                    {
                        GridManager.SetTilePlaceable(SetObject.transform.position, true, true);
                        Destroy(SetObject);
                        SetObject = null;
                    }

                    float CurAngle = gameObject.transform.eulerAngles.y;
                    Debug.Log("현재 각도: " + CurAngle);
                    Vector2 CurCellPos = UnitCtrl.unitPos;
                    Vector2[] TargetCells = new Vector2[3];

                    // 각도에 따라 x, y 오프셋을 설정
                    Vector2[] directionOffsets = GetTargetCellPos(CurAngle);
                    
                    // TargetCells에 오프셋 적용하여 좌표 계산
                    for (int i = 0; i < 3; i++)
                    {
                        TargetCells[i] = CurCellPos + directionOffsets[i];
                    }

                    Vector3 TargetCellWorldPos = Vector3.zero;

                    for (int i = 0; i < 3; i++)
                    {
                        if (!TargetCellPlaceable.ContainsKey(i))
                        {
                            TargetCellPlaceable.Add(i, GridManager.GetTilePlaceable(TargetCells[i]));
                        }
                        if (!TargetCellIdx.Contains(i))
                        {
                            TargetCellIdx.Add(i);
                        }
                    }

                    if (TargetCellIdx.Count > 0)
                    {
                        TargetCellIdxFinal = Random.Range(0, TargetCellIdx.Count - 1);
                        Debug.Log("난수 : " + TargetCellIdxFinal);
                        int TargetCellFinal = TargetCellIdx[TargetCellIdxFinal];
                        Vector3 TargetCellFinalWorldPos = GridManager.mapGrid.CellToWorld(new Vector3Int((int)TargetCells[TargetCellFinal].x, (int)TargetCells[TargetCellFinal].y, 1));

                        while (TargetCellIdx.Count != 0)
                        {
                            TargetCellFinalWorldPos = GridManager.mapGrid.CellToWorld(new Vector3Int((int)TargetCells[TargetCellFinal].x, (int)TargetCells[TargetCellFinal].y, 1));

                            Debug.Log(TargetCellFinal + "번 셀에 설치를 시도합니다.");
                            if (GridManager.GetTilePlaceable(TargetCellFinalWorldPos))
                            {
                                Debug.Log("해당 셀에 설치가 가능 합니다.");
                                TargetCellWorldPos = TargetCellFinalWorldPos;
                                break;
                            }
                            else
                            {
                                Debug.Log("해당 셀에 설치가 불가능 합니다.");
                                TargetCellIdx.RemoveAt(TargetCellIdxFinal);
                                if (TargetCellIdx.Count == 0)
                                {
                                    Debug.Log("모두 설치 불가. 스킬 쿨타임을 초기화합니다.");
                                    skillCooldown_Cur = 0;
                                    return;
                                }
                                else
                                {
                                    TargetCellIdxFinal = Random.Range(0, TargetCellIdx.Count - 1);
                                    Debug.Log("새로운 난수 : " + TargetCellIdxFinal);
                                    TargetCellFinal = TargetCellIdx[TargetCellIdxFinal];
                                    continue;
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("모두 설치 불가. 스킬 쿨타임을 초기화합니다.");
                        skillCooldown_Cur = 0;
                        return;
                    }

                    if (GridManager.GetTilePlaceable(TargetCellWorldPos))
                    {
                        Vector3 CellWorldPosFinal = TargetCellWorldPos + new Vector3(GridManager.mapGrid.cellSize.x * 0.5f, 0, GridManager.mapGrid.cellSize.y * 0.5f);

                        //GridManager.mapGrid.GetCellCenterWorld(new Vector3Int((int)TargetCells[TargetCellFinal].x, 1, (int)TargetCells[TargetCellFinal].y));
                        SetObject = Instantiate(Trap);
                        AttackCtrl attackCtrl = SetObject.GetComponent<AttackCtrl>();
                        attackCtrl.Crit = UnitCtrl.unitData.critChanceRate;
                        SetObject.transform.position = new Vector3(CellWorldPosFinal.x, -1, CellWorldPosFinal.z);
                        GridManager.SetTilePlaceable(TargetCellWorldPos, true, false);
                        TargetCellPlaceable.Clear();
                        TargetCellIdx.Clear();
                    }
                    break;

                default:
                    if (TargetEnemy != null && UnitCtrl.isEnemyInRange)
                    {
                        TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + Skill.bounsCrit, Skill.attackType, Skill.debuff);
                    }
                    break;
            }
        }
        else
        {
            if (!SpecialSkillStop)
            {
                skillCooldown_Cur -= Time.deltaTime;
            }
        }
    }

    Vector2[] GetTargetCellPos(float CurAngle)
    {
        if (CurAngle <= 45 || CurAngle > 315)       // 북쪽
            return new Vector2[] { new Vector2(-1, 2), new Vector2(0, 2), new Vector2(1, 2) };
        else if (CurAngle <= 135 && CurAngle > 45)   // 동쪽
            return new Vector2[] { new Vector2(2, 1), new Vector2(2, 0), new Vector2(2, -1) };
        else if (CurAngle <= 225 && CurAngle > 135)  // 남쪽
            return new Vector2[] { new Vector2(1, -2), new Vector2(0, -2), new Vector2(-1, -2) };
        else if (CurAngle > 225 && CurAngle <= 315) // 서쪽
            return new Vector2[] { new Vector2(-2, -1), new Vector2(-2, 0), new Vector2(-2, 1) };
        else
        {
            Debug.LogError("덫 설치 관련 각도 계산 오류.");
            return new Vector2[] { new Vector2(-1, 2), new Vector2(0, 2), new Vector2(1, 2) };
        }
    }    

    //적군 일반 스킬
    public void EnemyGeneralSkill(UnitSkillData Skill, GameObject TargetEnemy, float weaponCooldown, bool isEnemyAttack)
    {
        Vector3 TargetPos = TargetEnemy.transform.position;

        //공격 정보 관리 (데미지, 치명타 확률, 공격 타입, 디버프(필요한 경우))
        int damage = Skill.damage;
        AttackType attackType = Skill.attackType;
        UnitDebuff debuff = Skill.debuff;
        bool AttackTargetPoint = Skill.AttackTargetPoint;

        if (weaponCooldown_Cur <= 0)
        {
            UnitCtrl.VisualModel.transform.LookAt(TargetPos);

            unitAnimator.SetTrigger(CONSTANT.ANITRIGGER_ATTACK);
            switch (Skill.skillType)
            {
                //활 쏘기
                case AttackSkillType.막쏘기:
                    if (Bow != null)
                    {
                        Bow.transform.LookAt(TargetPos);
                        Bow.GetComponent<BowCtrl>().ArrowShoot(true);
                    }
                    break;
            }

            if (TargetEnemy.gameObject.CompareTag(CONSTANT.TAG_UNIT))
            {
                if (AttackTargetPoint)
                {
                    Ingame_UnitCtrl EnemyCtrl = TargetEnemy.GetComponent<Ingame_UnitCtrl>();
                    int HitSoundRandomNum = Random.Range(0, 2);
                    AudioClip SFX2Play = UnitCtrl.unitData.attackSound[HitSoundRandomNum];

                    UnitCtrl.soundManager.PlaySFX(UnitCtrl.soundManager.ATTACK_SFX, SFX2Play);
                    EnemyCtrl.ReceivePhysicalDamage(damage, UnitCtrl.unitData.critChanceRate, attackType, debuff,UnitCtrl.unitData.attackVFX);
                }
                else
                {
                    if (AttackTrigger != null)
                    {
                        GameObject AttackTriggerObj = Instantiate(UnitCtrl.unitData.attackVFX, UnitCtrl.transform);
                        AttackCtrl attackCtrl = AttackTriggerObj.GetComponent<AttackCtrl>();
                        attackCtrl.Damage = damage;
                        attackCtrl.Crit = UnitCtrl.unitData.critChanceRate;
                        attackCtrl.Type = attackType;
                        attackCtrl.Debuff2Add = debuff;
                    }
                }
            }
            else
            {
                BaseStatus BaseCtrl = TargetEnemy.GetComponent<BaseStatus>();
                BaseCtrl.ReceiveDamage(UnitCtrl.unitData.level);
            }

            weaponCooldown_Cur = weaponCooldown;
        }
        else
        {
            weaponCooldown_Cur -= Time.deltaTime;
        }
    }

    public void EnemySpecialSkill(int SkillCode, float skillCooldown)
    {
        int SkillDamage = SpecialSkillDamageInit();
        Ingame_UnitCtrl TargetEnemy = null;

        if (UnitCtrl.targetEnemy != null)
        {
            TargetEnemy = UnitCtrl.targetEnemy.GetComponent<Ingame_UnitCtrl>();
            SpecialSkillStop = false;
        }
        else
        {
            SpecialSkillStop = true;
        }

        if (skillCooldown_Cur <= 0)
        {
            skillCooldown_Cur = skillCooldown;
            switch (SkillCode)
            {
                case 401:
                    //광포의 포효를 질러 5초동안 범위 내의 아군에게 속도 증가 버프
                    break;
                case 501:
                    //망자를 소환하는 주술로 자신 주변에 5명의 너덜너덜한 좀비, 부식된 백골 병사를 소환한다.
                    break;
            }
        }
        else
        {
            if (!SpecialSkillStop)
            {
                skillCooldown_Cur -= Time.deltaTime;
            }
        }
    }
}
