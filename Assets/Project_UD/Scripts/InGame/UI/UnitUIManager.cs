using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitUIManager : MonoBehaviour
{
    [Header("UI Prefabs")]
    public GameObject hpBarPrefab;       // HP 바 프리팹
    public GameObject selectedAllyUIPrefab;     // 아군 선택 UI 프리팹
    public GameObject selectedEnemyUIPrefab;    // 적군 선택 UI 프리팹
    public Image hpBarFill;              // HP 바 채우기 이미지
    public GameObject unitOptionBox;

    [Header("UI Settings")]
    [SerializeField] private Canvas uiCanvas;    // World Space Canvas
    [SerializeField] private Camera mainCamera;  // 메인 카메라
    [SerializeField] private float hpBarYOffset;            // HP 바 Y축 오프셋
    [SerializeField] private float selectedUIYOffset;     // 선택 UI Y축 오프셋

    // 현재 선택된 유닛과 연결된 UI
    private GameObject curHpBar;
    private GameObject curAllySelectUI;
    private GameObject curEnemySelectUI;
    private GameObject curUnitOptionBox;

    private Transform selectedUnitTr;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

    }

    private void Start()
    {

    }

    private void Update()
    {
        if (selectedUnitTr != null)
        {
            // UI들이 유닛을 따라다니도록 설정
            curHpBar.transform.position = selectedUnitTr.position + new Vector3(0, hpBarYOffset, 0);

            if(curAllySelectUI != null)
            {
                curAllySelectUI.transform.position = selectedUnitTr.position + new Vector3(0, selectedUIYOffset, 0);
            }

            if(curEnemySelectUI != null)
            {
                curEnemySelectUI.transform.position = selectedUnitTr.position + new Vector3(0, selectedUIYOffset, 0);
            }
            
            if(curUnitOptionBox != null)
            {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(selectedUnitTr.transform.position);
                screenPos.x += 100;
                screenPos.y -= 20;

                curUnitOptionBox.transform.position = screenPos;
            }
        }
    }

    public void SelcetUnit(Transform unitTransform, bool on, bool isAlly)
    {
        // 유닛 선택시 hp와 선택 파티클 표시
        if (on)
        {
            selectedUnitTr = unitTransform;

            // HP 바가 없으면 생성
            if (curHpBar == null)
            {
                curHpBar = Instantiate(hpBarPrefab);
            }

            // HP 바를 선택된 캔버스의 자식으로 설정
            curHpBar.transform.SetParent(uiCanvas.transform);
            curHpBar.transform.position = selectedUnitTr.position + new Vector3(0, hpBarYOffset, 0);
            curHpBar.transform.rotation = Quaternion.identity;
            curHpBar.SetActive(true);

            // 선택된 유닛의 위치에 UI 표시를 위한 포지션 계산
            Vector3 uiPosition = unitTransform.position + new Vector3(0, selectedUIYOffset, 0);

            if (isAlly)
            {
                // 아군 UI 활성화
                if (curAllySelectUI == null)
                {
                    curAllySelectUI = Instantiate(selectedAllyUIPrefab, transform);
                }
                curAllySelectUI.transform.position = uiPosition;
                curAllySelectUI.SetActive(true);

                // 적군 UI가 있다면 비활성화
                if (curEnemySelectUI != null)
                {
                    curEnemySelectUI.SetActive(false);
                }
            }
            else
            {
                // 적군 UI 활성화
                if (curEnemySelectUI == null)
                {
                    curEnemySelectUI = Instantiate(selectedEnemyUIPrefab, transform);
                }
                curEnemySelectUI.transform.position = uiPosition;
                curEnemySelectUI.SetActive(true);

                // 아군 UI가 있다면 비활성화
                if (curAllySelectUI != null)
                {
                    curAllySelectUI.SetActive(false);
                }
            }
        }
        else
        {
            if(curHpBar != null)
            {
                curHpBar.SetActive(false);
            }

            if (curHpBar != null)
            {
                curAllySelectUI.SetActive(false);
            }

            if (curHpBar != null)
            {
                curEnemySelectUI.SetActive(false);
            }
        }
    }

    public void AllyUnitSelect(Vector3 worldPos, Ingame_UnitCtrl selectedUnit, bool on)
    {
        // Change 상태에서는 모드변경, 업그레이드 불가
        if(selectedUnit.Ally_Mode == AllyMode.Change)
        {
            return;
        }

        if (on)
        {
            if (curUnitOptionBox == null)
            {
                curUnitOptionBox = Instantiate(unitOptionBox, uiCanvas.transform);
            }

            RectTransform rectTransform = curUnitOptionBox.GetComponent<RectTransform>();

            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
            screenPos.x += 100;
            screenPos.y -= 20;

            rectTransform.position = screenPos;

            curUnitOptionBox.SetActive(true);
        }
        else
        {
            curUnitOptionBox.SetActive(false);
        }
        
    }
}
