using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UD_Ingame_UnitSkillManager : MonoBehaviour
{
    public Dictionary<int, bool> TargetCellPlaceable = new Dictionary<int, bool>
    { };

    public List<int> TargetCellIdx = new List<int>();

    public GameObject Bow;
    public GameObject Sword;

    public GameObject Trap;

    float weaponCooldown_Cur = 0;
    float skillCooldown_Cur = 0;

    public UD_Ingame_UnitCtrl UnitCtrl;
    UD_Ingame_GridManager GridManager;

    public GameObject SetObject = null;

    int TargetCellIdxFinal = 0;


    private void Awake()
    {
        GridManager = UD_Ingame_GridManager.inst;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //TargetCellIdxFinal = Random.Range(0, TargetCellidx.Count);
    }

    public void UnitGeneralSkill(int SkillCode, UD_Ingame_UnitCtrl TargetEnemy, float weaponCooldown, bool isEnemyAttack)
    {
        Vector3 TargetPos = TargetEnemy.transform.position;
        if (weaponCooldown_Cur <= 0)
        {
            weaponCooldown_Cur = weaponCooldown;
            switch (SkillCode)
            {
                case 101://검 베기
                         //근거리 공격 정해지는 대로 작업. -> 타겟팅 방식으로 재작업중.
                    TargetEnemy.ReceivePhysicalDamage(UnitCtrl.attackPoint, UnitCtrl.critChanceRate, AttackType.Slash);
                    break;
                case 102://활 쏘기
                    if (Bow != null && Bow.GetComponent<UD_Ingame_BowCtrl>() != null)
                    {
                        Bow.transform.LookAt(TargetPos);
                        Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(isEnemyAttack);
                        TargetEnemy.ReceivePhysicalDamage(UnitCtrl.attackPoint, UnitCtrl.critChanceRate, AttackType.Pierce);
                    }
                    break;
            }
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



        UD_Ingame_UnitCtrl TargetEnemy = null;
        if (UnitCtrl.targetEnemy != null)
        {
            TargetEnemy = UnitCtrl.targetEnemy.GetComponent<UD_Ingame_UnitCtrl>();
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

                    TargetEnemy.ReceivePhysicalDamage(SkillDamage, UnitCtrl.critChanceRate + 5, AttackType.Pierce);

                    break;
                case 102://덫 설치
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
                    Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(true);
                }
                break;
        }
    }
}
