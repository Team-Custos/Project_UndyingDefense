using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_GridTile : MonoBehaviour
{
    UD_Ingame_GameManager GAMEMANAGER;

    UD_Ingame_GridManager GridMgr;

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

    void Start()
    {
        GAMEMANAGER = UD_Ingame_GameManager.inst;
        GridMgr = UD_Ingame_GridManager.inst;

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


        if (showPlacementColors)
        {
            if (!isPlaceable)
            {
                MeshR.material.color = colorOccupied;
            }
            else if (!mouseHover)
            {
                MeshR.material.color = colorAvailable;
            }
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
        MeshR.material.color = colorHighlit;
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
        else
        {
            MeshR.material.color = colorAvailable;
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
}
