using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    public static MouseCursorManager instance;

    public Texture2D defaultCursor;         // 기본 커서 이미지
    public Texture2D uiCursor;              // 상호작용 ui 전용 이미지(버튼 등)
    public Texture2D fingerCursor;          // 유닛 배치(가능) 시킬때 사용하는 이미지
    public Texture2D fingerRedCursor;       // 배치, 이동 불가능 타일 표시 이미지

    // 커서 중심점
    public Vector2 hotspot = Vector2.zero;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // 기본 커서를 설정
        SetDefaultCursor();
    }

    

    public void SetUiCursor()
    {
        Cursor.SetCursor(uiCursor, hotspot, CursorMode.Auto);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetFingerCursor()
    {
        Cursor.SetCursor(fingerCursor, Vector2.zero, CursorMode.Auto);
    }

    public void SetFingerRedCursor()
    {
        Cursor.SetCursor(fingerRedCursor, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseEnter()
    {
        // 특정 UI나 오브젝트 위로 마우스가 올라갈 때 커서 변경
        SetUiCursor();
    }

    void OnMouseExit()
    {
        // 마우스가 벗어날 때 기본 커서로 변경
        SetDefaultCursor();
    }
}
