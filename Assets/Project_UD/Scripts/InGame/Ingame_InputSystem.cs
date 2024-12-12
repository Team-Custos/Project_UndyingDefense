using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//이 스크립트는 인게임 조작을 관리하기 위한 스크립트입니다.

public class Ingame_InputSystem : MonoBehaviour
{
    public static Ingame_InputSystem Instance { get; private set; }
    public UnitSpawnManager spawnManager;

    public float AxisX = 0;
    public float AxisY = 0;

    public bool IsPressedPrimaryButton = false;
    public bool IsPressedSecondaryButton = false;
    public bool IsPressingSecondaryButton = false;
    public bool IsPressedWheelButton = false;

    public bool IsWheelScrollUp = false;
    public bool IsWheelScrollDown = false;

    public System.Action OnPrimaryPerformed;
    public System.Action OnSecondaryPerformed;
    public System.Action OnWheelButtonPerformed;


    private void Awake()
    {
        Instance = this;



    }

    // Update is called once per frame
    private void Update()
    {
        _inputInit();
        HotKey();

    }

    void HotKey()
    {
        InGameManager.inst.FasterTimeScale =
            Input.GetKeyDown(KeyCode.E) || Input.GetKey(KeyCode.E);


        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject selectedUnit = GameOrderSystem.instance.selectedUnit;
            Transform CameraPivot = InGameManager.inst.camManager.cameraPivot;

            if (selectedUnit != null)
            {
                CameraPivot.position =
                    new Vector3(selectedUnit.transform.position.x, CameraPivot.position.y, selectedUnit.transform.position.z);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (!Ingame_UIManager.instance.unitSpawnBtn[0].interactable)
            {
                return;
            }

            ToggleUnitSpawnState(0); // 1번 단축키
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (!Ingame_UIManager.instance.unitSpawnBtn[1].interactable)
            {
                return;
            }

            ToggleUnitSpawnState(1); // 2번 단축키
        }
    }


    void _inputInit()
    {
        IsPressedPrimaryButton = Input.GetMouseButtonDown(0);
        if (IsPressedPrimaryButton)
        {
            OnPrimaryPerformed?.Invoke();
        }

        IsPressedSecondaryButton = Input.GetMouseButtonDown(1);
        if (IsPressedSecondaryButton)
        {
            OnSecondaryPerformed?.Invoke();
        }
        IsPressingSecondaryButton = Input.GetMouseButton(1);

        IsPressedWheelButton = Input.GetMouseButtonDown(2);
        if (IsPressedWheelButton)
        {
            OnWheelButtonPerformed?.Invoke();
        }

        IsWheelScrollUp = Input.mouseScrollDelta.y > 0;
        IsWheelScrollDown = Input.mouseScrollDelta.y < 0;

        AxisX = Input.GetAxisRaw("Horizontal");
        AxisY = Input.GetAxisRaw("Vertical");
    }

    // 소환 상태를 설정하는 메서드
    void ToggleUnitSpawnState(int unitIndex)
    {
        // 같은 유닛이 이미 활성화되어 있는 경우: 소환 상태 해제
        if (spawnManager.unitToSpawn == unitIndex && InGameManager.inst.UnitSetMode && InGameManager.inst.AllyUnitSetMode)
        {
            InGameManager.inst.UnitSetMode = false;
            InGameManager.inst.AllyUnitSetMode = false;

            // 버튼 효과 초기화
            Ingame_UIManager.instance.UpdateButtonEffect(-1); // 모든 버튼 비활성화
        }
        else
        {
            // 새로운 유닛 소환 상태 활성화
            InGameManager.inst.UnitSetMode = true;
            InGameManager.inst.AllyUnitSetMode = true;

            // 선택한 유닛 설정
            spawnManager.unitToSpawn = unitIndex;

            // 버튼 효과 갱신
            Ingame_UIManager.instance.UpdateButtonEffect(unitIndex);
        }
    }
}
