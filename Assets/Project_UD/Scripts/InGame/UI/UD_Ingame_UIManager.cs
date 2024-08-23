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

    [Header("====UnitSpawnDeck====")]
    public GameObject unitSpawnPanel;
    public Button[] unitSpawnBtn;

    [Header("====UnitInfoPanel====")]
    public Image unitInfoImage;
    public Image levelImage;
    public Text nameText;
    public Image gSkillImage;
    public Text gSkillText;
    public Image sSkillImage;
    public Text sSkillText;
    public Image hpImage;
    public Text hpText;
    public Image attackTypeImage;
    public Text attackTypeText;
    public Image defenseTypeImage;
    public Text defeneTypeText;
    


    [Header("====GameOption====")]
    public Button endGameBtn;
    public Button restartGameBtn;
    public Button pauseGameBtn;

    [Header("====UnitOptionMenu====")]
    public GameObject slectedUnitOptionBox;         // 업그레이드 and 모드 변경 판넬
    public GameObject currentSelectedUnitOptionBox; // 프리펩으로 생성될 판넬
    public Button unitStateChageBtn;                // 유닛 모드 변경 버튼
    public Sprite FreeModeImage;                    // 모든 변경 버튼 Free
    public Sprite SiegeModeImage;                   // 모든 변경 버튼 Siege
    public GameObject unitUpgradeMenuBox;           // 유닛 업그레이드 메뉴 판넬
    public GameObject currentUpgradeMenu;           // 프리펩으로 생성될 업그레이드 판넬

    public int curUnitHp;


    public GameObject UnitStateChangeBox;
    public GameObject currentUnitStateChangeBox;

    private bool isPasue = false;

    private AllyMode allyMode;

    private UD_Ingame_UnitCtrl selectedUnit;


    Camera mainCamera;


    public Button unitUpgradeBtn;


    public Image unitMoveImage;
    public GameObject unitMoveUI;

    private UD_UnitDataManager unitDataManager;


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

        unitDataManager = UD_UnitDataManager.inst;


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

        UpdateMoveImagesForAllUnits();
    }


    public void UpdateUnitInfoPanel(UD_Ingame_UnitCtrl selectedUnit)
    {
        string unitDataKey = "";
        if(selectedUnit.unitType == UnitType.Warrior)
        {
            unitDataKey = "민병";
        }
        else if(selectedUnit.unitType == UnitType.Archer)
        {
            unitDataKey = "사냥꾼";
        }

        UD_UnitDataManager.UnitData unitData = unitDataManager.GetUnitData(unitDataKey);

        curUnitHp = unitData.Hp;

        if(unitData != null)
        {
            nameText.text = unitData.Name;
            gSkillText.text = unitData.g_SkillName;
            sSkillText.text = unitData.s_SkillName;
            hpText.text = curUnitHp.ToString() + "/" + unitData.Hp.ToString();
            defeneTypeText.text = unitData.DefenseType;
            //attackTypeText.text = unitData.
           
        }
        
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

    public void CreateSeletedUnitdOptionBox(Vector3 worldPosition, UD_Ingame_UnitCtrl unit)
    {
        if (unit.Ally_Mode == AllyMode.Change)
        {
            return;
        }


        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

        if (slectedUnitOptionBox != null)
        {
            Destroy(currentSelectedUnitOptionBox);
            currentSelectedUnitOptionBox = null;
        }

        currentSelectedUnitOptionBox = Instantiate(currentSelectedUnitOptionBox) as GameObject;

        SetSelectedUnit(unit);

        GameObject canvas = GameObject.Find("Canvas");
        currentSelectedUnitOptionBox.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = currentSelectedUnitOptionBox.GetComponent<RectTransform>();
        screenPos.x += 140;
        screenPos.y -= 90;

        rectTransform.position = screenPos;

        // 모드 전환 버튼
        unitStateChageBtn = currentUnitStateChangeBox.transform.Find("ChangeStateBtn").GetComponent<Button>();
        // 업그레이드 버튼
        unitUpgradeBtn = currentUnitStateChangeBox.transform.Find("UnitUpgradeBtn").GetComponent<Button>();

        // 모드 전환 버튼
        if (unitStateChageBtn != null)
        {
            unitStateChageBtn.onClick.RemoveAllListeners();
            unitStateChageBtn.onClick.AddListener(() =>
            {
                UnitStateChange(unit);

                DestroyUnitStateChangeBox();
            });
        }

        Image buttonImage = unitStateChageBtn.GetComponent<Image>();

        if (selectedUnit.Ally_Mode == AllyMode.Siege)
        {
            buttonImage.sprite = FreeModeImage;
        }
        else if (selectedUnit.Ally_Mode == AllyMode.Free)
        {
            buttonImage.sprite = SiegeModeImage;
        }

        // 업그레이드 버튼
        if (unitUpgradeBtn != null)
        {
            // 업그레이드 메뉴 생성


            //unitStateChageBtn.onClick.RemoveAllListeners();
            //unitStateChageBtn.onClick.AddListener(() =>
            //{
            //    UnitStateChange(unit);

            //    DestroyUnitStateChangeBox();
            //});
        }
    }

    void CreateUpgradeMenu(Vector3 worldPosition, UD_Ingame_UnitCtrl unit)
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

    }

    public void CreateUnitStateChangeBox(Vector3 worldPosition, UD_Ingame_UnitCtrl unit)
    {
        if (unit.Ally_Mode == AllyMode.Change)
        {
            return; 
        }


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

        unitStateChageBtn = currentUnitStateChangeBox.transform.Find("ChangeStateBtn").GetComponent<Button>();


        if (unitStateChageBtn != null)
        {
            unitStateChageBtn.onClick.RemoveAllListeners();
            unitStateChageBtn.onClick.AddListener(() =>
            {
                UnitStateChange(unit);

                // 버튼을 누른 후에 UnitStateChangeBox를 삭제
                DestroyUnitStateChangeBox();
            });
        }
        Image buttonImage = unitStateChageBtn.GetComponent<Image>();


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

    public void UnitStateChange(UD_Ingame_UnitCtrl unit)
    {
        if (unit != null && unit.isSelected) // 선택된 유닛만 상태 변경
        {
            unit.ChangeAllyMode(); // 버튼 클릭 시 유닛의 Ally_Mode 변경
        }
    }

    public void ShowMoveImage(bool show)
    {
        if (unitMoveUI != null)
        {
            unitMoveUI.SetActive(show);
        }
    }

    public void UpdateMoveImagesForAllUnits()
    {
        UD_Ingame_UnitCtrl[] allUnits = FindObjectsOfType<UD_Ingame_UnitCtrl>();

        foreach (UD_Ingame_UnitCtrl unitCtrl in allUnits)
        {

            if (unitCtrl.CompareTag("Unit"))
            {
                if (unitCtrl.Ally_State.fsm.State == UnitState.Move)// ||
                    //unitCtrl.Ally_State.fsm.State == UnitState.Chase)
                {
                    ShowMoveImage(unitCtrl.gameObject, true);
                }
                else
                {
                    ShowMoveImage(unitCtrl.gameObject, false);
                }
            }
        }
    }

    public void ShowMoveImage(GameObject unit, bool show)
    {
        if (unit == null)
        {
            return;
        }

        Transform unitMoveImageTransform = unit.transform.Find("Canvas/UnitMoveImage");

        if (unitMoveImageTransform == null)
        {
            return;
        }

        GameObject unitMoveImage = unitMoveImageTransform.gameObject;
        unitMoveImage.SetActive(show);

        if (show)
        {
            Canvas canvas = unitMoveImage.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode == RenderMode.WorldSpace)
            {
                RectTransform rectTransform = unitMoveImage.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(1, 1);

                rectTransform.position = unit.transform.position + new Vector3(0, 2.0f, 0);
                rectTransform.rotation = Quaternion.identity;
            }
        }
    }
}