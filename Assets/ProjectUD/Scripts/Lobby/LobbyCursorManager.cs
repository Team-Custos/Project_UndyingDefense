using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyCursorManager : MonoBehaviour
{
    public Texture2D defaultArrowCursor;        // 기본 커서
    public Texture2D greenArrowCursor;          // 배치 가능 또는 상호작용 가능 ui
    public Texture2D redArrowCursor;            // 배치 불가능 또는 상호작용 불가능 ui

    // Start is called before the first frame update
    void Start()
    {
        SetLobbyDefaultCursor();
    }

    // Update is called once per frame
    void Update()
    {
        // UI 위에 있는지 확인
        //string uiTag = LobbyGetPointerOverUITag();

        //if (uiTag == "InteractiveUi")
        //{
        //    LobbyInteractiveCursor(); // 상호작용 가능한 UI 커서
        //    return;
        //}
        //else if (uiTag == "UnInteractiveUi")
        //{
        //    LobbyUnInteractiveCursor(); // 상호작용 불가능한 UI 커서
        //    return;
        //}
        //else
        //{
        //    SetLobbyDefaultCursor();
        //}
    }

    public void SetLobbyDefaultCursor()
    {
        Cursor.SetCursor(defaultArrowCursor, Vector2.zero, CursorMode.Auto);
    }

    public void LobbyInteractiveCursor()
    {
        Cursor.SetCursor(greenArrowCursor, Vector2.zero, CursorMode.Auto);
    }

    public void LobbyUnInteractiveCursor()
    {
        Cursor.SetCursor(redArrowCursor, Vector2.zero, CursorMode.Auto);
    }

    // UI 위에 있는지 확인
    private string LobbyGetPointerOverUITag()
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
