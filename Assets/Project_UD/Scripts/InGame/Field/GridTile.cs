using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridTile : MonoBehaviour
{
    InGameManager GAMEMANAGER;

    GridManager GridMgr;

    public bool Selected = false;
    public Vector2 GridPos = Vector2.zero;

    public Color32 colorDefault = Color.white;
    public Color32 colorHighlit = Color.white;
    public Color32 colorSelected = Color.white;

    MeshRenderer MeshR;

    public GameObject currentPlacedUnit;

    bool mouseHover = false;
    public bool isPlaceable = true;

    public Color32 colorOccupied = Color.red; // 유닛이 있는 타일 색상
    public Color32 colorAvailable = Color.green; // 유닛이 없는 타일 색상
    public bool showPlacementColors = false;
    private bool isTileSelected = false; // 프리모드 유닛 이동시 지정된 타일 색상

    void Start()
    {
        GAMEMANAGER = InGameManager.inst;
        GridMgr = GridManager.inst;

        MeshR = GetComponent<MeshRenderer>();
        Selected = false;

        this.gameObject.name = this.name + " " + GridPos.x + " " + GridPos.y;

        colorDefault = new Color(0, 0, 0, 0);
    }

    void UpdateTilePlaceable()
    {
        //Debug.Log(GridPos + " " + GridMgr._tiles[GridPos]);
        isPlaceable = GridMgr._tiles[GridPos];
    }

    void Update()
    {
        UpdateTilePlaceable();

        // 타일 색상 설정
        if (isTileSelected) // 유닛이 이동할 타일로 지정된 경우
        {
            MeshR.material.color = colorSelected;
            isTileSelected = false;
        }
        if (showPlacementColors)
        {
            if (mouseHover && isPlaceable)
            {
                // 마우스가 UI 위에 있는지 확인
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    // UI 위에 있을 경우 이벤트 무시
                    return;
                }

                MeshR.material.color = colorAvailable;
            }
            else if (mouseHover && !isPlaceable)
            {
                MeshR.material.color = colorOccupied;
            }
            else if (!mouseHover)
            {
                MeshR.material.color = colorDefault;
            }

            //if (!isPlaceable)
            //{
            //    MeshR.material.color = colorOccupied;
            //}
            //else if (!mouseHover)
            //{
            //    MeshR.material.color = colorAvailable;
            //}
        }
        else
        {
            if (Selected)
            {
                MeshR.material.color = colorSelected;
            }
            else if (!mouseHover)
            {
                MeshR.material.color = colorDefault;
            }
        }
    }

    private void OnMouseOver()
    {
        mouseHover = true;
    }

    private void OnMouseExit()
    {
        //    mouseHover = false;
        //    GetComponent<MeshRenderer>().material.color = colorDefault;


        mouseHover = false;
        if (!showPlacementColors)
        {
            MeshR.material.color = colorDefault;
        }
    }



    public void SetPlaceable(bool placeable)
    {
        isPlaceable = placeable;
    }

    public bool IsPlaceable()
    {
        return isPlaceable;
    }

    public void ShowPlacementColors(bool show)
    {
        showPlacementColors = show;
    }

    public void SelectedTile(bool selected)
    {
        isTileSelected = selected;

        if (!selected)
        {
            // 선택 해제 시 기본 색상으로 즉시 변경
            MeshR.material.color = colorDefault;
        }
    }
}
