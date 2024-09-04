using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UD_Ingame_UnitSkillManager : MonoBehaviour
{
    public Dictionary<int, bool> TargetCellPlaceable = new Dictionary<int, bool>
    { };

    public List<int> TargetCellidx = new List<int>();

    public GameObject Bow;
    public GameObject Sword;

    public GameObject Trap;

    float skillCooldown_Cur = 0;

    public UD_Ingame_UnitCtrl UnitCtrl;
    UD_Ingame_GridManager GridManager;

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
        TargetCellIdxFinal = Random.Range(0, TargetCellidx.Count);
    }

    public void UnitGeneralSkill(int SkillCode, Vector3 TargetPos)
    {
        switch (SkillCode)
        {
            case 101://검 베기
                //근거리 공격 정해지는 대로 작업.

                break;
            case 102://활 쏘기
                if (Bow != null)
                {
                    Bow.transform.LookAt(TargetPos);
                    Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(UnitCtrl.weaponCooldown, UnitCtrl.attackPoint, true);
                }
                break;  
        }
    }

    public void UnitSpecialSkill(int SkillCode, Vector3 TargetPos, float skillCooldown)
    {
        if (skillCooldown_Cur <= 0)
        {
            skillCooldown_Cur = skillCooldown;
            switch (SkillCode)
            {
                case 101://낫 찍기
                         // 검베기와 같은 처리. 스탯만 다르게.
                    break;
                case 102://덫 설치
                         //현재 바라보는 방향으로 설치 범위를 설정. 각도에 따라 범위가 변경. 현재 위치에서 2칸 정도 떨어져 있는 위치의 변 3칸중 랜덤으로 지정하여 설치.
                         //TODO : 난수가 제대로 적용이 되는지 확인해야함.
                    float CurAngle = Mathf.Abs(gameObject.transform.rotation.y % 360);
                    Vector2 CurCellPos = UnitCtrl.unitPos;
                    Vector2[] TargetCells = new Vector2[]
                    {
                        Vector2.zero, Vector2.zero, Vector2.zero
                    };

                    if (CurAngle <= 45 || CurAngle > 315)//북
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            TargetCells[i].y = CurCellPos.y + 2;
                            TargetCells[i].x = CurCellPos.x - 1 + i;
                        }
                    }
                    else if (CurAngle <= 135 || CurAngle > 45)//동
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            TargetCells[i].x = CurCellPos.x + 2;
                            TargetCells[i].y = CurCellPos.y + 1 - i;
                        }
                    }
                    else if (CurAngle <= 225 || CurAngle > 135)//남
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            TargetCells[i].y = CurCellPos.y - 2;
                            TargetCells[i].x = CurCellPos.x + 1 - i;
                        }
                    }
                    else if (CurAngle <= 315 && CurAngle > 225)//서
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            TargetCells[i].x = CurCellPos.x - 2;
                            TargetCells[i].y = CurCellPos.y - 1 + i;
                        }
                    }


                    Vector3 TargetCellWorldPos = Vector3.zero;

                    for (int i = 0; i < 3; i++)
                    {
                        if (!TargetCellPlaceable.ContainsKey(i))
                        {
                            TargetCellPlaceable.Add(i, GridManager.GetTilePlaceable(TargetCells[i]));
                        }
                        if (!TargetCellidx.Contains(i))
                        {
                            TargetCellidx.Add(i);
                        }
                    }

                    int TargetCellFinal = TargetCellidx[TargetCellIdxFinal];

                    while (TargetCellidx.Count != 0)
                    {
                        if (GridManager.GetTilePlaceable(TargetCells[TargetCellIdxFinal]))
                        {
                            TargetCellWorldPos = GridManager.mapGrid.CellToWorld(new Vector3Int((int)TargetCells[TargetCellFinal].x, (int)TargetCells[TargetCellFinal].y, 1));
                            break;
                        }
                        else
                        {
                            TargetCellidx.RemoveAt(TargetCellIdxFinal);
                        }

                        if (TargetCellidx.Count == 0)
                        {
                            Debug.Log("모두 설치 불가. 스킬 쿨타임을 초기화합니다.");
                            skillCooldown_Cur = skillCooldown;
                            return;
                        }
                    }

                    if (GridManager.GetTilePlaceable(TargetCellWorldPos))
                    {
                        Vector3 CellWorldPosFinal = TargetCellWorldPos+ new Vector3(GridManager.mapGrid.cellSize.x * 0.5f,0, GridManager.mapGrid.cellSize.y * 0.5f);
                        
                            //GridManager.mapGrid.GetCellCenterWorld(new Vector3Int((int)TargetCells[TargetCellFinal].x, 1, (int)TargetCells[TargetCellFinal].y));
                        GameObject TrapObj = Instantiate(Trap);
                        TrapObj.transform.position = new Vector3(CellWorldPosFinal.x,-1,CellWorldPosFinal.z);
                        GridManager.SetTilePlaceable(TargetCellWorldPos, false);
                        TargetCellPlaceable.Clear();
                        TargetCellidx.Clear();
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
                    Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(UnitCtrl.weaponCooldown, UnitCtrl.attackPoint, true);
                }
                break;
        }
    }
}
