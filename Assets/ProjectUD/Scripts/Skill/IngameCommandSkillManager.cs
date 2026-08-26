using InputEventInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class IngameCommandSkillManager : MonoBehaviour, IInputClick, IInputFunction
{
    //------지휘관 스킬 로드 & 셋팅
    [SerializeField] private CommandSkillRepository cSkillRepository;
    [SerializeField] private CommandSkill[] commandSkillList;       // 총 commandSkill 배열
    [SerializeField] private SkillButtonCooldownUI[] cSkillBtns;

    [Header("인디케이터")]
    [SerializeField] private GameObject indicator;

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
        inputEventManager.OnFunctionTarget = this;
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
    public void SetBeingUsedCommandSkill(int i) //, CommandSkill commandSkill)
    {
        //if (commandSkill == null)
        //    return;
        //beingUsedCommandskill = commandSkill;
        // 인디케이터

        //Debug.Log(currentSelected[i].Name);

        if (currentSelected[i] == null)
        {
            Debug.LogError("Selected CommandSkill is null!");
        }

        if (currentSelected[i].TargetType == CommandSkill.TargetType.NONE)
            return;

        if (indicator == null)
        {
            Debug.LogError("Indicator is NOT assigned!");
            return;
        }
        indicator.transform.position = cSkillBtns[i].transform.position;
        indicator.SetActive(true);


        //inputEventManager.OnESCTarget = this;
        //inputEventManager.OnRightClickTarget = this;
        //selectedUI0 = cSkillBtns[i].transform;
        //selectedUI0.gameObject.SetActive(true);
    }
    public void ResetButton()
    {
        //beingUsedCommandskill = null;
        // 인디케이터
        
        if (indicator == null)
        {
            Debug.LogError("Indicator is NOT assigned!");
            return;
        }
        //indicator.SetActive(false);


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
        //if (beingUsedCommandskill == null)
        //    return;
        //beingUsedCommandskill.SetSkillState(false);
        //beingUsedCommandskill = null;
        // 인디케이터
        indicator.SetActive(false);

    }


    public void OnFunction(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            string keyName = context.control.name;

            int skillIndex = -1;

            switch(keyName)
            {
                case "f1":
                    skillIndex = 0;
                    break;

                case "f2":
                    skillIndex = 1;
                    break;

                case "f3":
                    skillIndex = 2;
                    break;

                default:
                    return;
            }

            if (skillIndex == -1)
                return;

            cSkillBtns[skillIndex].OnButtonClick();
        }
    }
}
