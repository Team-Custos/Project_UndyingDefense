using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using static UnityEngine.UI.CanvasScaler;
using static UnitDataManager;
using UnityEngine.Rendering.Universal;

public class Ingame_UIManager : MonoBehaviour
{
    public static Ingame_UIManager instance;

    UnitSpawnManager unitSpawnManager;

    public Button EnemyTestModeBtn = null;
    public Text UnitSetModeText = null;

    [Header("====UnitSpawnDeck====")]
    public GameObject unitSpawnPanel;
    public Button[] unitSpawnBtn;
    public GameObject[] unitSpawnBtnPanel;

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
    public Text gSkillInfoText;
    public Text sSkillInfoText;


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
    public Button nextWavBtn;
    public Button waveCheckBtn;

    int unitLevel;

    public Text curGoldText;
    public int curHaveGold;


    public int curUnitHp;



    public GameObject UnitStateChangeBox;
    public GameObject currentUnitStateChangeBox;

    private bool isPasue = false;

    private AllyMode allyMode;

    private Ingame_UnitCtrl selectedUnit;
    public GameObject unit;


    Camera mainCamera;


    public Button unitUpgradeBtn;


    public Image unitMoveImage;
    public GameObject unitMoveUI;

    private UnitDataManager unitDataManager;
    private UnitUpgradeManager unitUpgradeManager;
    private GameOrderSystem gameOrderSystem;


    [Header("====Game Setting====")]
    public Button settingBtn;
    public Button settingCloseBtn;
    public GameObject settingPanel;



