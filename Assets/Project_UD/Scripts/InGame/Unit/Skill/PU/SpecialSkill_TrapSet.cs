using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SpecialSkill_TrapSet : AttackSkill
{
    [Header("====Required Component====")]
    UnitCtrl_ReBuild UnitCtrl;
    GridManager GridManager;//설치에 필요한 위치 불러올 그리드.
    public GameObject Trap_Obj;
    public Dictionary<int, bool> TargetCellPlaceable = new Dictionary<int, bool> { };
    public List<int> TargetCellIdx = new List<int>();
    int TargetCellIdxFinal = 0;//최종적으로 설치될 위치의 셀.

    private void Awake()
    {
        UnitCtrl = GetComponentInParent<UnitCtrl_ReBuild>();
        GridManager = GridManager.inst;
    }

    public override void Activate(UnitCtrl_ReBuild target)
    {
        Debug.Log("TestSkill2 Activate");
        if (Trap_Obj != null)
        {
            GridManager.SetTilePlaceable(Trap_Obj.transform.position, true, true);
            Destroy(Trap_Obj);
            Trap_Obj = null;
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
                        UnitSkillCooldownInit();
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
            UnitSkillCooldownInit();
            return;
        }

        if (GridManager.GetTilePlaceable(TargetCellWorldPos))
        {
            Vector3 CellWorldPosFinal = TargetCellWorldPos + new Vector3(GridManager.mapGrid.cellSize.x * 0.5f, 0, GridManager.mapGrid.cellSize.y * 0.5f);

            //GridManager.mapGrid.GetCellCenterWorld(new Vector3Int((int)TargetCells[TargetCellFinal].x, 1, (int)TargetCells[TargetCellFinal].y));
            Trap_Obj = Instantiate(Trap_Obj);
            AttackCtrl attackCtrl = Trap_Obj.GetComponent<AttackCtrl>();
            attackCtrl.Crit = UnitCtrl.curCrit;
            Trap_Obj.transform.position = new Vector3(CellWorldPosFinal.x, -1, CellWorldPosFinal.z);
            GridManager.SetTilePlaceable(TargetCellWorldPos, true, false);
            TargetCellPlaceable.Clear();
            TargetCellIdx.Clear();
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
}
