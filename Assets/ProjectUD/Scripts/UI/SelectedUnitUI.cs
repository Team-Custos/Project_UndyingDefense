using Cinemachine;
using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnitDataManager;
using static UnityEngine.UI.CanvasScaler;

public class SelectedUnitUI : MonoBehaviour, IInputESC, IInputRightClick
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private SelectedUnitManager selecteUnitManger;
    [SerializeField] private UpgradeMenuUI upgradeMenuUI;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private Ingame_CamManager ingameCamManager;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private InGameManager inGameManager;
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

    [SerializeField] private TextMeshProUGUI unitGSkillText;
    [SerializeField] private TextMeshProUGUI unitSSkillText;
    [SerializeField] private TextMeshProUGUI unitDefenseTypeText;

    // 상태 아이콘
    [SerializeField] private Sprite bleedSprite;
    [SerializeField] private Sprite painSprite;
    [SerializeField] private Sprite shockSprite;
    [SerializeField] private Sprite stunSprite;
    [SerializeField] private Sprite trappedSprite;
    [SerializeField] private Sprite fearSprite;
    [SerializeField] private Sprite provokeSprite;
    [SerializeField] private Sprite weakenSprite;
    [SerializeField] private Sprite fastMoveSprite;
    [SerializeField] private Sprite shrinkSprite;
    [SerializeField] private Sprite burnSprite;
    [SerializeField] private Sprite igniteSprite;
    [SerializeField] private Sprite posionSprite;
    [SerializeField] private Sprite focusSprite;
    [SerializeField] private Sprite executeSprite;
    [SerializeField] private Sprite defenseSprite;
    [SerializeField] private Sprite fortitudeSprite;
    [SerializeField] private Sprite crushSprite;

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

    public void UpdateUnitInfoByBtn(UnitData unitData)
    {
        unitInfoImage.gameObject.SetActive(true);

        unitImage.sprite = unitData.Icon;
        unitNameText.text = unitData.Name;
        unitMentalText.text =  "멘탈 : " + unitData.Mental.ToString();

        unitHPImage.fillAmount = unitData.MaxHp / unitData.MaxHp;
        unitHPText.text = $"{unitData.MaxHp} / {unitData.MaxHp}";

        SetUnitTierIcon(unitData.Tier);

        atTypeIcon.sprite = unitData.AtTypeIcon;
        dfTypeIcon.sprite = unitData.DfTypeIcon;

        unitDefenseTypeText.text = ConvertDefenseName(unitData.ArmorType.ToString());

        Unit unit = unitData.Prefab.GetComponent<Unit>();

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


        critText.text = "치명타율 : " + unitData.CritChance.ToString() + "%";
        moveSpeedText.text = "이동속도 : " + unitData.MoveSpeed.ToString();
        atSpeedText.text = "공격속도 : " + unitData.AttackSpeed.ToString();
        atRangeText.text = "공격거리 : " + unitData.AttackRange.ToString() + "칸";
        mentalText.text = "멘탈 : " + unitData.Mental.ToString();
    }

    public void UpdateUnitInfo(Unit unit)
    {
       unit.SetUnitUI(this);

       unitInfoImage.gameObject.SetActive(true);

       UpdateHPUI(unit);
       unitImage.sprite = unit.Data.Icon;
       unitNameText.text = unit.Data.Name;
       unitMentalText.text = "멘탈 : " + unit.Data.Mental.ToString();

        SetUnitTierIcon(unit.Data.Tier);

        atTypeIcon.sprite = unit.Data.AtTypeIcon;
       dfTypeIcon.sprite = unit.Data.DfTypeIcon;

       unitDefenseTypeText.text = ConvertDefenseName(unit.Data.ArmorType.ToString());
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


        critText.text = "치명타율 : " + unit.Data.CritChance.ToString() + "%";
        moveSpeedText.text = "이동속도 : " + unit.Data.MoveSpeed.ToString();
        atSpeedText.text = "공격속도 : " + unit.Data.AttackSpeed.ToString();
        atRangeText.text = "공격거리 : " + unit.Data.AttackRange.ToString() + "칸";
        mentalText.text = "멘탈 : " + unit.Data.Mental.ToString();

        UpdateUnitStateUI();
    }
    
    public void HideUntInfo()
    {
        unitInfoImage.gameObject.SetActive(false);

    }

    public void UpdateHPUI(Unit unit)
    {
        if(unit != null)
        {
            unitHPText.text = $"{unit.Hp} / {unit.Data.MaxHp}";
            unitHPImage.fillAmount = unit.HpPercent;
        }
    }

    public string ConvertDefenseName(string armorType)
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

    public void SetUnitTierIcon(int tier)
    {
        for (int i = 0; i < tierImage.Length; i++)
        {
            tierImage[i].gameObject.SetActive(i < tier);
        }
    }

    public void UpdateUnitStateUI()
    {
        if(selecteUnitManger.SelectedUnit != null)
        {
            // 모든 이미지 끄기
            for (int i = 0; i < unitStateImage.Length; i++)
            {
                unitStateImage[i].gameObject.SetActive(false);
            }

            List<Effect> effects = selecteUnitManger.SelectedUnit.EffectList;

            HashSet<Sprite> usedSprites = new HashSet<Sprite>();

            int imageIndex = 0;

            for (int i = 0; i < effects.Count && imageIndex < unitStateImage.Length; i++)
            {
                Effect effect = effects[i];
                Sprite sprite = GetSpriteForEffect(effect.Id);

                // 스프라이트가 유효하고 중복이 아닐 때만 표시
                if (sprite != null && !usedSprites.Contains(sprite))
                {
                    usedSprites.Add(sprite);

                    unitStateImage[imageIndex].sprite = sprite;
                    unitStateImage[imageIndex].gameObject.SetActive(true);
                    imageIndex++;
                }
                
            }

            for (int i = imageIndex; i < unitStateImage.Length; i++)
            {
                unitStateImage[i].gameObject.SetActive(false);
            }


            //for (int i = imageIndex; i < unitStateImage.Length; i++)
            //{
            //    unitStateImage[i].gameObject.SetActive(false);
            //}
        }

        
    }

    // ID에 따라 스프라이트 결정
    private Sprite GetSpriteForEffect(string id)
    {
        switch (id)
        {
            case "Bleed": return bleedSprite;
            case "OverBleed": return bleedSprite;
            case "Pain": return painSprite;
            case "Shock": return shockSprite;
            case "Stun": return stunSprite;
            case "Trapped": return trappedSprite;
            case "Fear": return fearSprite;
            case "Provoke": return provokeSprite;
            case "Weaken": return weakenSprite;
            case "FastMove": return fastMoveSprite;
            case "Shrink": return shrinkSprite;
            case "Burn": return burnSprite;
            case "Ignite": return igniteSprite;
            case "Posion": return posionSprite;
            case "Focus": return focusSprite;
            case "Execute": return executeSprite;
            case "Defense": return defenseSprite;
            case "Fortitude": return fortitudeSprite;

            default:
                Debug.LogWarning($"Effect ID '{id}'에 해당하는 스프라이트가 없습니다.");
                return null;
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