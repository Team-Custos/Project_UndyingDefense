using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_GridTile : MonoBehaviour
{
    UD_Ingame_GameManager GAMEMANAGER;

    public bool Selected = false;
    public Vector2 GridPos = Vector2.zero;

    public Color32 colorDefault = Color.cyan;
    public Color32 colorHighlit = Color.white;
    public Color32 colorSelected = Color.white;

    MeshRenderer MeshR;

    public GameObject currentPlacedUnit;

    bool mouseHover = false;
    public bool isPlaceable = true;

    public Color32 colorOccupied = Color.red; // 유닛이 있는 타일 색
    public Color32 colorAvailable = Color.green; // 배치 가능한 타일 색
    public Color32 colorUnitSiege = Color.yellow; // 시즈 모드인 유닛 타일 색
    public bool showPlacementColors = false;


    public bool changePlacementColors = false;

    public bool isTileOccupied = false;     // 타일에 유닛이 배친된 상태
    public bool isArrangeState = false;     // 유닛 배치 대기 상태

    void Start()
    {
        colorDefault = Color.cyan;

        colorUnitSiege = Color.yellow;

        GAMEMANAGER = UD_Ingame_GameManager.inst;
        MeshR = GetComponent<MeshRenderer>();
        Selected = false;

        GridPos = new Vector2((int)(transform.position.x / 2), (int)(transform.position.z / 2));

        this.gameObject.name = this.name + " " + GridPos.x + " " + GridPos.y;

        UpdateTileColor();
    }

    void Update()
    {

        UpdateTileColor();
    }

    private void OnMouseOver()
    {
        mouseHover = true;
        MeshR.material.color = colorHighlit;

        mouseHover = true;
        //if (!changePlacementColors)
        //{
        //    MeshR.material.color = colorHighlit;
        //}
    }

    private void OnMouseExit()
    {
        mouseHover = false;
        GetComponent<MeshRenderer>().material.color = colorDefault;

        mouseHover = false;
        //if (!changePlacementColors)
        //{
        //    MeshR.material.color = colorDefault;
        //}
    }

    private void OnMouseDown()
    {
        //if (currentPlacedUnit != null)
        //{
        //    return;
        //}

        //if (GAMEMANAGER.AllyUnitSetMode && isPlaceable)
        //{
        //    isPlaceable = false;
        //}
        //else
        //{
        //    Selected = !Selected;
        //}

    }

    public void SetPlaceable(bool placeable)
    {
        isPlaceable = placeable;
        UpdateTileColor();
    }

    public bool IsPlaceable()
    {
        return isPlaceable;
    }

    public void ChangePlacementColors(bool show)
    {
        changePlacementColors = show;
    }

    public void ResetTileColor()
    {
        if (MeshR == null) MeshR = GetComponent<MeshRenderer>();
        MeshR.material.color = colorDefault;
    }

    public void UpdateTileColor()
    {
        if (UD_Ingame_GameManager.inst.AllyUnitSetMode)
        {
            if (currentPlacedUnit != null)
            {
                MeshR.material.color = colorOccupied;
                
            }
            else
            {
                MeshR.material.color = colorAvailable;
            }
        }
        else if (currentPlacedUnit != null)
        {
            if (currentPlacedUnit.GetComponent<UD_Ingame_UnitCtrl>().Ally_Mode == AllyMode.Siege)
            {
                MeshR.material.color = colorUnitSiege;
            }
            else
            {
                MeshR.material.color = colorDefault;
            }
        }
        else
        {
            MeshR.material.color = colorDefault;
        }
    }

    public void SetTileOccupied(bool occupied)
    {
        isTileOccupied = occupied;
        UpdateTileColor(); // 점령 상태 변경 시 타일 색상 업데이트
    }

    public void SetArrangeState(bool arrangeState)
    {
        isArrangeState = arrangeState;
        UpdateTileColor(); // 배치 상태 변경 시 타일 색상 업데이트
    }

    public void ResetTile()
    {
        isTileOccupied = false;  // 타일에 유닛이 없음을 표시
        isPlaceable = true;  // 타일을 다시 배치 가능 상태로 설정
        UpdateTileColor();  // 타일 색상을 기본값 또는 배치 가능 상태에 맞게 변경
    }




    public void ShowPlacementColors(bool show)
    {
        showPlacementColors = show;
    }
}
