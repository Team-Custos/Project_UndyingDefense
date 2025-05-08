//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using DG.Tweening;
//using UnityEngine.Tilemaps;
//using System;
//public class GridManager : MonoBehaviour
//{
//    private Dictionary<Vector3, bool> occupiedgridDic = new Dictionary<Vector3, bool>();
//    public Dictionary<Vector3, bool> OccupiedGridDic => occupiedgridDic;

//    // 그리드 등록
//    // 그리드 확인
//    // 그리드 해제

//    [SerializeField] private Grid grid;

//    public bool IsGridOccupied(AllyUnit allyUnit, out Vector3 gridPos)
//    {
//        if (GetOccupiedGridCenters(allyUnit.GridCollider, out gridPos))
//        {
//            return true;
//            //if (!gridOccupyDic.TryGetValue(gridPos, out bool isOccupied) || !isOccupied)
//            //{
//            //    gridOccupyDic[gridPos] = true;
//            //    return true;
//            //}
//        }

//        gridPos = default;
//        return false;
//    }

//    public bool GetOccupiedGridCenters(Collider unitCollider, out Vector3 resultGrid)
//    {
//        resultGrid = default;

//        List<Vector3> occupiedGridCenters = new List<Vector3>();

//        Bounds bounds = unitCollider.bounds;
//        Vector3 min = bounds.min;
//        Vector3 max = bounds.max;

//        for (float x = min.x; x <= max.x; x += grid.cellSize.x)
//        {
//            for (float z = min.z; z <= max.z; z += grid.cellSize.z)
//            {
//                Vector3 worldPos = new Vector3(x, 0, z);
//                Vector3Int cellPos = grid.WorldToCell(worldPos);

                
//                // 딕셔너리에 등록되어 있지 않으면 딕셔너리와 리스트에 추가
//                if(!occupiedgridDic.ContainsKey(grid.GetCellCenterWorld(cellPos)))
//                {
//                    occupiedgridDic.Add(grid.GetCellCenterWorld(cellPos), false);
//                    occupiedGridCenters.Add(grid.GetCellCenterWorld(cellPos));
//                }   // 딕셔너리에 등록되어있지만 false면 리스트에 추가
//                else if (occupiedgridDic[grid.GetCellCenterWorld(cellPos)] == false) 
//                {
//                    occupiedGridCenters.Add(grid.GetCellCenterWorld(cellPos));
//                }
//                else if(occupiedgridDic[grid.GetCellCenterWorld(cellPos)] == true)
//                {
//                    Debug.Log("이미 점유된 그리드 : " + grid.GetCellCenterWorld(cellPos));
//                }
//            }
//        }

//        //foreach(var gridCenter in occupiedGridCenters)
//        //{
//        //    Debug.Log("등록된 자표 : " + gridCenter);
//        //    Debug.Log(gridOccupyDic[gridCenter]);
//        //}


//        if (occupiedGridCenters.Count == 0)
//            return false;

//        Vector3 unitPos = unitCollider.transform.position;

//        occupiedGridCenters.Sort((a, b) =>Vector3.Distance(unitPos, a)
//        .CompareTo(Vector3.Distance(unitPos, b)));


//        resultGrid = occupiedGridCenters[0];

//        //Debug.Log("확정 그리드 : " + resultGrid);

//        return true;


//        //return occupiedGridCenters;
//    }

//    public void ClearGrid(Vector3 cellPosition)
//    {
//        if (occupiedgridDic.ContainsKey(cellPosition))
//        {
//            occupiedgridDic[cellPosition] = false;
//        }
//    }


//    // 유닛에 겹쳐져 있는 그리드 중 이동할 그리드를 가져옴, 또는 nll
//    //public bool TryGetAvailableGrid(Collider unitCollider, out Vector3Int resultGrid)
//    //{
//    //    resultGrid = default;

//    //    List<Vector3Int> overlappedGrid = new List<Vector3Int>();

//    //    Bounds bounds = unitCollider.bounds;
//    //    Vector3 min = bounds.min;
//    //    Vector3 max = bounds.max;

//    //    for (float x = min.x; x <= max.x; x += 0.5f)
//    //    {
//    //        for (float z = min.z; z <= max.z; z += 0.5f)
//    //        {
//    //            Vector3 worldPos = new Vector3(x, 0, z);
//    //            Vector3Int cellPos = grid.WorldToCell(worldPos);
//    //            if (!overlappedGrid.Contains(cellPos))
//    //            {
//    //                overlappedGrid.Add(cellPos);
//    //            }
//    //        }
//    //    }

//    //    if (overlappedGrid.Count == 0)
//    //        return false;

//    //    Vector3 unitPos = unitCollider.transform.position;
//    //    overlappedGrid.Sort((a, b) =>
//    //        Vector3.Distance(unitPos, grid.GetCellCenterWorld(a))
//    //        .CompareTo(Vector3.Distance(unitPos, grid.GetCellCenterWorld(b)))
//    //    );

//    //    resultGrid = overlappedGrid[0];

//    //    return true;
//    //}


//}