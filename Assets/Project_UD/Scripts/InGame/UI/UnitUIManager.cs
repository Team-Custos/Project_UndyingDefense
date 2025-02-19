using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnitUIManager : MonoBehaviour
{
    [Header("UI Prefabs")]
    public GameObject hpBarPrefab;       // HP 바 프리팹
    public GameObject selectedUIPrefab;  // 선택 UI 프리팹
    public Image hpBarFill;              // HP 바 채우기 이미지

    [Header("UI Settings")]
    [SerializeField] private Canvas uiCanvas;    // World Space Canvas
    [SerializeField] private Camera mainCamera;  // 메인 카메라
    [SerializeField] private float hpBarYOffset;            // HP 바 Y축 오프셋
    [SerializeField] private float selectedUIYOffset;     // 선택 UI Y축 오프셋

    // 현재 선택된 유닛과 연결된 UI
    private GameObject curHpBar;
    private GameObject curSelectedUI;
    private Transform selectedUnitTr;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (selectedUnitTr != null)
        {
            // HP 바가 유닛을 따라가게
            curHpBar.transform.position = selectedUnitTr.position + new Vector3(0, hpBarYOffset, 0);
        }

        UpdateHPBar();
    }

    public void OnOffUnitUI(Transform unitTransform, bool on)
    {
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

            // 선택 ui가 없으면 생성
            if (curSelectedUI == null)
            {
                curSelectedUI = Instantiate(selectedUIPrefab, transform);
            }

            // 선택 UI를 선택된 유닛의 자식으로 설정
            curSelectedUI.transform.SetParent(selectedUnitTr);
            curSelectedUI.transform.localPosition = new Vector3(0, selectedUIYOffset, 0);
            curSelectedUI.transform.localRotation = Quaternion.identity;
            curSelectedUI.SetActive(true);
        }
        else
        {
            if (curHpBar != null)
            {
                curHpBar.SetActive(false);
            }

            if (curSelectedUI != null)
            {
                curSelectedUI.SetActive(false);
            }

            selectedUnitTr = null;
        }
    }

    // HP 바 업데이트
    private void UpdateHPBar()
    {
        if (selectedUnitTr != null)
        {
            Ingame_UnitCtrl unit = selectedUnitTr.GetComponent<Ingame_UnitCtrl>();
            if (unit != null && hpBarFill != null)
            {
                // 체력 비율 계산 (0 ~ 1)
                float healthRatio = unit.HP / unit.unitData.maxHP;
                hpBarFill.fillAmount = healthRatio; // HP 바 업데이트
            }
        }
    }
}
