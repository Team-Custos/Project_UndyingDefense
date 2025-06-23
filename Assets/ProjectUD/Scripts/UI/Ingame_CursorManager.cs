using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Ingame_CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D defaultArrowCursor;        // 기본 커서
    [SerializeField] private Texture2D greenArrowCursor;          // 배치 가능 또는 상호작용 가능 ui
    [SerializeField] private Texture2D redArrowCursor;            // 배치 불가능 또는 상호작용 불가능 ui
    [SerializeField] private Texture2D fingerCursor;              // 상호 작용 가능 커서
    [SerializeField] private Texture2D redFingerCursor;           // 상호 작용 불 가능 커서

    [SerializeField] private SelectedUnitManager selectedUnitManager; // 선택된 유닛 매니저
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;

    private Texture2D currentCursor;
    private AllyUnit allyUnit;
    [SerializeField] private PlayerInputEventManager inputEventManager;

    void Start()
    {
        SetDefaultCursor();
    }


    private void Update()
    {
        Texture2D targetCursor = defaultArrowCursor;

        string uiTag = GetPointerOverUITag();
        if (uiTag == "InteractiveUi")
        {
            targetCursor = fingerCursor;
        }
        else if (uiTag == "UnInteractiveUi")
        {
            targetCursor = redFingerCursor;
        }
        else
        {

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Unit"))
                {
                    targetCursor = fingerCursor;
                }
                else if (hit.collider.CompareTag("Tile"))
                {
                    Tile tile = hit.collider.GetComponent<Tile>();

                    if (selectedUnitManager.SelectedUnit == null ||
                        selectedUnitManager.SelectedUnit is EnemyUnit)
                    {
                        targetCursor = defaultArrowCursor;
                    }

                    if(selectedUnitManager.SelectedUnit != null)
                    {
                        allyUnit = selectedUnitManager.SelectedUnit as AllyUnit;

                        if (selectedUnitManager.SelectedUnit is AllyUnit &&
                            allyUnit.ModeType == AllyUnit.Mode.FREE)
                        {
                            
                            targetCursor = greenArrowCursor;
                        }
                        else
                        {
                            targetCursor = defaultArrowCursor;
                        }
                    }
                    
                }
                else
                {
                    targetCursor = defaultArrowCursor;
                }
            }
        }

        //커서 변경이 필요할 때만 변경
        if (currentCursor != targetCursor)
        {
            Cursor.SetCursor(targetCursor, Vector2.zero, CursorMode.Auto);
            currentCursor = targetCursor;
        }
    }

    //// UI 위에 있는지 확인
    //string uiTag = GetPointerOverUITag();

    //if (uiTag == "InteractiveUi")
    //{
    //    InteractiveCursor(); // 상호작용 가능한 UI 커서
    //    return;
    //}
    //else if (uiTag == "UnInteractiveUi")
    //{
    //    UnInteractiveCursor(); // 상호작용 불가능한 UI 커서
    //    return;
    //}

    //// Ray를 사용하여 마우스 위치에 있는 오브젝트 확인
    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //if (Physics.Raycast(ray, out RaycastHit hit))
    //{


    //    if (hit.collider.tag == CONSTANT.TAG_UNIT || hit.collider.tag == CONSTANT.TAG_ENEMY)
    //    {
    //        SetFingerCursor();
    //    }
    //    else if (hit.collider.tag == CONSTANT.TAG_TILE)
    //    {

    //        SetDefaultCursor();

    //        if (InGameManager.inst.UnitSetMode)
    //        {
    //            // GridTile 컴포넌트 가져오기
    //            GridTile gridTile = hit.collider.GetComponent<GridTile>();
    //            if (gridTile != null && gridTile.IsPlaceable())
    //            {
    //                // 배치 가능한 타일일 때 커서 변경
    //                InteractiveCursor();
    //            }
    //            else
    //            {
    //                // 배치 불가능한 타일일 때 기본 커서로 설정
    //                UnInteractiveCursor();

    //            }
    //        }
    //        else if (GameOrderSystem.instance.selectedUnit != null &&
    //                 GameOrderSystem.instance.selectedUnit.GetComponent<Ingame_UnitCtrl>().Ally_Mode == AllyMode.Free)
    //        {
    //            // GridTile 컴포넌트 가져오기
    //            GridTile gridTile = hit.collider.GetComponent<GridTile>();
    //            if (gridTile != null && gridTile.IsPlaceable())
    //            {
    //                // 배치 가능한 타일일 때 커서 변경
    //                InteractiveCursor();
    //            }
    //            else
    //            {
    //                // 배치 불가능한 타일일 때 기본 커서로 설정
    //                UnInteractiveCursor();

    //            }
    //        }
    //    }
    //    else
    //    {
    //        SetDefaultCursor();
    //    }
    //}

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultArrowCursor, Vector2.zero, CursorMode.Auto);
    }
    public void SetFingerCursor()
    {
        Cursor.SetCursor(fingerCursor, Vector2.zero, CursorMode.Auto);
    }

    public void InteractiveCursor()
    {
        Cursor.SetCursor(greenArrowCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetUnInteractiveCursor()
    {
        Cursor.SetCursor(redArrowCursor, Vector2.zero, CursorMode.Auto);
    }


    // UI 위에 있는지 확인
    private string GetPointerOverUITag()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("InteractiveUi"))
            {
                return "InteractiveUi";
            }
            else if (result.gameObject.CompareTag("UnInteractiveUi"))
            {
                return "UnInteractiveUi";
            }
        }

        return null; // UI 위에 있지 않음
    }
}
