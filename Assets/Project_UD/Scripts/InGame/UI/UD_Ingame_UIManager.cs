using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using static UnityEngine.UI.CanvasScaler;
using static UD_UnitDataManager;
using UnityEngine.Rendering.Universal;

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
    public GameObject unitInfoPanel;
    public Image unitInfoImage;
    public Image levelImage;
    public Text levelText;
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
    public Button UnitUpgrade1Btn;
    public Button UnitUpgrade2Btn;

    [Header("====Unit Upgrade Image====")]
    public Sprite upGradeImage_2Level;
    public Sprite upGradeImage_3Level;
    public Sprite upGradeImage_4Level;

    [Header("====Wave Menu UI====")]
    public float waveCount = 20;
    public Text waveCountText;
    public int waveStep;
    public Text waveStepText;
    public Text waveStartText;
    public GameObject waveResultPanel;
    public Text waveResultText;
    public Button reStartBtn;
    public Button menuBtn;
    public Text waveRewardText;
    public Image waveGoldImage;
    public Text waveGoldText;
    public bool isCurrentWaveFinshed;
    public bool isCurrentWaveSucceed;

    int unitLevel;

    public Text curGoldText;
    public int curHaveGold;


    public int curUnitHp;



    public GameObject UnitStateChangeBox;
    public GameObject currentUnitStateChangeBox;

    private bool isPasue = false;

    private AllyMode allyMode;

    private UD_Ingame_UnitCtrl selectedUnit;
    public GameObject unit;


    Camera mainCamera;


    public Button unitUpgradeBtn;


    public Image unitMoveImage;
    public GameObject unitMoveUI;

    private UD_UnitDataManager unitDataManager;
    private UD_Ingame_UnitUpgradeManager unitUpgradeManager;


    private void Awake()
    {
        instance = this;
        unitSpawnManager = UD_Ingame_UnitSpawnManager.inst;
        unitSpawnBtn = unitSpawnPanel.GetComponentsInChildren<Button>();

        GameObject upgradeManagerObj = GameObject.Find("UD_Ingame_UnitUpgradeManager");
        if (upgradeManagerObj != null)
        {
            unitUpgradeManager = upgradeManagerObj.GetComponent<UD_Ingame_UnitUpgradeManager>();
        }

        // 같은 오브젝트에 할당된 경우
        if (unitUpgradeManager == null)
        {
            unitUpgradeManager = GetComponent<UD_Ingame_UnitUpgradeManager>();
        }

        if (unitUpgradeManager == null)
        {
            Debug.LogError("unitUpgradeManager 초기화 실패");
        }

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

        curHaveGold = 10000000;

        waveCount = 20;
    }

    

    // Update is called once per frame
    void Update()
    {
        // 웨이브 카운트 다운 텍스트
        if (waveCountText != null)
        {
            waveCount -= Time.deltaTime;
            if (waveCount >= 0)
            {
                waveStartText.gameObject.SetActive(false);
                waveCountText.gameObject.SetActive(true);
                waveCountText.text = "적군 침공까지 " + Mathf.Ceil(waveCount).ToString() + "초";
            }
            else if (waveCount < 0)
            {
                waveCountText.gameObject.SetActive(false);
                waveStartText.gameObject.SetActive(true);
                isCurrentWaveFinshed = true;
                //ShowWaveResultPanel();
            }
        }

        //curGoldText.text = curHaveGold.ToString();

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
        

        //if (selectedUnit != null)
        //{
        //    unitInfoPanel.SetActive(true);
        //}
        //else
        //{
        //    unitInfoPanel.SetActive(false);
        //}

        // UI 유닛 따라가기
        if (selectedUnit != null && currentSelectedUnitOptionBox != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(selectedUnit.transform.position);
            screenPos.x += 140;
            screenPos.y -= 90;

            currentSelectedUnitOptionBox.transform.position = screenPos;
        }

        //LookUIMainCamera(unit);
    }


    public void UpdateUnitInfoPanel(UD_Ingame_UnitCtrl selectedUnit)
    {
        if (selectedUnit == null)
        {
            Debug.LogError("선택된 유닛이 null입니다.");
            return;
        }

        string unitID = selectedUnit.ID;
        string unitName = selectedUnit.unitName;

        UD_UnitDataManager.UnitData unitData = unitDataManager.GetUnitData(unitID);


        if (unitData == null)
        {
            Debug.LogError($"'{unitName}' 유닛데이터 없음");
            return;
        }

        levelText.text = unitData.Level.ToString() + "티어" ;
        nameText.text = unitData.Name; 
        gSkillText.text = unitData.g_SkillName; 
        sSkillText.text = unitData.s_SkillName; 

        curUnitHp = selectedUnit.HP; 
        hpText.text = $"{curUnitHp} / {unitData.Hp}"; 

        defeneTypeText.text = unitData.DefenseType; 

       // Debug.Log($"'{unitName}'업데이트 성공");

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

        currentSelectedUnitOptionBox = Instantiate(slectedUnitOptionBox) as GameObject;

        SetSelectedUnit(unit);

        GameObject canvas = GameObject.Find("Canvas");
        currentSelectedUnitOptionBox.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = currentSelectedUnitOptionBox.GetComponent<RectTransform>();
        screenPos.x += 140;
        screenPos.y -= 90;

        rectTransform.position = screenPos;

        // 모드 전환 버튼
        unitStateChageBtn = currentSelectedUnitOptionBox.transform.Find("ChangeStateBtn").GetComponent<Button>();
        // 업그레이드 버튼
        unitUpgradeBtn = currentSelectedUnitOptionBox.transform.Find("UnitUpgradeBtn").GetComponent<Button>();

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
            unitUpgradeBtn.onClick.RemoveAllListeners();
            unitUpgradeBtn.onClick.AddListener(() =>
            {
                if (currentSelectedUnitOptionBox != null)
                {
                    Destroy(currentSelectedUnitOptionBox);
                    currentSelectedUnitOptionBox = null;
                }


                CreateUpgradeMenu(unit);
            });
        }


    }

    private void CreateUpgradeMenu(UD_Ingame_UnitCtrl unit)
    {
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("Canvas를 찾을 수 없습니다.");
            return;
        }

        currentUpgradeMenu = Instantiate(unitUpgradeMenuBox, canvas.transform);

        RectTransform rectTransform = currentUpgradeMenu.GetComponent<RectTransform>();
        Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);
        screenPos.x += 300;
        rectTransform.position = screenPos;

        UnitUpgrade1Btn = currentUpgradeMenu.transform.Find("UnitUpgrade1Btn").GetComponent<Button>();
        Image upGrade1BtnImage = UnitUpgrade1Btn.GetComponent<Image>();

        UnitUpgrade2Btn = currentUpgradeMenu.transform.Find("UnitUpgrade2Btn").GetComponent<Button>();
        Image upGrade2BtnImage = UnitUpgrade2Btn.GetComponent<Image>();

        // 업그레이드 옵션 가져오기
        List<string> upgradeOptions = unitUpgradeManager.GetUpgradeOptions(unit.ID);

        if (upgradeOptions == null || upgradeOptions.Count == 0)
        {
            Debug.LogError("업그레이드 옵션이 없습니다.");
            Destroy(currentUpgradeMenu);
            currentUpgradeMenu = null;
            return;
        }

        // 3티어인 경우 버튼 하나만 생성
        if (unit.curLevel == 3)
        {
            UnitUpgrade2Btn.gameObject.SetActive(false);

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x - 80, rectTransform.sizeDelta.y);

            RectTransform unitUpgrade1Rect = UnitUpgrade1Btn.GetComponent<RectTransform>();
            unitUpgrade1Rect.anchoredPosition = new Vector2(unitUpgrade1Rect.anchoredPosition.x + 60, unitUpgrade1Rect.anchoredPosition.y);
        }

        // 첫 번째 업그레이드 옵션 버튼 
        UnitUpgrade1Btn.onClick.RemoveAllListeners();
        UnitUpgrade1Btn.onClick.AddListener(() =>
        {
            if (upgradeOptions.Count > 0)
            {
                unitUpgradeManager.PerformUpgrade(unit, upgradeOptions[0]);
            }
            Destroy(currentUpgradeMenu);
            currentUpgradeMenu = null;
        });

        // 두 번째 업그레이드 옵션 버튼 
        UnitUpgrade2Btn.onClick.RemoveAllListeners();
        UnitUpgrade2Btn.onClick.AddListener(() =>
        {
            if (upgradeOptions.Count > 1)
            {
                unitUpgradeManager.PerformUpgrade(unit, upgradeOptions[1]);
            }
            Destroy(currentUpgradeMenu);
            currentUpgradeMenu = null;
        });

    }

    




    public void DestroyUnitStateChangeBox() 
    {
        if (currentSelectedUnitOptionBox != null)
        {
            Destroy(currentSelectedUnitOptionBox);

            currentSelectedUnitOptionBox = null;
        }
    }

    public void DestroyUnitUpgradeMenu()
    {
        if (currentUpgradeMenu != null)
        {
            Destroy(currentUpgradeMenu);

            currentUpgradeMenu = null;
        }
    }

    public void SetSelectedUnit(UD_Ingame_UnitCtrl unit)
    {
        selectedUnit = unit;
    }

    public void UnitStateChange(UD_Ingame_UnitCtrl unit)
    {
        if (unit != null && unit.isSelected)
        {
           // unit.ChangeAllyMode(); 
        }
    }

    public void ShowMoveUI(GameObject unit, bool show)
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
                rectTransform.sizeDelta = new Vector2(1.5f, 1.5f);

                //rectTransform.position = unit.transform.position + new Vector3(0, 3.0f, 0);
                //rectTransform.rotation = Quaternion.Euler(new Vector3(60, 0, 0));
                //rectTransform.rotation = Quaternion.identity;
            }
        }

        if (unitMoveUI != null)
        {
            unitMoveUI.SetActive(show);
        }
    }



    //void ShowWaveResultPanel()
    //{
    //    if(isCurrentWaveFinshed == true)
    //    {
    //        waveResultPanel.gameObject.SetActive(true);

    //        if(isCurrentWaveSucceed == true)
    //        {
    //            waveResultText.text = "전투에 승리했습니다.";
    //        }
    //        else
    //        {
    //            waveResultText.text = "전투에 패배했습니다.";
    //        }


    //    }
    //}

    //void LookUIMainCamera(GameObject unit)
    //{
    //    Transform unitCanvas = unit.transform.Find("Canvas");

    //    if (unitCanvas != null)
    //    {
    //        // 카메라의 회전과 상관없이 캔버스가 카메라를 항상 바라보게 함
    //        unitCanvas.rotation = Quaternion.LookRotation(unitCanvas.position - mainCamera.transform.position);

    //        Debug.Log("Canvas is now facing the camera.");
    //    }
    //}
}