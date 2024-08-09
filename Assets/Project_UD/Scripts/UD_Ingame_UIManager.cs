using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class UD_Ingame_UIManager : MonoBehaviour
{
    public static UD_Ingame_UIManager instance;

    private UD_Ingame_UnitSpawnManager unitSpawnManager;

    public Button allyUnitSetMode = null;

    [Header("====UnitInfoPanel====")]
    public Text unitName;
    public Text unitType;
    public Text unitTier;
    public Text unitWeapon;
    public Text unitSkill;
    public Text unitDamage;
    public Text unitAttackType;
    public Button unitModeChangeBtn;
    public Button unitUpgradeBtn;

    [Header("====GameOption====")]
    public Button endGameBtn;
    public Button restartGameBtn;
    public Button pauseGameBtn;

    private bool isPasue = false;

    private AllyMode allyMode;

    private UD_Ingame_UnitCtrl selectedUnit;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        unitSpawnManager = UD_Ingame_UnitSpawnManager.inst;


        if (allyUnitSetMode != null)
        {
            allyUnitSetMode.onClick.AddListener(() => 
            {
                UD_Ingame_GameManager.inst.AllyUnitSetMode = !UD_Ingame_GameManager.inst.AllyUnitSetMode;
            });
        }

        if(endGameBtn != null)
        {
            endGameBtn.onClick.AddListener(EndGame);
        }

        if(restartGameBtn != null)
        {
            restartGameBtn.onClick.AddListener(RestartGame);
        }

        if(pauseGameBtn != null)
        {
            pauseGameBtn.onClick.AddListener(PauseGame);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (UD_Ingame_GameManager.inst.AllyUnitSetMode)
        //{
        //    allyUnitSetMode.GetComponentInChildren<Text>().text = "Ally Unit Set Mode OFF";
        //}
        //else
        //{
        //    allyUnitSetMode.GetComponentInChildren<Text>().text = "Ally Unit Set Mode ON";
        //}

    }


    public void UpdateUnitInfoPanel(string unitName)
    {

        //UD_UnitDataManager.UnitData unitData = UD_UnitDataManager.inst.GetUnitData(unitName);
        //if (unitData != null)
        //{
        //    this.unitName.text = "이름 : " + unitData.Name;
        //    this.unitType.text = "타입 : " + unitData.Type;
        //    this.unitTier.text = "티어 : " + unitData.Tier.ToString();
        //    this.unitWeapon.text = "무기 : " + unitData.Weapon;
        //    this.unitSkill.text = "스킬 : " + unitData.Skill;
        //    this.unitDamage.text = "데미지 : " + unitData.Damage.ToString();
        //    this.unitAttackType.text = "공격 타입 : " + unitData.AttackType;
        //}
    }

    void SetUnitFreeMode(UD_Ingame_UnitCtrl unit)
    {
        unit.Ally_Mode = AllyMode.Free;
    }

    void SetUnitSiegeMode(UD_Ingame_UnitCtrl unit)
    {
        unit.Ally_Mode = AllyMode.Siege;
    }

    void EndGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void PauseGame()
    {
        if(isPasue == false)
        {
            Time.timeScale = 0.0f;
            isPasue = true;
        }
        else
        {
            Time.timeScale = 1.0f;
            isPasue = false;
        }

    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