    private void Awake()
    {
        instance = this;
        unitSpawnManager = UnitSpawnManager.inst;
        unitSpawnBtn = unitSpawnPanel.GetComponentsInChildren<Button>();

        GameObject upgradeManagerObj = GameObject.Find("UD_Ingame_UnitUpgradeManager");
        if (upgradeManagerObj != null)
        {
            unitUpgradeManager = upgradeManagerObj.GetComponent<UnitUpgradeManager>();
        }

        // 같은 오브젝트에 할당된 경우
        if (unitUpgradeManager == null)
        {
            unitUpgradeManager = GetComponent<UnitUpgradeManager>();
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

        unitDataManager = UnitDataManager.inst;

        gameOrderSystem = GetComponent<GameOrderSystem>();

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

                    DestroyUnitStateChangeBox();
                    

                });
            }
        }

        if (EnemyTestModeBtn != null)
        {
            EnemyTestModeBtn.onClick.AddListener(() =>
            {
                InGameManager.inst.UnitSetMode = !InGameManager.inst.UnitSetMode;
                InGameManager.inst.EnemyUnitSetMode = !InGameManager.inst.EnemyUnitSetMode;

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

        if (nextWavBtn != null)
        {
            nextWavBtn.onClick.AddListener(() =>
            {
                Time.timeScale = 0.0f;
                waveResultPanel.gameObject.SetActive(true);
                if (waveCheckBtn !=null)
                {
                    waveCheckBtn.onClick.AddListener(() =>
                    {
                        waveResultPanel.gameObject.SetActive(false);
                        Time.timeScale = 1.0f;
                    });
                }
                EnemySpawner.inst.NextWave();
            });
        }


        if(settingBtn != null)
        {
            settingBtn.onClick.AddListener(() =>
            {
                settingPanel.gameObject.SetActive(true);
            });
        }

        if (settingCloseBtn != null)
        {
            settingCloseBtn.onClick.AddListener(() =>
            {
                settingPanel.gameObject.SetActive(false);
            });
        }

        curHaveGold = 10000000;

        waveCount = 20;

        isCurrentWaveFinshed = true;
    }


    public void UpdateSpawnButtonState(int index)
    {
        if (index >= 0 && index < unitSpawnBtn.Length)
        {
            unitSpawnBtn[index].interactable = false; // 버튼 비활성화
            if (unitSpawnBtnPanel[index] != null)
            {
                unitSpawnBtnPanel[index].SetActive(true); // 패널 활성화
                StartCoroutine(UnitSpawnCoolTime(index)); // 코루틴 실행
            }
        }
    }

    private IEnumerator UnitSpawnCoolTime(int index)
    {
        GameObject panel = unitSpawnBtnPanel[index];
        RectTransform panelRect = panel.GetComponent<RectTransform>();

        Vector2 originalSize = panelRect.sizeDelta;
        Vector2 originalPosition = panelRect.anchoredPosition;

        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchorMin = new Vector2(0.5f, 0f);
        panelRect.anchorMax = new Vector2(0.5f, 0f);

        float duration = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            float newHeight = Mathf.Lerp(originalSize.y, 0, progress);
            panelRect.sizeDelta = new Vector2(originalSize.x, newHeight);

            yield return null;
        }

        panel.SetActive(false);
        panelRect.sizeDelta = originalSize;
        panelRect.anchoredPosition = originalPosition; 

        unitSpawnBtn[index].interactable = true;
    }

    public int GetButtonIndexByUnitCode(int unitCode)
    {
        for (int i = 0; i < unitSpawnBtn.Length; i++)
        {
            if (unitSpawnBtn[i].GetComponent<Ingame_UnitSpawnBtnStatus>().UnitCode == unitCode)
            {
                return i;
            }
        }
        return -1;
    }

    // Update is called once per frame
    void Update()
    {
        // 웨이브 카운트 다운 텍스트
        if (waveCountText != null && isCurrentWaveFinshed)
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
                EnemySpawner.inst.StartCoroutine(EnemySpawner.inst.WaveSystem());
                isCurrentWaveFinshed = false;
                //ShowWaveResultPanel();
            }
        }

        //curGoldText.text = curHaveGold.ToString();

        if (UnitSetModeText != null)
        {
            if (InGameManager.inst.UnitSetMode)
            {


                if (InGameManager.inst.AllyUnitSetMode)
                {
                    UnitSetModeText.color = Color.white;
                    UnitSetModeText.text = "UnitSetMode";
                }
                else if (InGameManager.inst.EnemyUnitSetMode)
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
            screenPos.x += 100;
            screenPos.y -= 50;

            currentSelectedUnitOptionBox.transform.position = screenPos;
        }

        //LookUIMainCamera(unit);

        if(waveStartText.gameObject.activeSelf == true)
        {
            float coolTime = 3.0f;

            coolTime -= Time.deltaTime;

            if(coolTime < 0)
            {
                waveStartText.gameObject.SetActive(true);
            }
        }
    }


    public void UpdateUnitInfoPanel(Ingame_UnitCtrl selectedUnit, string newUnitCode)
    {
        if (selectedUnit == null)
        {
            Debug.LogError("선택된 유닛이 null입니다.");
            return;
        }

        //string unitCode = selectedUnit.unitCode;
        string unitName = selectedUnit.unitName;

        //UD_UnitDataManager.UnitData unitData = unitDataManager.GetUnitData(unitCode);
        UnitSpawnData newUnitData = UnitSpawnManager.inst.GetUnitSpawnData(newUnitCode);

        if (newUnitData == null)
        {
            Debug.LogError($"'{unitName}' 유닛데이터 없음");
            return;
        }

        levelText.text = newUnitData.level + "티어" ;
        nameText.text = newUnitData.unitName;
        gSkillText.text = newUnitData.gSkillName;
        sSkillText.text = newUnitData.sSkillName; 
        gSkillInfoText.text = newUnitData.gSkillName;
        sSkillInfoText.text = newUnitData.sSkillName;


        //curUnitHp = selectedUnit.HP; 
        hpText.text = $"{newUnitData.HP} / {newUnitData.HP}";

        defeneTypeText.text = newUnitData.defenseType.ToString();

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

    public void CreateSeletedUnitdOptionBox(Vector3 worldPosition, Ingame_UnitCtrl unit)
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
        screenPos.x += 100;
        screenPos.y -= 50;

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
                if (unit.isSelected)
                {
                    unit.previousAllyMode = unit.Ally_Mode;
                    unit.Ally_Mode = AllyMode.Change;

                    DestroyUnitStateChangeBox();
                }
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

    private void CreateUpgradeMenu(Ingame_UnitCtrl unit)
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
        screenPos.x += 200;
        rectTransform.position = screenPos;

        UnitUpgrade1Btn = currentUpgradeMenu.transform.Find("UnitUpgrade1Btn").GetComponent<Button>();
        Image upGrade1BtnImage = UnitUpgrade1Btn.GetComponent<Image>();

        UnitUpgrade2Btn = currentUpgradeMenu.transform.Find("UnitUpgrade2Btn").GetComponent<Button>();
        Image upGrade2BtnImage = UnitUpgrade2Btn.GetComponent<Image>();

        // 업그레이드 옵션 가져오기
        List<string> upgradeOptions = unitUpgradeManager.GetUpgradeOptions(unit.unitCode);

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

    public void SetSelectedUnit(Ingame_UnitCtrl unit)
    {
        selectedUnit = unit;
    }

    //public void UnitStateChange(UD_Ingame_UnitCtrl unit)
    //{
    //    if (unit != null && unit.isSelected)
    //    {
    //        unit.Ally_Mode = AllyMode.Change;
    //    }
    //}

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



    void ShowWaveResultPanel()
    {
        if (isCurrentWaveFinshed == true)
        {
            waveResultPanel.gameObject.SetActive(true);

            if (isCurrentWaveSucceed == true)
            {
                waveResultText.text = "전투에 승리했습니다.";
            }
            else
            {
                waveResultText.text = "전투에 패배했습니다.";
            }


        }
    }

}