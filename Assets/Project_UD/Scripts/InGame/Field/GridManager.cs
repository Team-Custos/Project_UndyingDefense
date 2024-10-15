using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
using System;


public class GridManager : MonoBehaviour
{
    public static GridManager inst;

    public int _width, _height;

    [SerializeField] private GridTile _tilePrefab;

    public Dictionary<Vector2, bool> _tiles = new Dictionary<Vector2, bool>
    { };

    public GameObject TILEPARENT;

    public GridTile[] Tiles_Obj;
    int Tiles_idx;

    public Grid mapGrid;
    public Tilemap groundTilemap;

    public float tile_Offset;

    private void Awake()
    {
        inst = this;
        Tiles_Obj = FindObjectsOfType<GridTile>();
        for (Tiles_idx = 0; Tiles_idx < Tiles_Obj.Length; Tiles_idx++)
        {
            Tiles_Obj[Tiles_idx].GridPos = new Vector2(mapGrid.WorldToCell(Tiles_Obj[Tiles_idx].transform.position).x, mapGrid.WorldToCell(Tiles_Obj[Tiles_idx].transform.position).y);
            _tiles.Add(new Vector2(Tiles_Obj[Tiles_idx].GridPos.x, Tiles_Obj[Tiles_idx].GridPos.y), true);
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public Vector2 GetTilePos(Vector3 pos)
    {
        Vector2 TilePos = new Vector2(groundTilemap.WorldToCell(pos).x, groundTilemap.WorldToCell(pos).y);
        return TilePos;
    }

    public void SetTilePlaceable(Vector3 pos, bool SetManualMode, bool PlaceableToSetManual)
    {
        Vector3Int CurCellPos = groundTilemap.WorldToCell(pos);
        Vector3 CurCellWorldPos = new Vector3 (groundTilemap.GetCellCenterWorld(CurCellPos).x,0,groundTilemap.GetCellCenterWorld(CurCellPos).z);

        Vector3 CurUnitWorldPos = new Vector3(pos.x, 0, pos.z);

        //Debug.Log("현재 배치 가능 여부 : " + _tiles[CurCellPos]);
        //Debug.Log("현재 위치한 타일의 그리드 좌표 : " + CurCellPos);
        //Debug.Log("현재 위치한 타일의 월드 좌표 : " + CurCellWorldPos);

        // 유닛과 타일의 중심 간의 거리 계산 (x와 z만 비교)
        float distanceX = Mathf.Abs(CurCellWorldPos.x - CurUnitWorldPos.x);
        float distanceZ = Mathf.Abs(CurCellWorldPos.z - CurUnitWorldPos.z);

        //Debug.Log(new Vector2(distanceX,distanceZ));

        if (SetManualMode == true)//직접 설정할경우
        {
            _tiles[new Vector2(CurCellPos.x, CurCellPos.y)] = PlaceableToSetManual;
            return;
        }
        else
        {
            // 타일 중심과 유닛 위치의 x 및 z 거리 차이를 이용하여 배치 가능 여부를 판단
            if (distanceX > 0.95f || distanceZ > 0.95f)
            {
                // 타일과 유닛이 같은 위치에 있다고 판단 - 배치 가능
                _tiles[new Vector2(CurCellPos.x, CurCellPos.y)] = true;
            }
            else
            {
                // 타일과 유닛이 충분히 가까이 있지 않음 - 배치 불가
                _tiles[new Vector2(CurCellPos.x, CurCellPos.y)] = false;
            }
        }

        
    }

    public bool GetTilePlaceable(Vector3 WorldPosToFind)
    {
        Vector3Int CellPos = groundTilemap.WorldToCell(WorldPosToFind);
        if (_tiles.ContainsKey(new Vector2(CellPos.x, CellPos.y)))
        {
            return _tiles[new Vector2(CellPos.x, CellPos.y)];
        }
        else
        {
            return false;
        }
    }


    public bool IsTilesAllOff()
    {
        if (Tiles_Obj[Tiles_idx].GetComponent<GridTile>().Selected)
        {
            return false;
        }
        else { return true; }
    }

}
