using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//이 스크립트는 배치된 그리트 타일 오브텍트를 관리하기 위한 스크립트입니다.
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
        if (showPlacementColors)        // 배치 상태
        {

            if (mouseHover && isPlaceable)  // 배치 상태이고 배치 가능한 타일이면
            {
                // 마우스가 UI 위에 있는지 확인
                //if (EventSystem.current.IsPointerOverGameObject())
                //{
                //    // UI 위에 있을 경우 이벤트 무시
                //    return;
                //}

                // 마우스가 특정 UI 위에 있는지 확인 (특정 UI만 무시)
                if (IsPointerOverSpecificUI())
                {
                    // 특정 UI 위에 있을 경우 이벤트 무시
                    return;
                }

                MeshR.material.color = colorAvailable;
                //MouseCursorManager.instance.InteractiveCursor();
            }
            else if (mouseHover && !isPlaceable)    // 배치 상태이고 배치 불가능한 타일
            {
                // 마우스가 UI 위에 있는지 확인
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    // UI 위에 있을 경우 이벤트 무시
                    return;
                }

                MeshR.material.color = colorOccupied;
                //MouseCursorManager.instance.UnInteractiveCursor();


            }
            else if (!mouseHover)   // 배치 상태이지만 마우스 로 호버링 안된 타일
            {
                MeshR.material.color = colorDefault;
            }
        }
        else  // 배치 아닌 상태
        {
            if (Selected)
            {
                MeshR.material.color = colorSelected;
            }
            else if (!mouseHover)
            {
                MeshR.material.color = colorDefault;
            }

            //마우스가 UI 위에 있는지 확인
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // UI 위에 있을 경우 이벤트 무시
                return;
            }

        }
    }

    private void OnMouseOver()
    {
        mouseHover = true;

        // 유닛이 프리모드시 이동가능한 타일 색 설정
        if (GameOrderSystem.instance.selectedUnit != null &&
        GameOrderSystem.instance.selectedUnit.GetComponent<Ingame_UnitCtrl>().Ally_Mode == AllyMode.Free)
        {
            //마우스가 UI 위에 있는지 확인
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // UI 위에 있을 경우 이벤트 무시
                return;
            }

            // 마우스가 특정 UI 위에 있는지 확인 (특정 UI만 무시)
            if (IsPointerOverSpecificUI())
            {
                // 특정 UI 위에 있을 경우 이벤트 무시
                return;
            }

            // 이동 가능할 때 파란색으로, 불가능할 때 붉은색으로 표시
            if (isPlaceable)
            {
                MeshR.material.color = colorSelected;
                //MouseCursorManager.instance.InteractiveCursor();
            }
            else
            {
                MeshR.material.color = colorOccupied;
                //MouseCursorManager.instance.UnInteractiveCursor();
            }
        }
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

        //MouseCursorManager.instance.SetDefaultCursor();
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


    private bool IsPointerOverSpecificUI()
    {
        // PointerEventData 생성
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        // Raycast를 통해 현재 마우스 포지션의 UI 요소 감지
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);

        foreach (RaycastResult result in raycastResults)
        {
            // 특정 UI 요소만 무시하고자 할 때 사용
            if (result.gameObject.CompareTag("UI"))
            {
                return true;
            }
        }

        return false;
    }
}
