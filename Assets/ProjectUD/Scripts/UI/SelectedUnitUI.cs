using Cinemachine;
using InputEventInterface;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SelectedUnitUI : MonoBehaviour, IInputESC, IInputRightClick
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SelectedUnitManager selecteUnitManger;
    [SerializeField] private UpgradeMenuUI upgradeMenuUI;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Ingame_CamManager ingameCamManager;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private UnitDataLoader unitDataLoader;
    private CinemachineFramingTransposer framingTransposer;


    [SerializeField] private Button upgradeBtn;
    [SerializeField] private Image unitHP;
    [SerializeField] private Image unitDuration;
    [SerializeField] private RectTransform hpRectTransform;

    [SerializeField] private GameObject unitMenuPrefab;
    [SerializeField] private GameObject unitHPPrefab;
    [SerializeField] private GameObject unitDurationPrefab;
    [SerializeField] private GameObject unitUpgradeMenuPrefab;

    [SerializeField] private Image modeChangeBtnImage;
    [SerializeField] private Sprite freeIcon;
    [SerializeField] private Sprite siegeIcon;
    [SerializeField] private float yPos;
    [SerializeField] private float xPos;
    [SerializeField] private float upgradeXpos;
    [SerializeField] private float menuYpos;

    [Header("NameTextTable")]
    [SerializeField] private FactionNameTextTable fNameTextTable;

    [Header("■ UntiInfo")]
    [SerializeField] private Image unitInfoImage;
    [SerializeField] private Image unitImage;
    [SerializeField] private TextMeshProUGUI unitNameText;
    [SerializeField] private TextMeshProUGUI unitHPText;
    [SerializeField] private Image unitHPImage;
    [SerializeField] private TextMeshProUGUI unitMentalText;
    //[SerializeField] private Image unitMentalImage;  나중에 작업
    [SerializeField] private Image atTypeIcon;
    [SerializeField] private Image dfTypeIcon;
    [SerializeField] private Image unitSSkillImage;
    [SerializeField] private Image unitGSkillImage;
    [SerializeField] private Image specialAbilityImage;
    [SerializeField] private TextMeshProUGUI dfTypeText;
    [SerializeField] private TextMeshProUGUI critText;
    [SerializeField] private TextMeshProUGUI moveSpeedText;
    [SerializeField] private TextMeshProUGUI atSpeedText;
    [SerializeField] private TextMeshProUGUI atRangeText;
    [SerializeField] private TextMeshProUGUI mentalText;
    [SerializeField] private Image[] tierImage;

    [SerializeField] private Image[] unitStateImage;
    [SerializeField] private StatusUI[] unitStateUIs;
    [SerializeField] private GameObject unitStatePanel;
    [SerializeField] private GameObject typeInfo;

    [SerializeField] private GameObject allyUnitUI;
    [SerializeField] private GameObject enemyUnitUI;

    [Header("======= SkillInfo =======")]

    // 기본 스킬
    [SerializeField] private TextMeshProUGUI gSkillNameText;
    [SerializeField] private TextMeshProUGUI gSkillInfoText;
    [SerializeField] private TextMeshProUGUI gSkillEffectText;
    [SerializeField] private TextMeshProUGUI gSkillEtcText;
    
    // 특수 스킬
    [SerializeField] private TextMeshProUGUI sSkilNameText;
    [SerializeField] private TextMeshProUGUI sSkillInfoText;
    [SerializeField] private TextMeshProUGUI sSkillEffectText;
    [SerializeField] private TextMeshProUGUI sSkillEtcText;

    // 특수 능력
    [SerializeField] private TextMeshProUGUI sAbilityNameText;
    [SerializeField] private TextMeshProUGUI sAbilityInfoText;
    [SerializeField] private TextMeshProUGUI sAbilityEffectText;


    private void Start()
    {
        if (virtualCamera != null)
        {
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (selecteUnitManger.SelectedUnit != null)
        {
            UpdateUI();

            if (selecteUnitManger.SelectedUnit.HpPercent <= 0)
                return;

            if (selecteUnitManger.SelectedUnit is AllyUnit)
            {
                allyUnitUI.SetActive(true);
                allyUnitUI.transform.position = selecteUnitManger.SelectedUnit.transform.position;
                enemyUnitUI.SetActive(false);
            }
            else if (selecteUnitManger.SelectedUnit is EnemyUnit)
            {
                enemyUnitUI.SetActive(true);
                enemyUnitUI.transform.position = selecteUnitManger.SelectedUnit.transform.position;
                allyUnitUI.SetActive(false);
            }
        }
    }

    public void ShowHp(Unit unit)
    {
        HideUnitDuration();
        unit = selecteUnitManger.SelectedUnit; 
        unitHPPrefab.SetActive(true);
        
    }

    public void HideHp()
    {
        allyUnitUI.SetActive(false);
        enemyUnitUI.SetActive(false);

        if (unitHPPrefab != null)
        {
            unitHPPrefab.SetActive(false);
            unitInfoImage.gameObject.SetActive(false);
            typeInfo.SetActive(false);
        }

    }

    public void HideUpgrdeUI()
    {
        upgradeMenuUI.HideUpgradeUI();
        //SoundManager.Instance.playCancleSFX();
        selecteUnitManger.OnUpgrade(false);

        if(selecteUnitManger.SelectedUnit is AllyUnit)
            ShowAllyUI((AllyUnit)selecteUnitManger.SelectedUnit);

        inputEventManager.OnESCTarget = selecteUnitManger;
        inputEventManager.OnRightClickTarget = selecteUnitManger;
    }

    public void OffUpgradeUI()
    {
        unitUpgradeMenuPrefab.SetActive(false);
        //SoundManager.Instance.playCancleSFX();
        selecteUnitManger.OnUpgrade(false);
    }

    public void ShowUpgradeMenu(Unit unit)
    {
        if (unit.Data.Tier >= 4)
            return;

        if (!inGameManager.IsGameStart)
            return;

        SoundManager.Instance.PlayUIClickSFX();
        unitMenuPrefab.SetActive(false);
        unitUpgradeMenuPrefab.SetActive(true);
        inputEventManager.OnRightClickTarget = this;
        inputEventManager.OnESCTarget = this;
        upgradeMenuUI.SetUnitUpgradeMenu(unit);
        selecteUnitManger.OnUpgrade(true);
    }

    public void ShowAllyUI(AllyUnit allyUnit)
    {
        if(selecteUnitManger.SelectedUnit == null)
            return;

        allyUnit = (AllyUnit)selecteUnitManger.SelectedUnit;
        

        if(allyUnit.ModeType == AllyUnit.Mode.FREE)
        {
            modeChangeBtnImage.sprite = siegeIcon;
        }
        else if(allyUnit.ModeType == AllyUnit.Mode.SEIGE)
        {
            modeChangeBtnImage.sprite = freeIcon;
        }

        if (allyUnit.Data.Tier >= 4)
            upgradeBtn.interactable = false;
        else
            upgradeBtn.interactable = true;

        unitMenuPrefab.SetActive(true);

        

        //unitMenuUI.PerformModeChange((AllyUnit)selectedUnit);


        //unitMenuUI.PerformUpgrade((AllyUnit)selectedUnit, allyUnitData, upgradeOption);
    }

    public void HideAllyUI()
    {
        unitMenuPrefab.SetActive(false);
        SoundManager.Instance.PlayUIClickSFX();
        //selectedUnit = null;
        selecteUnitManger.OnUpgrade(false);
    }

    private void UpdateUI()
    {
        if(selecteUnitManger.SelectedUnit != null)
        {
            if (unitHPPrefab != null)
            {
                unitHP.fillAmount = selecteUnitManger.SelectedUnit.HpPercent;

                //Vector3 worldPosition = selecteUnitManger.SelectedUnit.HeightPos.position;

                //// 월드 좌표로 HP UI 위치 고정
                //unitHPPrefab.transform.position = worldPosition;

                //// UI가 항상 카메라를 바라보도록
                //unitHPPrefab.transform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

                //// 카메라 거리 기반 크기 보정 (줌인/줌아웃에도 안정적인 크기)
                //float dist = Vector3.Distance(mainCamera.transform.position, worldPosition);
                //unitHPPrefab.transform.localScale = Vector3.one * dist * 0.0015f;

                Vector3 worldPosition = selecteUnitManger.SelectedUnit.transform.position + Vector3.up * selecteUnitManger.SelectedUnit.HeightPos.position.y;
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

                unitHPPrefab.transform.position = screenPosition;
            }

            if (unitMenuPrefab != null)
            {
                float currentFov = framingTransposer.m_CameraDistance;

                float maxZoom = ingameCamManager.ZoomMax;
                float minZoom = ingameCamManager.ZoomMin;



                float zoomPercnet = (currentFov - minZoom) / (maxZoom - minZoom);

                menuYpos = Mathf.Lerp(2.0f, 3.0f, zoomPercnet);

                Vector3 worldPosition = selecteUnitManger.SelectedUnit.transform.position + Vector3.right *xPos + Vector3.down * (zoomPercnet * menuYpos);
                Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

                unitMenuPrefab.transform.position = screenPosition;

            }


          //  if (unitUpgradeMenuPrefab != null)
           // {

                //Vector3 worldPosition = selecteUnitManger.SelectedUnit.transform.position + Vector3.right * upgradeXpos;
                //Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

                //unitUpgradeMenuPrefab.transform.position = screenPosition;
                // 1️⃣ 유닛의 월드 좌표 가져오기
                //Vector3 worldPosition = selecteUnitManger.SelectedUnit.transform.position;

                //// 2️⃣ 월드 좌표 → 화면 좌표로 변환
                //Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

                //// 3️⃣ 오른쪽으로 픽셀 이동 (upgradeXpos 사용)
                //screenPosition.x += upgradeXpos;

                //// 4️⃣ 화면 경계에서 벗어나지 않도록 클램핑
                //float marginX = 300f;
                //float marginY = 300f;
                //float clampedX = Mathf.Clamp(screenPosition.x, marginX, Screen.width - marginX);
                //float clampedY = Mathf.Clamp(screenPosition.y, marginY, Screen.height - marginY);

                //// 5️⃣ 버튼 위치 설정
                //unitUpgradeMenuPrefab.transform.position = new Vector3(clampedX, clampedY, screenPosition.z);
            //}
        }
    }

    public void UpdateUnitInfoByBtn(UnitData unitData, UnitDataLoader unitDataLoader)
    {
        Unit unit = unitData.Prefab.GetComponent<Unit>();

        UnitStats unitStats = unitDataLoader.GetUnitDataById(unit.UnitId);

        unitInfoImage.gameObject.SetActive(true);


        unitImage.sprite = unitData.Icon;

        //unitNameText.text = unitStats.unitName;
        //-- Localization --
        unitNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{unitStats.id}_name", LocalizationSettings.SelectedLocale);
        unitMentalText.text =  "멘탈 : " + unitStats.mental .ToString();
        unitHPImage.fillAmount = unitStats.maxHp / unitStats.maxHp;
        unitHPText.text = $"{unitStats.maxHp} / {unitStats.maxHp}";

        SetUnitTierIcon(unitData.Tier);

        AttackSkillData attackSkillData = unit.GeneralSkill.Data as AttackSkillData;

        atTypeIcon.sprite = attackSkillData.Info.TypeIcon;
        dfTypeIcon.sprite = unitData.DfTypeIcon;

        dfTypeText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", $"CON_defense{unitData.ArmorType}", LocalizationSettings.SelectedLocale);

        //attackTypeText.text = unitData.AttackType;

        //unitGSkillText.text = unit.GeneralSkill.Data.name;

        UpdateSkillInfo(unitData);

        /*

        unitGSkillImage.sprite = unit.GeneralSkill.Data.Icon;

        //gSkillNameText.text = unit.GeneralSkill.Data.Name;
        //gSkillInfoText.text = unit.GeneralSkill.Data.Description;

        gSkillNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.GeneralSkill.Data.Name}_name", LocalizationSettings.SelectedLocale);
        gSkillInfoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.GeneralSkill.Data.Name}_desc", LocalizationSettings.SelectedLocale);
        //infoGSkillEffect.text = LocalizationSettings.StringDatabase.
        //GetLocalizedString("UnitSkill", $"{unit.GeneralSkill.Data.Name}_effect", LocalizationSettings.SelectedLocale);

        if (unit.SpecialSkill != null)
        {
            unitSSkillImage.sprite = unit.SpecialSkill.Data.Icon;
            //sSkilNameText.text = unit.SpecialSkill.Data.Name;
            //sSkillInfoText.text = unit.SpecialSkill.Data.Description;

            sSkilNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_name", LocalizationSettings.SelectedLocale);
            sSkillInfoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_desc", LocalizationSettings.SelectedLocale);
            //sSkillInfoText.text = LocalizationSettings.StringDatabase.
            //    GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_effect", LocalizationSettings.SelectedLocale);

            unitSSkillImage.gameObject.SetActive(true);

        }
        else
            unitSSkillImage.gameObject.SetActive(false);


        if (unit.SpecialAbility != null)
        {
            specialAbilityImage.sprite = unit.SpecialAbility.Icon;
            //sSkilNameText.text = unit.PassiveSkill.Data.Name;
            //sSkillInfoText.text = unit.PassiveSkill.Data.Description;

            sAbilityNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("SpecialAbility", $"{unit.SpecialAbility.Id}_name", LocalizationSettings.SelectedLocale);
            sAbilityInfoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("SpecialAbility", $"{unit.SpecialAbility.Id}_desc", LocalizationSettings.SelectedLocale);
            //sSkillInfoText.text = LocalizationSettings.StringDatabase.
            //    GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_effect", LocalizationSettings.SelectedLocale);

            specialAbilityImage.gameObject.SetActive(true);
        }
        else
            specialAbilityImage.gameObject.SetActive(false);

        */

        FieldLocalization(unitStats);

        //critText.text = "치명타율 : " + unitStats.critChance.ToString() + "%";
        //moveSpeedText.text = "이동속도 : " + unitStats.moveSpeed.ToString();
        //atSpeedText.text = "공격속도 : " + unitStats.attackSpeed;
        //atRangeText.text = "공격거리 : " + (unitStats.attackRange / 2).ToString() + "칸";
        //mentalText.text = "멘탈 : " + unitStats.mental.ToString();

        // 상태 이미지 끄기
        for(int i = 0; i < unitStateImage.Length; i++)
        {
            unitStateImage[i].gameObject.SetActive(false);
        }
    }

    string pointColor = "#000000";

    private void FieldLocalization(UnitStats unitStats)     //260408 로폴 수정
    {
        string mentalT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_mental", LocalizationSettings.SelectedLocale);
        mentalText.text = $"{mentalT} <color={pointColor}>{unitStats.mental}</color>";

        string critT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_critChance", LocalizationSettings.SelectedLocale);
        critText.text = $"{critT} <color={pointColor}>{unitStats.critChance}%</color>";

        string moveSpeedT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_moveSpeed", LocalizationSettings.SelectedLocale);
        moveSpeedText.text = $"{moveSpeedT} <color={pointColor}>{unitStats.moveSpeed}</color>";

        string attackSText = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_attackSpeed", LocalizationSettings.SelectedLocale);
        string atSpeedId = fNameTextTable.GetName("Interval_" + unitStats.interval.ToString());
        string atSpeedValue = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", atSpeedId, LocalizationSettings.SelectedLocale);
        atSpeedText.text = $"{attackSText} <color={pointColor}>{atSpeedValue}</color>";

        string attackRangeT = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_attackRange", LocalizationSettings.SelectedLocale);
        atRangeText.text = $"{attackRangeT} <color={pointColor}>{unitStats.attackRange / 2}보</color>";
    }

    public void UpdateUnitInfo(Unit unit)
    {
       unit.SetSelectedUnitUI(this);

       unitInfoImage.gameObject.SetActive(true);

        if(unit.UnitStats == null)
        {
            Debug.Log("데이터 없음");
            return;
        }

       UpdateHPUI(unit);
       unitImage.sprite = unit.Data.Icon;
        //unitNameText.text = unit.UnitStats.unitName;
        //--Localization
        unitNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitStringData(Name, Description)", $"{unit.UnitStats.id}_name", LocalizationSettings.SelectedLocale);
        unitMentalText.text = "멘탈 : " + unit.UnitStats.mental.ToString();

        SetUnitTierIcon(unit.Data.Tier);

        AttackSkillData attackSkillData = unit.GeneralSkill.Data as AttackSkillData;

        atTypeIcon.sprite = attackSkillData.Info.TypeIcon;
        dfTypeIcon.sprite = unit.Data.DfTypeIcon;

        dfTypeText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", $"CON_defense{unit.Data.ArmorType}", LocalizationSettings.SelectedLocale);

        //attackTypeText.text = unit.Data.AttackType;

        //unitGSkillText.text = unit.GeneralSkill.Data.name;

        UpdateSkillInfo(unit.Data);

        /*

        unitGSkillImage.sprite = unit.GeneralSkill.Data.Icon;
        //gSkillNameText.text = unit.GeneralSkill.Data.Name;
        //gSkillInfoText.text = unit.GeneralSkill.Data.Description;

        gSkillNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.GeneralSkill.Data.Name}_name", LocalizationSettings.SelectedLocale);
        gSkillInfoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.GeneralSkill.Data.Name}_desc", LocalizationSettings.SelectedLocale);
        //infoGSkillEffect.text = LocalizationSettings.StringDatabase.
        //GetLocalizedString("UnitSkill", $"{unit.GeneralSkill.Data.Name}_effect", LocalizationSettings.SelectedLocale);

        if (unit.SpecialSkill != null)
        {
            unitSSkillImage.sprite = unit.SpecialSkill.Data.Icon;

            //sSkilNameText.text = unit.SpecialSkill.Data.Name;
            //sSkillInfoText.text = unit.SpecialSkill.Data.Description;

            sSkilNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_name", LocalizationSettings.SelectedLocale);
            sSkillInfoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_desc", LocalizationSettings.SelectedLocale);
            //sSkillInfoText.text = LocalizationSettings.StringDatabase.
            //    GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_effect", LocalizationSettings.SelectedLocale);

            unitSSkillImage.gameObject.SetActive(true);
        }
        else
            unitSSkillImage.gameObject.SetActive(false);

        if (unit.SpecialAbility != null)
        {
            specialAbilityImage.sprite = unit.SpecialAbility.Icon;

            sAbilityNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("SpecialAbility", $"{unit.SpecialAbility.Id}_name", LocalizationSettings.SelectedLocale);
            sAbilityInfoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("SpecialAbility", $"{unit.SpecialAbility.Id}_desc", LocalizationSettings.SelectedLocale);

            specialAbilityImage.gameObject.SetActive(true);

            //sSkilNameText.text = unit.PassiveSkill.Data.Name;
            //sSkillInfoText.text = unit.PassiveSkill.Data.Description;

            //Special.text = LocalizationSettings.StringDatabase.
            //GetLocalizedString("UnitSkill", $"{unit.PassiveSkill.Data.Name}_name", LocalizationSettings.SelectedLocale);
            //sSkillInfoText.text = LocalizationSettings.StringDatabase.
            //GetLocalizedString("UnitSkill", $"{unit.PassiveSkill.Data.Name}_desc", LocalizationSettings.SelectedLocale);
            //sSkillInfoText.text = LocalizationSettings.StringDatabase.
            //    GetLocalizedString("UnitSkill", $"{unit.SpecialSkill.Data.Name}_effect", LocalizationSettings.SelectedLocale);
        }
        else
            specialAbilityImage.gameObject.SetActive(false);

        */

        FieldLocalization(unit.UnitStats);

        //critText.text = "치명타율 : " + unit.UnitStats.critChance.ToString() + "%";
        //moveSpeedText.text = "이동속도 : " + unit.UnitStats.moveSpeed.ToString();
        //atSpeedText.text = "공격속도 : " + unit.UnitStats.attackSpeed;
        //atRangeText.text = "공격거리 : " + (unit.UnitStats.attackRange / 2).ToString() + "칸";
        //mentalText.text = "멘탈 : " + unit.UnitStats.mental.ToString();

        UpdateUnitStateUI();
    }
    
    public void HideUntInfo()
    {
        unitInfoImage.gameObject.SetActive(false);
        unitStatePanel.SetActive(false);
        typeInfo.SetActive(false);

    }

    public void UpdateHPUI(Unit unit)
    {
        if(unit != null)
        {
            unitHPText.text = $"{unit.Hp} / {unit.Maxhp}";
            unitHPImage.fillAmount = unit.HpPercent;
        }
    }

    

    public void SetUnitTierIcon(int tier)
    {
        for (int i = 0; i < tierImage.Length; i++)
        {
            tierImage[i].gameObject.SetActive(i < tier);
        }
    }

    private string GetAttackTypeInfo(UnitData unitData)
    {
        string attackTypeInfo = "";

        //if (unitData.AttackType == "베기")
        //{
        //    attackTypeInfo = "철갑에 약하다. 철갑을 입은 대상에게 주는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        //}
        //else if (unitData.AttackType == "찌르기")
        //{
        //    attackTypeInfo = "방탄갑에 약하다. 방탄갑을 입은 대상에게 주는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        //}
        //else if(unitData.AttackType == "때리기")
        //{
        //    attackTypeInfo = "완충갑에 약하다. 완충갑을 입은 대상에게 주는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        //}

        return attackTypeInfo;
    }


    public void UpdateUnitStateUI()
    {
        if (selecteUnitManger.SelectedUnit != null)
        {
            IReadOnlyList<DurationEffect> effects = selecteUnitManger.SelectedUnit.EffectList;
            //HashSet<Sprite> usedSprites = new HashSet<Sprite>();
            int imageIndex = 0;

            for (int i = 0; i < effects.Count && imageIndex < unitStateImage.Length; i++)
            {
                unitStateImage[imageIndex].sprite = effects[imageIndex].IconSprite;
                unitStateImage[imageIndex].gameObject.SetActive(true);
                unitStateUIs[imageIndex].SetEffect(effects[imageIndex]);
                imageIndex++;

                //if (sprite != null && !usedSprites.Contains(sprite))
                //{
                //    usedSprites.Add(sprite);

                //    // Sprite가 다르면 Sprite 업데이트
                //    if (unitStateImage[imageIndex].sprite != sprite)
                //    {
                //        unitStateImage[imageIndex].sprite = sprite;
                //    }

                //    // 꺼져있으면 켜기
                //    if (!unitStateImage[imageIndex].gameObject.activeSelf)
                //    {
                //        unitStateImage[imageIndex].gameObject.SetActive(true);
                //    }

                //    // Effect도 갱신
                //    unitStateUIs[imageIndex].SetEffect(effect);

                //    imageIndex++;
                //}
            }

            // 이펙트가 더 적어졌을 때만 뒤에 남은 이미지를 꺼주기
            for (int i = imageIndex; i < unitStateImage.Length; i++)
            {
                if (unitStateImage[i].gameObject.activeSelf)
                {
                    unitStateImage[i].gameObject.SetActive(false);
                }
            }
        }
    }


    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(selecteUnitManger.SelectedUnit is AllyUnit)
                upgradeMenuUI.HideUpgradeUI();

            inputEventManager.OnESCTarget = selecteUnitManger;
            inputEventManager.OnRightClickTarget = selecteUnitManger;
            selecteUnitManger.OnUpgrade(false);
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (selecteUnitManger.SelectedUnit is AllyUnit)
                upgradeMenuUI.HideUpgradeUI();

            inputEventManager.OnRightClickTarget = selecteUnitManger;
            inputEventManager.OnESCTarget = selecteUnitManger;
            selecteUnitManger.OnUpgrade(false);
        }
    }

    public void ShowUnitDurtion(float durtiaon)
    {
        unitDurationPrefab.SetActive(true);

        unitDuration.fillAmount = durtiaon;
    }

    public void HideUnitDuration()
    {
        unitDurationPrefab.SetActive(false);
        unitDuration.fillAmount = 0f;
    }

    private void UpdateSkillInfo(UnitData unitData)
    {
        unitGSkillImage.sprite = unitData.GeneralSkill.Icon;
        //--Local 스킬이름
        gSkillNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.GeneralSkill.Name}_name", LocalizationSettings.SelectedLocale);
        //--Local 스킬 설명 (desc + effect)
        gSkillInfoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.GeneralSkill.Name}_desc", LocalizationSettings.SelectedLocale);
        gSkillEffectText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.GeneralSkill.Name}_effect", LocalizationSettings.SelectedLocale);

        // Smart String Arguments용
        string gCooltime = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillCooltime",
            new object[] { new { num = unitData.GeneralSkill.CoolTime } });

        var gRange = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillRange",
            new object[] { new { num = unitData.GeneralSkill.Range / 2 } });

        var gMental = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillMental",
            new object[] { new { num = unitData.GeneralSkill.ActiveMental } });

        gSkillEtcText.text = $"{gCooltime} / {gRange}  / {gMental}";
        //infoGSkillEtc.text = $"쿨타임 {unitData.GeneralSkill.CoolTime}초 / 사거리 {unitData.GeneralSkill.Range}보 / 멘탈 요구 {unitData.GeneralSkill.ActiveMental}";

        // special skill
        unitSSkillImage.sprite = unitData.SpecialSkill.Icon;
        sSkilNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.SpecialSkill.Name}_name", LocalizationSettings.SelectedLocale);
        sSkillInfoText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.SpecialSkill.Name}_desc", LocalizationSettings.SelectedLocale);
        sSkillEffectText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("UnitSkill", $"{unitData.SpecialSkill.Name}_effect", LocalizationSettings.SelectedLocale);

        // Smart String Arguments용
        var sCooltime = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillCooltime",
            new object[] { new { num = unitData.SpecialSkill.CoolTime } });

        var sRange = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillRange",
            new object[] { new { num = unitData.SpecialSkill.Range / 2 } });

        var sMental = LocalizationSettings.StringDatabase.
            GetLocalizedString("CommonUI", "CON_skillMental",
            new object[] { new { num = unitData.SpecialSkill.ActiveMental } });

        sSkillEtcText.text = $"{sCooltime} / {sRange} / {sMental}";
        //infoSSkillEtc.text = $"쿨타임 {unitData.SpecialSkill.CoolTime}초 / 사거리 {unitData.SpecialSkill.Range}보 / 멘탈 요구 {unitData.SpecialSkill.ActiveMental}";

        if (unitData.SpecialAbility != null)
        {
            // 아이콘이 있는 경우 알파값1로 변경
            specialAbilityImage.sprite = unitData.SpecialAbility.Icon;
            specialAbilityImage.gameObject.SetActive(true);
            //specialAbilityImage.color = new Color(1, 1, 1, 1f);
            //specialAbilityImage.sprite = unitData.SpecialAbility.Icon;

            sAbilityNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("SpecialAbility", $"{unitData.SpecialAbility.Id}_name", LocalizationSettings.SelectedLocale);
            sAbilityInfoText.text = LocalizationSettings.StringDatabase.
                GetLocalizedString("SpecialAbility", $"{unitData.SpecialAbility.Id}_desc", LocalizationSettings.SelectedLocale);
            sAbilityEffectText.text = LocalizationSettings.StringDatabase.
                GetLocalizedString("SpecialAbility", $"{unitData.SpecialAbility.Id}_effect", LocalizationSettings.SelectedLocale);


        }
        else
        {
            specialAbilityImage.gameObject.SetActive(false);

            // 아이콘이 없는 경우 알파값0으로 변경
            //passiveIcon.color = new Color(1, 1, 1, 0f);
            //infoAbilityName.text = "";
            //infoAbilityDescript.text = "";
            //infoAbilityEffect.text = "";
            //passiveIcon.gameObject.SetActive(false);
        }
    }
}