using InputEventInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class IngameCommandSkillManager : MonoBehaviour, IInputClick, IInputESC, IInputRightClick
{
    [SerializeField] private SelectedUnitManager SelectedUnitManager;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;
    [SerializeField] private InGameManager ingameManager;
    //[SerializeField] private GameObject mouseIndicator;
    [SerializeField] private Transform BurningOilPos;
    [SerializeField] private CommandSkill_FireOilCtrl BurningOilCtrl;
    private Unit selectedTargetUnit;
    //[SerializeField] private Button[] skillButtons;
    [SerializeField] private float skillCastDelayTime;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputEventManager inputEventManager;

    [SerializeField] private Transform selectedUI0;
    [SerializeField] private Transform selectedUI1;
    [SerializeField] private GameObject circle;
    [SerializeField] private LayerMask groundLayer;
    private LayerMask targetUnitLayer;
    private bool isSkillActivated = false;

    private ActiveCommandSkill[] skill;
    private int activatedSkillButtonIdx = 0;

    [SerializeField] private Image[] alarmImages;

    [SerializeField] private AudioClip[] btnClickSFX;

    void Update()
    {
        if (isSkillActivated)
        {
            if (inputEventManager.IsPointerOnUIElements())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
            {
                circle.transform.position = hit.point;
            }
        }
    }
    private void Awake()
    {
        if (GetComponentsInChildren<CommandSkill>() == null)
        {
            Debug.LogError("CommandSkillNullError");
            return;
        }
        skill = GetComponentsInChildren<ActiveCommandSkill>();
    }
    public void ActivateCommandSkill(ActiveCommandSkill skill, Transform pos)
    {
        if (btnClickSFX[activatedSkillButtonIdx] != null)
        {
            SoundManager.Instance.PlaySFX(btnClickSFX[activatedSkillButtonIdx]);
        }

        if (skill.Data.StartSFX != null)
        {
            //SoundManager.Instance.PlaySFX(skill.Data.StartSFX);
        }

        switch (skill.Data.TargetType)
        {
            case CommandSkill.TargetType.NONE:
                skill.Activate();
                break;
            case CommandSkill.TargetType.UNIT:
                skill.Activate(selectedTargetUnit);
                SoundManager.Instance.PlaySFX(skill.Data.StartSFX, selectedTargetUnit.transform.position);
                selectedTargetUnit = null;
                break;
            case CommandSkill.TargetType.MOUSEPOSAREA:
                inputEventManager.OnClickTarget = this;
                skill.Activate(pos);
                SoundManager.Instance.PlaySFX(skill.Data.StartSFX, pos.position);
                break;
            case CommandSkill.TargetType.AREA:
                skill.Activate(BurningOilPos);
                SoundManager.Instance.PlaySFX(skill.Data.StartSFX, BurningOilPos.position);
                Debug.Log(111111);
                BurningOilCtrl.SpawnStart();
                break;
        }
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (skill[activatedSkillButtonIdx].Data.TargetType
                    == CommandSkill.TargetType.UNIT)
            {
                if (inputEventManager.IsPointerOnUIElements())
                    return;
                targetUnitLayer = skill[activatedSkillButtonIdx].AttackTargetLayer;
                if (Physics.Raycast(ray, out hit, float.MaxValue, targetUnitLayer))
                {
                    if (hit.collider.GetComponent<Unit>() != null)
                    {
                        selectedTargetUnit = hit.collider.GetComponent<Unit>();
                        ActivateCommandSkill(skill[activatedSkillButtonIdx], hit.transform);

                        inputEventManager.OnClickTarget = SelectedUnitManager;
                        inputEventManager.OnESCTarget = ingameManager;
                        selectedUI0.gameObject.SetActive(false);
                        selectedUI1.gameObject.SetActive(false);
                        return;
                    }
                }
            }
            else if (skill[activatedSkillButtonIdx].Data.TargetType
                == CommandSkill.TargetType.MOUSEPOSAREA)
            {
                if (Physics.Raycast(ray, out hit,float.MaxValue,groundLayer))
                {
                    if (inputEventManager.IsPointerOnUIElements())
                        return;
                    if (hit.collider.CompareTag(CONSTANT.TAG_TILE))
                    {
                        ActivateCommandSkill(skill[activatedSkillButtonIdx], hit.transform);

                        inputEventManager.OnClickTarget = SelectedUnitManager;
                        inputEventManager.OnESCTarget = ingameManager;
                        selectedUI0.gameObject.SetActive(false);
                        selectedUI1.gameObject.SetActive(false);
                        circle.SetActive(false);
                        isSkillActivated = false;
                    }
                }
            }

            /*if (Physics.Raycast(ray, out hit))
            //{
            //    if (inputEventManager.IsPointerOnUIElements())
            //        return;

            //    if (skill[activatedSkillButtonIdx].Data.TargetType
            //        == CommandSkill.TargetType.UNIT)
            //    {
            //        if (hit.collider.GetComponent<Unit>() != null)
            //        {
            //            selectedTargetUnit = hit.collider.GetComponent<Unit>();
            //            ActivateCommandSkill(skill[activatedSkillButtonIdx], hit.transform);
                        
            //            inputEventManager.OnClickTarget = SelectedUnitManager;
            //            selectedUI0.gameObject.SetActive(false);
            //            selectedUI1.gameObject.SetActive(false);
            //            return;
            //        }
            //    }

            //    if (hit.collider.CompareTag(CONSTANT.TAG_TILE))
            //    {
            //        ActivateCommandSkill(skill[activatedSkillButtonIdx], hit.transform);

            //        inputEventManager.OnClickTarget = SelectedUnitManager;
            //        selectedUI0.gameObject.SetActive(false);
            //        selectedUI1.gameObject.SetActive(false);
            //        circle.SetActive(false);
            //        isSkillActivated = false;
            //    }
            //}
            */

        }
    }

    public void GetClickControl(int idx)
    {
        if (!skill[idx].IsCoolDown)
        {
            Debug.Log(skill[idx].name + "이 쿨타임 중...)");
            SoundManager.Instance.PlayUnableUIClickSFX();
            return;
        }

        CommandSkillData skillData = skill[idx].Data;

        activatedSkillButtonIdx = idx;

        if (skillData.TargetType == CommandSkill.TargetType.MOUSEPOSAREA
            || skillData.TargetType == CommandSkill.TargetType.UNIT)
        {
            allyUnitSpawner.CancelSpawn();
            inputEventManager.OnClickTarget = this;
            inputEventManager.OnESCTarget = this;
            inputEventManager.OnRightClickTarget = this;
            SelectedUnitManager.DeSelecteUnit();

            if (idx >= 0 && idx < alarmImages.Length)
            {
                if (alarmImages[idx] != null)
                {
                    alarmImages[idx].gameObject.SetActive(false);
                }
            }

            if (isSkillActivated && activatedSkillButtonIdx == idx)
            {
                isSkillActivated = false;
                if (idx == 0)
                {
                    selectedUI0.gameObject.SetActive(false);
                    circle.SetActive(false);
                }
                else if (idx == 1)
                {
                    selectedUI1.gameObject.SetActive(false);
                    circle.SetActive(false);
                }
            }
            else
            {
                if (isSkillActivated)
                {
                    if (activatedSkillButtonIdx == 0)
                        selectedUI0.gameObject.SetActive(false);
                    else if (activatedSkillButtonIdx == 1)
                        selectedUI1.gameObject.SetActive(false);
                    circle.SetActive(false);
                }

                isSkillActivated = true;

                if (idx == 0)
                {
                    selectedUI0.gameObject.SetActive(true);
                    selectedUI1.gameObject.SetActive(false);
                    circle.SetActive(false);
                }
                else if (idx == 1)
                {
                    selectedUI0.gameObject.SetActive(false);
                    selectedUI1.gameObject.SetActive(true);
                    circle.SetActive(true);
                }
            }
        }
        else if (skillData.TargetType == CommandSkill.TargetType.AREA)
        {
            if (idx >= 0 && idx < alarmImages.Length)
            {
                if (alarmImages[idx] != null)
                {
                    alarmImages[idx].gameObject.SetActive(false);
                }
            }

            selectedUI0.gameObject.SetActive(false);
            selectedUI1.gameObject.SetActive(false);
            isSkillActivated = false;
            circle.SetActive(false);
            ActivateCommandSkill(skill[idx], BurningOilPos);
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    public void CancelSkill()
    {
        selectedUI0.gameObject.SetActive(false);
        selectedUI1.gameObject.SetActive(false);
        circle.SetActive(false);
        isSkillActivated = false;
        
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CancelSkill();
            inputEventManager.OnESCTarget = ingameManager;
            inputEventManager.OnRightClickTarget = SelectedUnitManager;
            inputEventManager.OnClickTarget = SelectedUnitManager;
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CancelSkill();
            inputEventManager.OnRightClickTarget = SelectedUnitManager;
            inputEventManager.OnESCTarget = ingameManager;
            inputEventManager.OnClickTarget = SelectedUnitManager;
        }
    }
}
