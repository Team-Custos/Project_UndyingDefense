using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    public static MouseCursorManager instance;

    public Texture2D defaultArrowCursor;        // 기본 커서
    public Texture2D greenArrowCursor;          // 배치 가능 또는 상호작용 가능 ui
    public Texture2D redArrowCursor;            // 배치 불가능 또는 상호작용 불가능 ui
    public Texture2D fingerCursor;              // 상호 작용 가능 커서

    public bool isDefaultCursor;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 기본 커서를 설정
        SetDefaultCursor();
    }


    private void Update()
    {
        if(isDefaultCursor)
        {
            SetDefaultCursor();
        }
    }

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

    public void UnInteractiveCursor()
    {
        Cursor.SetCursor(redArrowCursor, Vector2.zero, CursorMode.Auto);
    }


}
