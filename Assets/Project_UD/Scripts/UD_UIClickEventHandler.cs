using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UD_UIClickEventHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // UI 요소를 클릭할 때만 이벤트가 전달되도록 설정
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // 클릭된 것이 UI 요소라면 게임 오브젝트에 클릭 이벤트 전달되지 않도록 설정
                Debug.Log("UI Element Clicked");
            }
            else
            {
                // UI 요소가 아닌 다른 게임 오브젝트가 클릭된 경우
                Debug.Log("Game Object Clicked");
            }
        }
    }
}
