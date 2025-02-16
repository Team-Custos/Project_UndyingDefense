using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnitDataManager;

//이 스크립트는 유닛의 스킬들을 관리하기 위한 스크립트입니다. (아군, 적군 통합.)

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
    public void UnitGeneralSkill(int SkillCode, GameObject TargetEnemy, float weaponCooldown, bool isEnemyAttack)
    {
        Vector3 TargetPos = TargetEnemy.transform.position;

        //공격 정보 관리 (데미지, 치명타 확률, 공격 타입, 디버프(필요한 경우))
        int damage = UnitCtrl.unitData.generalSkillDamage;
        AttackType attackType = UnitCtrl.unitData.generalAttackType;
        int CritChanceRate = UnitCtrl.unitData.baseCritChanceRate;
        UnitDebuff debuff = UnitCtrl.unitData.generalSkillDebuff;
        bool AttackTargetPoint = true;

        if (weaponCooldown_Cur <= 0)
        {
            Animator animator = UnitCtrl.GetComponent<UnitAnimationParaCtrl>().animator;
            animator.SetTrigger(CONSTANT.ANITRIGGER_ATTACK);//쿨타임이 돌면 공격 애니메이션 실행.

            if (SkillCode == 102)
            {
                if (Bow != null)
                {
                    Bow.transform.LookAt(TargetPos);
                    Bow.GetComponent<BowCtrl>().ArrowShoot(false);
                }
            }

            /*
            //공격의 상세 스텟 설정.
            switch (SkillCode)
            {
                //낫 베기
                case 101:
                    //낫으로 앞에 있는 한 명의 적을 베어 5 데미지의 베기 공격을 가한다. 치명타 발동 시 출혈 효과
                    damage = 5;
                    attackType = AttackType.Slash;
                    debuff = UnitDebuff.Bleed;
                    break;
                //활 쏘기
                case 102:
                    //화살을 쏘아 한 명의 적에게 5 데미지의 관통 공격을 가한다. 치명타 발동 시 출혈 효과
                    if (Bow != null && Bow.GetComponent<BowCtrl>() != null)
                    {
                        Bow.transform.LookAt(TargetPos);
                        Bow.GetComponent<BowCtrl>().ArrowShoot(UnitCtrl.gameObject.CompareTag(CONSTANT.TAG_ENEMY));
                    }
                    attackType = AttackType.Pierce;
                    damage = 5;
                    debuff = UnitDebuff.Dizzy;
                    break;
                //창 찌르기
                case 201:
                    damage = 7;
                    attackType = AttackType.Pierce;
                    debuff = UnitDebuff.Bleed;
                    break;
                //망치 내려치기
                case 202:
                    damage = 12;
                    attackType = AttackType.Crush;
                    debuff = UnitDebuff.Dizzy;
                    AttackTargetPoint = false;
                    break;
                //엽총 쏘기
                case 203:
                    damage = 7;
                    attackType = AttackType.Pierce;
                    debuff = UnitDebuff.Bleed;
                    break;
            }
            */

            if (AttackTargetPoint)
            {
                //타겟이 적인가?
                if (TargetEnemy.gameObject.CompareTag(CONSTANT.TAG_ENEMY) && TargetEnemy.activeSelf)
                {
                    Ingame_UnitCtrl EnemyCtrl = TargetEnemy.GetComponent<Ingame_UnitCtrl>();
                    int HitSoundRandomNum = Random.Range(0, 2);
                    AudioClip SFX2Play = UnitCtrl.unitData.attackSound[HitSoundRandomNum];

                    UnitCtrl.soundManager.PlaySFX(UnitCtrl.soundManager.ATTACK_SFX, SFX2Play);
                    EnemyCtrl.ReceivePhysicalDamage(damage, CritChanceRate, attackType, debuff);
                }
            }
            else
            {
                if (AttackTrigger != null)
                {
                    GameObject AttackTriggerObj = Instantiate(AttackTrigger,UnitCtrl.transform);
                    AttackCtrl attackCtrl = AttackTriggerObj.GetComponent<AttackCtrl>();
                    attackCtrl.Damage = damage;
                    attackCtrl.Crit = UnitCtrl.unitData.baseCritChanceRate;
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
        return UnitCtrl.unitData.specialSkillDamage;
    }

    //아군 병사 특별 스킬
    public void UnitSpecialSkill(int SkillCode, float skillCooldown)
    {
        //공격 정보 관리 (데미지, 치명타 확률, 공격 타입, 디버프(필요한 경우))
        int damage = UnitCtrl.unitData.generalSkillDamage;
        AttackType attackType = UnitCtrl.unitData.generalAttackType;
        int CritChanceRate = UnitCtrl.unitData.baseCritChanceRate + UnitCtrl.unitData.bonusCritChanceRate;
        UnitDebuff debuff = UnitCtrl.unitData.generalSkillDebuff;
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
            if (TargetEnemy != null && UnitCtrl.isEnemyInRange)
            {

                if (SkillCode == 102)
                {
                    if (TargetEnemy == null && !UnitCtrl.isEnemyInRange)
                    {
                        return;
                    }
                    else
                    {
                        Bow.transform.LookAt(UnitCtrl.targetEnemy.transform.position);
                        Bow.GetComponent<BowCtrl>().DoubleShot();
                        TargetEnemy.ReceivePhysicalDamage(damage, CritChanceRate, attackType, debuff);
                    }
                }
                else if (SkillCode == 203)
                {
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
                }
                else if (SkillCode == 204)
                {
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
                        attackCtrl.Crit = UnitCtrl.unitData.baseCritChanceRate;
                        SetObject.transform.position = new Vector3(CellWorldPosFinal.x, -1, CellWorldPosFinal.z);
                        GridManager.SetTilePlaceable(TargetCellWorldPos, true, false);
                        TargetCellPlaceable.Clear();
                        TargetCellIdx.Clear();
                    }
                }
                else
                {
                    unitAnimator.SetTrigger(CONSTANT.ANITRIGGER_SPECIAL);
                    TargetEnemy.ReceivePhysicalDamage(damage, CritChanceRate, attackType, debuff);
                }

                
            }

            /*
            switch (SkillCode)
            {
                //낫 찍기
                case 101:
                    //낫으로 앞에 있는 한 명의 적을 세게 찍어 5 데미지의 관통 공격을 가한다. 치명타율 5% 증가. 치명타 발동 시 충격 효과
                    // 검베기와 같은 처리. 스탯만 다르게.
                    if (TargetEnemy != null && UnitCtrl.isEnemyInRange)
                    {
                        unitAnimator.SetTrigger(CONSTANT.ANITRIGGER_SPECIAL);
                        TargetEnemy.ReceivePhysicalDamage(damage, CritChanceRate, attackType, debuff);
                    }
                    break;
                //연사
                case 102:
                    if (TargetEnemy == null && !UnitCtrl.isEnemyInRange)
                    {
                        return;
                    }
                    else
                    {
                        Bow.transform.LookAt(UnitCtrl.targetEnemy.transform.position);
                        Bow.GetComponent<BowCtrl>().DoubleShot();
                        TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.baseCritChanceRate + 5, AttackType.Pierce);
                    }    
                    break;
                //멀리베기
                case 201:
                    //창에 달린 낫으로 멀리 있는 적을 베어 7 데미지의 베기 공격을 가한다. 치명타율 10% 증가. 치명타 발동 시 출혈 효과
                    //스킬을 사용할때만 거리 측정을 따로 해야할것 같음. -> 시야범위? 기획쪽과 이야기가 필요할것 같음.
                    if (TargetEnemy != null && UnitCtrl.isEnemyInRange)
                    {
                        TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.baseCritChanceRate + 10, AttackType.Slash, UnitDebuff.Bleed);
                    }
                    break;
                //방패치기
                case 202:
                    if (TargetEnemy != null && UnitCtrl.isEnemyInRange)
                    {
                        TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.baseCritChanceRate + 10, AttackType.Crush, UnitDebuff.Stun);
                    }
                    break;
                //수류탄 투척
                case 203:
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
                case 204:
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
                        attackCtrl.Crit = UnitCtrl.unitData.baseCritChanceRate;
                        SetObject.transform.position = new Vector3(CellWorldPosFinal.x, -1, CellWorldPosFinal.z);
                        GridManager.SetTilePlaceable(TargetCellWorldPos, true, false);
                        TargetCellPlaceable.Clear();
                        TargetCellIdx.Clear();
                    }
                    break;
            }
            */
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
    public void EnemyGeneralSkill(int SkillCode, GameObject TargetEnemy, float weaponCooldown, bool isEnemyAttack)
    {
        Vector3 TargetPos = TargetEnemy.transform.position;

        //공격 정보 관리 (데미지, 치명타 확률, 공격 타입, 디버프(필요한 경우))
        int damage = UnitCtrl.unitData.generalSkillDamage;
        AttackType attackType = UnitCtrl.unitData.generalAttackType;
        int CritChanceRate = UnitCtrl.unitData.baseCritChanceRate + UnitCtrl.unitData.bonusCritChanceRate;
        UnitDebuff debuff = UnitCtrl.unitData.generalSkillDebuff;
        bool AttackTargetPoint = true;

        if (weaponCooldown_Cur <= 0)
        {
            unitAnimator.SetTrigger(CONSTANT.ANITRIGGER_ATTACK);

            if (SkillCode == 202)
            {
                if (Bow != null)
                {
                    Bow.transform.LookAt(TargetPos);
                    Bow.GetComponent<BowCtrl>().ArrowShoot(true);
                }
            }

            /*
            //switch (SkillCode)
            //{
            //    //햘퀴기
            //    case 101:
            //        damage = 10;
            //        attackType = AttackType.Slash;
            //        debuff = UnitDebuff.Bleed;
            //        break;
            //    //망치질
            //    case 102:
            //        damage = 10;
            //        attackType = AttackType.Crush;
            //        debuff = UnitDebuff.Dizzy;
            //        break;
            //    //검 베기
            //    case 103:
            //        damage = 10;
            //        attackType = AttackType.Slash;
            //        debuff = UnitDebuff.Bleed;
            //        break;
            //    //창 찌르기
            //    case 201:
            //        damage = 12;
            //        attackType = AttackType.Pierce;
            //        debuff = UnitDebuff.Bleed;
            //        break;
            //    //활 쏘기
            //    case 202:
            //        if (Bow != null)
            //        {
            //            Bow.transform.LookAt(TargetPos);
            //            Bow.GetComponent<BowCtrl>().ArrowShoot(true);
            //        }
            //        damage = 6;
            //        attackType = AttackType.Pierce;
            //        debuff = UnitDebuff.Bleed;
            //        break;
            //    //칼의 속삭임
            //    case 203:
            //        damage = 10;
            //        attackType = AttackType.Slash;
            //        debuff = UnitDebuff.Bleed;
            //        break;
            //    //방페 앞세우기
            //    case 204:
            //        damage = 10;
            //        attackType = AttackType.Crush;
            //        debuff = UnitDebuff.Dizzy;
            //        break;
            //    //독가스 방출
            //    case 205:
            //        break;
            //    case 206:
            //        damage = 10;
            //        attackType = AttackType.Slash;
            //        debuff = UnitDebuff.Bleed;
            //        break;
            //    case 301:
            //        damage = 14;
            //        attackType = AttackType.Crush;
            //        debuff = UnitDebuff.Dizzy;
            //        break;
            //    case 302:
            //        //5초 동안 자신 주변의 아군에게 재생 버프
            //        break;
            //    //피의 향연
            //    case 303:
            //        damage = 10;
            //        attackType = AttackType.Slash;
            //        //치명타시 자신에게 재생 버프
            //        break;
            //    case 401:
            //        damage = 16;
            //        attackType = AttackType.Slash;
            //        debuff = UnitDebuff.Bleed;
            //        AttackTargetPoint = false;
            //        break;
            //    case 501:
            //        damage = 18;
            //        attackType = AttackType.Slash;
            //        debuff = UnitDebuff.Trapped;
            //        AttackTargetPoint = false;
            //        break;
            //}
            */

            if (TargetEnemy.gameObject.CompareTag(CONSTANT.TAG_UNIT))
            {
                if (AttackTargetPoint)
                {
                    Ingame_UnitCtrl EnemyCtrl = TargetEnemy.GetComponent<Ingame_UnitCtrl>();
                    int HitSoundRandomNum = Random.Range(0, 2);
                    AudioClip SFX2Play = UnitCtrl.unitData.attackSound[HitSoundRandomNum];

                    UnitCtrl.soundManager.PlaySFX(UnitCtrl.soundManager.ATTACK_SFX, SFX2Play);
                    EnemyCtrl.ReceivePhysicalDamage(damage, CritChanceRate, attackType, debuff, UnitCtrl.unitData.attackVFX);
                }
                else
                {
                    if (AttackTrigger != null)
                    {
                        GameObject AttackTriggerObj = Instantiate(UnitCtrl.unitData.attackVFX, UnitCtrl.transform);
                        AttackCtrl attackCtrl = AttackTriggerObj.GetComponent<AttackCtrl>();
                        attackCtrl.Damage = damage;
                        attackCtrl.Crit = CritChanceRate;
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
