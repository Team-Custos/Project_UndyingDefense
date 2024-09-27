using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Ingame_UIManager : MonoBehaviour
{
    public static Ingame_UIManager instance;

    UnitSpawnManager unitSpawnManager;

    public Button EnemyTestModeBtn = null;
    public Text UnitSetModeText = null;

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

    private Ingame_UnitCtrl selectedUnit;

    public GameObject unitSpawnPanel;
    public Button[] unitSpawnBtn;


    private void Awake()
    {
        instance = this;
        unitSpawnManager = UnitSpawnManager.inst;
        unitSpawnBtn = unitSpawnPanel.GetComponentsInChildren<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int ii = 0; ii < unitSpawnBtn.Length; ii++)
        {
            if (unitSpawnBtn[ii] != null)
            {
                int idx = ii;
                unitSpawnBtn[idx].onClick.AddListener(() => 
                {
                    UnitSpawnManager.inst.unitToSpawn = unitSpawnBtn[idx].GetComponent<Ingame_UnitSpawnBtnStatus>().UnitCode;
                    InGameManager.inst.UnitSetMode = !InGameManager.inst.UnitSetMode;
                    InGameManager.inst.AllyUnitSetMode = !InGameManager.inst.AllyUnitSetMode;
                });
            }
        }

        if (EnemyTestModeBtn != null)
        {
            EnemyTestModeBtn.onClick.AddListener(()=> 
            {
                InGameManager.inst.UnitSetMode = !InGameManager.inst.UnitSetMode;
                InGameManager.inst.EnemyUnitSetMode = !InGameManager.inst.EnemyUnitSetMode;
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
        


        if (UnitSetModeText != null)
        {
            if (InGameManager.inst.UnitSetMode)
            {
                UnitSetModeText.text = "UnitSetMode : ON";
                if (InGameManager.inst.AllyUnitSetMode)
                {
                    UnitSetModeText.color = Color.cyan;
                }
                else if (InGameManager.inst.EnemyUnitSetMode)
                {
                    UnitSetModeText.color = Color.red;
                }
            }
            else
            {
                UnitSetModeText.text = "UnitSetMode : OFF";
                UnitSetModeText.color = Color.white;
            }
        }
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
