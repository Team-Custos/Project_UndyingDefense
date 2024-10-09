using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UnitSkillManager : MonoBehaviour
{
    public Dictionary<int, bool> TargetCellPlaceable = new Dictionary<int, bool>
    { };

    public List<int> TargetCellIdx = new List<int>();

    public GameObject Bow;
    public GameObject Sword;

    public GameObject Trap;

    float weaponCooldown_Cur = 0;
    float skillCooldown_Cur = 0;

    public Ingame_UnitCtrl UnitCtrl;
    GridManager GridManager;

    public GameObject SetObject = null;

    int TargetCellIdxFinal = 0;

    public Dictionary<int, UnitDebuff> GeneralSkillCodeToDebuff = new Dictionary<int, UnitDebuff>()
    {
        {101 , UnitDebuff.Bleed},
        {102 , UnitDebuff.Bleed},
    };

    private void Awake()
    {
        GridManager = GridManager.inst;
    }

    public void UnitGeneralSkill(int SkillCode, GameObject TargetEnemy, float weaponCooldown, bool isEnemyAttack)
    {
        Vector3 TargetPos = TargetEnemy.transform.position;

        //공격 정보 관리 (데미지, 치명타 확률, 공격 타입, 디버프(필요한 경우))
        int damage = 0;
        AttackType attackType = AttackType.UnKnown;
        UnitDebuff debuff = UnitDebuff.None;

        switch (SkillCode)
        {
            case 101://낫 베기
                     //낫으로 앞에 있는 한 명의 적을 베어 5 데미지의 베기 공격을 가한다. 치명타 발동 시 출혈 효과
                damage = 5;
                attackType = AttackType.Slash;
                debuff = UnitDebuff.Bleed;
                break;
            case 102://활 쏘기
                     //화살을 쏘아 한 명의 적에게 5 데미지의 관통 공격을 가한다. 치명타 발동 시 출혈 효과
                if (Bow != null && Bow.GetComponent<BowCtrl>() != null)
                {
                    Bow.transform.LookAt(TargetPos);
                    Bow.GetComponent<BowCtrl>().ArrowShoot(isEnemyAttack);
                    if (TargetEnemy.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
                    {
                        Ingame_UnitCtrl EnemyCtrl = TargetEnemy.GetComponent<Ingame_UnitCtrl>();
                        EnemyCtrl.ReceivePhysicalDamage(UnitCtrl.unitData.attackPoint, UnitCtrl.unitData.critChanceRate, AttackType.Pierce, UnitDebuff.Bleed);
                    }
                    else
                    {
                        BaseStatus BaseCtrl = TargetEnemy.GetComponent<BaseStatus>();
                        BaseCtrl.ReceiveDamage(UnitCtrl.unitData.attackPoint);
                    }
                }
                break;
            case 201://창 찌르기
                damage = 7;
                attackType = AttackType.Pierce;
                debuff = UnitDebuff.Bleed;
                break;
            case 202://망치 내려치기
                damage = 7;
                attackType = AttackType.Crush;
                debuff = UnitDebuff.Dizzy;
                break;
            case 203://엽총 쏘기
                damage = 7;
                attackType = AttackType.Pierce;
                debuff = UnitDebuff.Bleed;
                break;
        }

        

        if (weaponCooldown_Cur <= 0)
        {
            if (TargetEnemy.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                Ingame_UnitCtrl EnemyCtrl = TargetEnemy.GetComponent<Ingame_UnitCtrl>();
                EnemyCtrl.ReceivePhysicalDamage(damage, UnitCtrl.unitData.critChanceRate, attackType, debuff);
            }
            else
            {
                BaseStatus BaseCtrl = TargetEnemy.GetComponent<BaseStatus>();
                BaseCtrl.ReceiveDamage(UnitCtrl.unitData.attackPoint);
            }
            weaponCooldown_Cur = weaponCooldown;
        }
        else
        {
            weaponCooldown_Cur -= Time.deltaTime;
        }
    }

    int SkillDamageInit()
    {
        return 0;
    }


    public void UnitSpecialSkill(int SkillCode,float skillCooldown)
    {
        int SkillDamage = SkillDamageInit();



        Ingame_UnitCtrl TargetEnemy = null;
        if (UnitCtrl.targetEnemy != null)
        {
            TargetEnemy = UnitCtrl.targetEnemy.GetComponent<Ingame_UnitCtrl>();
        }

        if (skillCooldown_Cur <= 0)
        {
            skillCooldown_Cur = skillCooldown;
            switch (SkillCode)
            {
                case 101://낫 찍기
                         //낫으로 앞에 있는 한 명의 적을 세게 찍어 5 데미지의 관통 공격을 가한다. 치명타율 5% 증가. 치명타 발동 시 충격 효과
                         // 검베기와 같은 처리. 스탯만 다르게.
                    if (TargetEnemy == null)
                    {
                        return;
                    }

                    TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + 5, AttackType.Pierce, UnitDebuff.Dizzy);
                    break;

                case 102://연사
                    if (TargetEnemy == null)
                    {
                        return;
                    }

                    StartCoroutine(DoubleShot());

                    IEnumerator DoubleShot()
                    {
                        TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + 5, AttackType.Pierce, UnitDebuff.Bleed);
                        yield return new WaitForSeconds(0.5f);
                    }
                    TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + 5, AttackType.Pierce, UnitDebuff.Bleed);

                    break;
                case 201://멀리베기
                    break;
                case 202://방패치기
                    TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.unitData.critChanceRate + 10, AttackType.Crush, UnitDebuff.Stun);
                    break;
                case 203://수류탄 투척
                    break;
                case 204://덫 설치
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
                    Vector2[] directionOffsets;
                    if (CurAngle <= 45 || CurAngle > 315)       // 북쪽
                    {
                        directionOffsets = new Vector2[] { new Vector2(-1, 2), new Vector2(0, 2), new Vector2(1, 2) };
                    }
                    else if (CurAngle <= 135 && CurAngle > 45)   // 동쪽
                    {
                        directionOffsets = new Vector2[] { new Vector2(2, 1), new Vector2(2, 0), new Vector2(2, -1) };
                    }
                    else if (CurAngle <= 225 && CurAngle > 135)  // 남쪽
                    {
                        directionOffsets = new Vector2[] { new Vector2(1, -2), new Vector2(0, -2), new Vector2(-1, -2) };
                    }
                    else if (CurAngle > 225 && CurAngle <= 315) // 서쪽
                    {
                        directionOffsets = new Vector2[] { new Vector2(-2, -1), new Vector2(-2, 0), new Vector2(-2, 1) };
                    }
                    else
                    {
                        Debug.LogError("덫 설치 관련 각도 계산 오류.");
                        directionOffsets = new Vector2[] { new Vector2(-1, 2), new Vector2(0, 2), new Vector2(1, 2) };
                    }

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
                        Vector3 CellWorldPosFinal = TargetCellWorldPos+ new Vector3(GridManager.mapGrid.cellSize.x * 0.5f,0, GridManager.mapGrid.cellSize.y * 0.5f);
                        
                            //GridManager.mapGrid.GetCellCenterWorld(new Vector3Int((int)TargetCells[TargetCellFinal].x, 1, (int)TargetCells[TargetCellFinal].y));
                        SetObject = Instantiate(Trap);
                        SetObject.transform.position = new Vector3(CellWorldPosFinal.x,-1,CellWorldPosFinal.z);
                        GridManager.SetTilePlaceable(TargetCellWorldPos, true, false);
                        TargetCellPlaceable.Clear();
                        TargetCellIdx.Clear();
                    }
                    break;
            }
        }
        else 
        {
            skillCooldown_Cur -= Time.deltaTime;
        }

        
    }

    public void EnemyGeneralSkill(int SkillCode, Vector3 TargetPos)
    {
        switch (SkillCode)
        {
            case 101://검 베기
                break;
            case 102://활 쏘기
                if (Bow != null)
                {
                    Bow.transform.LookAt(TargetPos);
                    Bow.GetComponent<BowCtrl>().ArrowShoot(true);
                }
                break;
        }
    }
}
