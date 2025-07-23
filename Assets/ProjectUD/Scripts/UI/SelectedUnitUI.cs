using Cinemachine;
using InputEventInterface;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private Text critText;
    [SerializeField] private Text moveSpeedText;
    [SerializeField] private Text atSpeedText;
    [SerializeField] private Text atRangeText;
    [SerializeField] private Text mentalText;
    [SerializeField] private Image[] tierImage;
    [SerializeField] private Text gSkillInfoText;
    [SerializeField] private Text gSkillNameText;
    [SerializeField] private Text sSkillInfoText;
    [SerializeField] private Text sSkilNameText;
    [SerializeField] private Image[] unitStateImage;
    [SerializeField] private UnitStateUI[] unitStateUIs;
    [SerializeField] private GameObject unitStatePanel;

    [SerializeField] private TextMeshProUGUI attackTypeText;
    [SerializeField] private TextMeshProUGUI attackTypeInfoText;
    [SerializeField] private TextMeshProUGUI defenseTypeText;
    [SerializeField] private TextMeshProUGUI defenseTypeInfoText;


    [SerializeField] private GameObject allyUnitUI;
    [SerializeField] private GameObject enemyUnitUI;

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
        }

    }

    public void HideUpgrdeUI()
    {
        unitUpgradeMenuPrefab.SetActive(false);
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

                Vector3 worldPosition = selecteUnitManger.SelectedUnit.transform.position + Vector3.up * yPos;
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


            if (unitUpgradeMenuPrefab != null)
            {

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
            }
        }
    }

    public void UpdateUnitInfoByBtn(UnitData unitData, UnitDataLoader unitDataLoader)
    {
        Unit unit = unitData.Prefab.GetComponent<Unit>();

        UnitStats unitStats = unitDataLoader.GetUnitDataById(unit.UnitId, unit);

        unitInfoImage.gameObject.SetActive(true);


        unitImage.sprite = unitData.Icon;

        unitNameText.text = unitStats.unitName;
        unitMentalText.text =  "멘탈 : " + unitStats.mental .ToString();
        unitHPImage.fillAmount = unitStats.maxHp / unitStats.maxHp;
        unitHPText.text = $"{unitStats.maxHp} / {unitStats.maxHp}";

        SetUnitTierIcon(unitData.Tier);

        atTypeIcon.sprite = unitData.AtTypeIcon;
        dfTypeIcon.sprite = unitData.DfTypeIcon;

        //attackTypeText.text = unitData.AttackType;
        attackTypeInfoText.text = GetAttackTypeInfo(unitData);

        defenseTypeText.text = ConvertDefenseName(unitData.ArmorType.ToString());
        defenseTypeInfoText.text = GetDefenseTypeInfo(unitData);

        //unitGSkillText.text = unit.GeneralSkill.Data.name;

        unitGSkillImage.sprite = unit.GeneralSkill.Data.Icon;

        gSkillNameText.text = unit.GeneralSkill.Data.Name;
        gSkillInfoText.text = unit.GeneralSkill.Data.Description;

        if (unit.SpecialSkill != null)
        {
            unitSSkillImage.sprite = unit.SpecialSkill.Data.Icon;
            sSkilNameText.text = unit.SpecialSkill.Data.Name;
            sSkillInfoText.text = unit.SpecialSkill.Data.Description;

        }
        else if(unit.PassiveSkill != null)
        {
            unitSSkillImage.sprite = unit.PassiveSkill.Data.Icon;
            sSkilNameText.text = unit.PassiveSkill.Data.Name;
            sSkillInfoText.text = unit.PassiveSkill.Data.Description;
        }


        critText.text = "치명타율 : " + unitStats.critChance.ToString() + "%";
        moveSpeedText.text = "이동속도 : " + unitStats.moveSpeed.ToString();
        atSpeedText.text = "공격속도 : " + unitStats.attackSpeed.ToString();
        atRangeText.text = "공격거리 : " + (unitStats.attackRange / 2).ToString() + "칸";
        mentalText.text = "멘탈 : " + unitStats.mental.ToString();

        // 상태 이미지 끄기
        for(int i = 0; i < unitStateImage.Length; i++)
        {
            unitStateImage[i].gameObject.SetActive(false);
        }
    }

    public void UpdateUnitInfo(Unit unit)
    {
       unit.SetUnitUI(this);

       unitInfoImage.gameObject.SetActive(true);

        if(unit.UnitStats == null)
        {
            Debug.Log("데이터 없음");
            return;
        }

       UpdateHPUI(unit);
       unitImage.sprite = unit.Data.Icon;
       unitNameText.text = unit.UnitStats.unitName;
       unitMentalText.text = "멘탈 : " + unit.UnitStats.mental.ToString();

        SetUnitTierIcon(unit.Data.Tier);

        atTypeIcon.sprite = unit.Data.AtTypeIcon;
       dfTypeIcon.sprite = unit.Data.DfTypeIcon;

        //attackTypeText.text = unit.Data.AttackType;
        attackTypeInfoText.text = GetAttackTypeInfo(unit.Data);

        defenseTypeText.text = ConvertDefenseName(unit.Data.ArmorType.ToString());
        defenseTypeInfoText.text = GetDefenseTypeInfo(unit.Data);

        //unitGSkillText.text = unit.GeneralSkill.Data.name;

        unitGSkillImage.sprite = unit.GeneralSkill.Data.Icon;
        gSkillNameText.text = unit.GeneralSkill.Data.Name;
        gSkillInfoText.text = unit.GeneralSkill.Data.Description;

        if (unit.SpecialSkill != null)
        {
            unitSSkillImage.sprite = unit.SpecialSkill.Data.Icon;

            sSkilNameText.text = unit.SpecialSkill.Data.Name;
            sSkillInfoText.text = unit.SpecialSkill.Data.Description;

        }
        else if(unit.PassiveSkill != null)
        {
            unitSSkillImage.sprite = unit.PassiveSkill.Data.Icon;

            sSkilNameText.text = unit.PassiveSkill.Data.Name;
            sSkillInfoText.text = unit.PassiveSkill.Data.Description;
        }


        critText.text = "치명타율 : " + unit.UnitStats.critChance.ToString() + "%";
        moveSpeedText.text = "이동속도 : " + unit.UnitStats.moveSpeed.ToString();
        atSpeedText.text = "공격속도 : " + unit.UnitStats.attackSpeed.ToString();
        atRangeText.text = "공격거리 : " + (unit.UnitStats.attackRange / 2).ToString() + "칸";
        mentalText.text = "멘탈 : " + unit.UnitStats.mental.ToString();

        UpdateUnitStateUI();
    }
    
    public void HideUntInfo()
    {
        unitInfoImage.gameObject.SetActive(false);
        unitStatePanel.SetActive(false);

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

    private string GetDefenseTypeInfo(UnitData unitData)
    {
        string defensTypeInfo = "";

        if (unitData.ArmorType == Unit.ArmorType.STEELPLATED)
        {
            defensTypeInfo = "베기에 강하다. 베기 공격에 받는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        }
        else if (unitData.ArmorType == Unit.ArmorType.PADDED)
        {
            defensTypeInfo = "때리기에 강하다. 때리기 공격에 받는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        }
        else if (unitData.ArmorType == Unit.ArmorType.ANTIPIERCING)
        {
            defensTypeInfo = "찌르기에 강하다. 찌르기 공격에 받는 총 데미지 30% 감소, 치명타율 총 30% 감소";
        }

        return defensTypeInfo;
    }

    private string ConvertDefenseName(string armorType)
    {
        if (armorType == Unit.ArmorType.PADDED.ToString())
        {
            return "완충갑";
        }
        else if (armorType == Unit.ArmorType.ANTIPIERCING.ToString())
        {
            return "방탄갑";
        }
        else if (armorType == Unit.ArmorType.STEELPLATED.ToString())
        {
            return "철갑";
        }
        else
            return "정보없음";
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
}