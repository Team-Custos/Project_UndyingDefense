using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using static UnityEngine.UI.CanvasScaler;

public class UD_Ingame_UIManager : MonoBehaviour
{
    public static UD_Ingame_UIManager instance;

    UD_Ingame_UnitSpawnManager unitSpawnManager;

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

    private UD_Ingame_UnitCtrl selectedUnit;

    public GameObject unitSpawnPanel;
    public Button[] unitSpawnBtn;

    Camera mainCamera;

    public GameObject UnitStateChangeBox;
    public GameObject currentUnitStateChangeBox;
    public Sprite FreeModeImage;
    public Sprite SiegeModeImage;

    public Button uniStateChageBtn;
    private float stateChangeTimer = 0f;
    private bool isStateChanging = false;




    private void Awake()
    {
        instance = this;
        unitSpawnManager = UD_Ingame_UnitSpawnManager.inst;
        unitSpawnBtn = unitSpawnPanel.GetComponentsInChildren<Button>();
    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        for (int ii = 0; ii < unitSpawnBtn.Length; ii++)
        {
            if (unitSpawnBtn[ii] != null)
            {
                int idx = ii;
                unitSpawnBtn[idx].onClick.AddListener(() =>
                {
                    UD_Ingame_UnitSpawnManager.inst.unitToSpawn = unitSpawnBtn[idx].GetComponent<UD_Ingame_UnitSpawnBtnStatus>().UnitCode;
                    UD_Ingame_GameManager.inst.UnitSetMode = !UD_Ingame_GameManager.inst.UnitSetMode;
                    UD_Ingame_GameManager.inst.AllyUnitSetMode = !UD_Ingame_GameManager.inst.AllyUnitSetMode;

                    DestroyUnitStateChangeBox();

                });
            }
        }

        if (EnemyTestModeBtn != null)
        {
            EnemyTestModeBtn.onClick.AddListener(() =>
            {
                UD_Ingame_GameManager.inst.UnitSetMode = !UD_Ingame_GameManager.inst.UnitSetMode;
                UD_Ingame_GameManager.inst.EnemyUnitSetMode = !UD_Ingame_GameManager.inst.EnemyUnitSetMode;

                DestroyUnitStateChangeBox();


            });
        }



        if (endGameBtn != null)
        {
            endGameBtn.onClick.AddListener(EndGame);
        }

        if (restartGameBtn != null)
        {
            restartGameBtn.onClick.AddListener(RestartGame);
        }

        if (pauseGameBtn != null)
        {
            pauseGameBtn.onClick.AddListener(PauseGame);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (UnitSetModeText != null)
        {
            if (UD_Ingame_GameManager.inst.UnitSetMode)
            {


                if (UD_Ingame_GameManager.inst.AllyUnitSetMode)
                {
                    UnitSetModeText.color = Color.white;
                    UnitSetModeText.text = "UnitSetMode";
                }
                else if (UD_Ingame_GameManager.inst.EnemyUnitSetMode)
                {
                    UnitSetModeText.color = Color.red;
                    UnitSetModeText.text = "EnemySetMode";
                }
            }
            else
            {
                UnitSetModeText.color = Color.black;
                UnitSetModeText.text = "SetModeOff";
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
        if (isPasue == false)
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

    public void CreateUnitStateChangeBox(Vector3 worldPosition, UD_Ingame_UnitCtrl unit)
    {

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

        if (currentUnitStateChangeBox != null)
        {
            Destroy(currentUnitStateChangeBox);
            currentUnitStateChangeBox = null;
        }

        currentUnitStateChangeBox = Instantiate(UnitStateChangeBox) as GameObject;

        SetSelectedUnit(unit);

        GameObject canvas = GameObject.Find("Canvas");
        currentUnitStateChangeBox.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = currentUnitStateChangeBox.GetComponent<RectTransform>();
        screenPos.x += 140;
        screenPos.y -= 90;

        rectTransform.position = screenPos;

        uniStateChageBtn = currentUnitStateChangeBox.transform.Find("ChangeStateBtn").GetComponent<Button>();


        if (uniStateChageBtn != null)
        {
            // 이전 리스너 제거 후 새 리스너 추가
            uniStateChageBtn.onClick.RemoveAllListeners();
           // uniStateChageBtn.onClick.AddListener(() => UnitStateChange(unit));
        }
        Image buttonImage = uniStateChageBtn.GetComponent<Image>();


        if (selectedUnit.Ally_Mode == AllyMode.Siege)
        {
            buttonImage.sprite = FreeModeImage;
        }
        else if (selectedUnit.Ally_Mode == AllyMode.Free)
        {
            buttonImage.sprite = SiegeModeImage;
        }

    }

    public void DestroyUnitStateChangeBox() 
    {
        if (currentUnitStateChangeBox != null)
        {
            // UnitStateChangeBox를 삭제
            Destroy(currentUnitStateChangeBox);

            // 참조를 null로 설정하여 이후 문제가 발생하지 않도록 함
            currentUnitStateChangeBox = null;
        }
    }



    public void SetSelectedUnit(UD_Ingame_UnitCtrl unit)
    {
        selectedUnit = unit;
    }



}