using InputEventInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class IngameCommandSkillManager : MonoBehaviour, IInputClick, IInputESC, IInputRightClick
{
    //------지휘관 스킬 로드 & 셋팅
    [SerializeField] private CommandSkillRepository cSkillRepository;
    [SerializeField] private CommandSkill[] commandSkillList;
    [SerializeField] private SkillButtonCooldownUI[] cSkillBtns;

    [Header("인디케이터")]
    //[SerializeField] private GameObject indicator;

    private CommandSkillData[] datas = new CommandSkillData[] { };
    private CommandSkillData[] currentSelected = new CommandSkillData[3];
    //-------지휘관 스킬 로드 & 셋팅

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
    //private bool isSkillActivated = false;

    //private ActiveCommandSkill[] skill;
    private ActiveCommandSkill activeSkill;
    private int activatedSkillButtonIdx = 0;

    [SerializeField] private Image[] alarmImages;

    [SerializeField] private AudioClip[] btnClickSFX;

    //ayo_0117
    private CommandSkill beingUsedCommandskill;

    private void Start()
    {
        LoadCSkillData();
        
    }

    void Update()
    {
        //if (isSkillActivated)
        //{
        //    if (inputEventManager.IsPointerOnUIElements())
        //        return;

        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        //    {
        //        circle.transform.position = hit.point;
        //    }
        //}
    }
    private void Awake()
    {
        if (GetComponentsInChildren<CommandSkill>() == null)
        {
            Debug.LogError("CommandSkillNullError");
            return;
        }
        //skill = GetComponentsInChildren<ActiveCommandSkill>();
    }

    // 지휘관스킬 로드
    private void LoadCSkillData()
    {
        cSkillRepository.SetCommanderSkill();
        datas = cSkillRepository.GetCommandSkills();
        Debug.Log($"지휘관스킬_리소스에서 로드 성공한 스킬 갯수 : {datas.Length}");

        List<string> selectedSkillList = PlayerPrefsData.instance.GetSelectedCommanderSkill();
        for (int i = 0; i < selectedSkillList.Count; i++)
        {
            Debug.Log($"지휘관스킬_프랩스에서 로드 성공한 스킬 갯수 : {selectedSkillList.Count}");

            for (int j = 0; j < datas.Length; j++)
            {
                if (string.Compare(datas[j].Id, selectedSkillList[i]) == 0)
                {
                    //Debug.Log("일치");
                    currentSelected[i] = datas[j];
                    //currentSelected.Add(datas[j]);
                    if (currentSelected[i] != null)
                    {
                        Debug.Log("지휘관스킬로드 성공");
                    }
                }
            }
        }

        SetCSkillDataList();
        FindCommandSkill();
    }

    private void SetCSkillDataList()
    {
        for (int i = 0; i < currentSelected.Length; i++)
        {
            SetCSkillData(i);
        }
    }

    private void SetCSkillData(int index)
    {
        if (currentSelected[index] == null)
        {
            cSkillBtns[index].SetSelectedCSkillDataUI(index, null,string.Empty,string.Empty,string.Empty);
            return;
        }
        string skillNameId = currentSelected[index].Id + "_name";
        string skillDescId = currentSelected[index].Id + "_desc";
        string skillEffectId = currentSelected[index].Id + "_effect";

        cSkillBtns[index].SetSelectedCSkillDataUI(index, currentSelected[index],
            LocalizationSettings.StringDatabase.
            GetLocalizedString("CommanderSkill", $"{skillNameId}", LocalizationSettings.SelectedLocale),
            LocalizationSettings.StringDatabase.
            GetLocalizedString("CommanderSkill", $"{skillDescId}", LocalizationSettings.SelectedLocale),
            LocalizationSettings.StringDatabase.
            GetLocalizedString("CommanderSkill", $"{skillEffectId}", LocalizationSettings.SelectedLocale));
    }

    private void FindCommandSkill()
    {
        for (int i = 0; i < currentSelected.Length; i++)
        {
            for (int j = 0; j < commandSkillList.Length; j++)
            {
                if (currentSelected[i] == commandSkillList[j].Data)
                {
                    cSkillBtns[i].SetCommandSkill(commandSkillList[j]);
                    Debug.Log($"지휘관스킬 매칭 성공 : {commandSkillList[j].Data.Name}");

                }
            }
        }
    }

    public void ActivateCommandSkill(ActiveCommandSkill skill, Transform pos)
    {
        /*
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
                //SoundManager.Instance.PlaySFX(skill.Data.StartSFX, selectedTargetUnit.transform.position);
                selectedTargetUnit = null;
                break;
            case CommandSkill.TargetType.MOUSEPOSAREA:
                inputEventManager.OnClickTarget = this;
                //skill.Activate(pos);
                //SoundManager.Instance.PlaySFX(skill.Data.StartSFX, pos.position);
                break;
            case CommandSkill.TargetType.AREA:
                //skill.Activate(BurningOilPos);
                //SoundManager.Instance.PlaySFX(skill.Data.StartSFX, BurningOilPos.position);
                //BurningOilCtrl.SpawnStart();
                break;
        }
        */
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        //if (context.performed)
        //{
        //    Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        //    RaycastHit hit;

        //    if (activeSkill.Data.TargetType            //skill[activatedSkillButtonIdx].Data.TargetType
        //            == CommandSkill.TargetType.UNIT)
        //    {
        //        if (inputEventManager.IsPointerOnUIElements())
        //            return;
        //        targetUnitLayer = activeSkill.AttackTargetLayer;                  //skill[activatedSkillButtonIdx].AttackTargetLayer;
        //        if (Physics.Raycast(ray, out hit, float.MaxValue, targetUnitLayer))
        //        {
        //            if (hit.collider.GetComponent<Unit>() != null)
        //            {
        //                selectedTargetUnit = hit.collider.GetComponent<Unit>();
        //                ActivateCommandSkill(activeSkill, hit.transform);          //skill[activatedSkillButtonIdx], hit.transform);

        //                inputEventManager.OnClickTarget = SelectedUnitManager;
        //                inputEventManager.OnESCTarget = ingameManager;
        //                selectedUI0.gameObject.SetActive(false);    // 인디케이터
        //                selectedUI1.gameObject.SetActive(false);
        //                return;
        //            }
        //        }
        //    }
        //    else if (activeSkill.Data.TargetType                           //skill[activatedSkillButtonIdx].Data.TargetType
        //        == CommandSkill.TargetType.MOUSEPOSAREA)
        //    {
        //        if (Physics.Raycast(ray, out hit,float.MaxValue,groundLayer))
        //        {
        //            if (inputEventManager.IsPointerOnUIElements())
        //                return;
        //            if (hit.collider.CompareTag(CONSTANT.TAG_TILE))
        //            {
        //                ActivateCommandSkill(activeSkill, hit.transform);               //skill[activatedSkillButtonIdx], hit.transform);

        //                inputEventManager.OnClickTarget = SelectedUnitManager;
        //                inputEventManager.OnESCTarget = ingameManager;
        //                selectedUI0.gameObject.SetActive(false);
        //                selectedUI1.gameObject.SetActive(false);
        //                circle.SetActive(false);
        //                isSkillActivated = false;
        //            }
        //        }
        //    }

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

        //}
    }

    public void GetClickControl(int idx, CommandSkill commandSkill)
    {
        /*
        activeSkill = commandSkill as ActiveCommandSkill;
        Debug.Log($"액티브 스킬 셋팅 완료 {activeSkill.name}");

        //if (!activeSkill.IsCoolDown)                                 //!skill[idx].IsCoolDown)
        //{
        //    Debug.Log(activeSkill.name + "이 쿨타임 중...");         //skill[idx].name + "이 쿨타임 중...)");
        //    SoundManager.Instance.PlayUnableUIClickSFX();
        //    return;
        //}

        CommandSkillData skillData = activeSkill.Data;              //skill[idx]

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
            ActivateCommandSkill(activeSkill, BurningOilPos);                  //skill[idx], BurningOilPos);
            //SoundManager.Instance.PlayUIClickSFX();
        }
        */
    }

    public void SetBeingUsedCommandSkill(int i, CommandSkill commandSkill)
    {
        beingUsedCommandskill = commandSkill;
        // 인디케이터
        /*
        //if (indicator == null)
        //{
        //    Debug.LogError("Indicator is NOT assigned!");
        //    return;
        //}
        //indicator.transform.position = cSkillBtns[i].transform.position;
        //indicator.SetActive(true);*/


        //inputEventManager.OnESCTarget = this;
        //inputEventManager.OnRightClickTarget = this;
        //selectedUI0 = cSkillBtns[i].transform;
        //selectedUI0.gameObject.SetActive(true);
    }
    public void ResetButton()
    {
        beingUsedCommandskill = null;
        // 인디케이터
        /*
        if (indicator == null)
        {
            Debug.LogError("Indicator is NOT assigned!");
            return;
        }
        indicator.SetActive(false);*/


        //inputEventManager.OnESCTarget = ingameManager;
        //inputEventManager.OnRightClickTarget = SelectedUnitManager;
        //inputEventManager.OnClickTarget = SelectedUnitManager;
        //selectedUI0.gameObject.SetActive(false);
    }
    public void CancelSkill()
    {
        //selectedUI0.gameObject.SetActive(false);
        //selectedUI1.gameObject.SetActive(false);
        //circle.SetActive(false);
        //isSkillActivated = false;
        //ayo_0117
        if (beingUsedCommandskill == null)
            return;
        beingUsedCommandskill.SetSkillState(false);
        beingUsedCommandskill = null;
        // 인디케이터
        //indicator.SetActive(false);

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
