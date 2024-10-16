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
using DG.Tweening.Core.Easing;
using System.Threading.Tasks;

public class Ingame_UIManager : MonoBehaviour
{
    public static Ingame_UIManager instance;

    private UnitSpawnManager unitSpawnManager;
    private UnitDataManager unitDataManager;
    private GameOrderSystem gameOrderSystem;
    private InGameManager inGameManager;

    private Camera mainCamera;

    [Header("====Test UI====")]
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

    [Header("====Game Option====")]
    public Button endGameBtn;
    public Button restartGameBtn;
    public Button pauseGameBtn;
    private bool isPasue = false;

    [Header("====UnitOptionMenu====")]
    public GameObject slectedUnitOptionBox;         // 업그레이드 and 모드 변경 판넬
    public GameObject currentSelectedUnitOptionBox; // 프리펩으로 생성될 판넬
    public Button unitStateChageBtn;                // 유닛 모드 변경 버튼
    public Sprite FreeModeImage;                    // 모드 변경 버튼 Free
    public Sprite SiegeModeImage;                   // 모드 변경 버튼 Siege
    public GameObject unitUpgradeMenuBox;           // 유닛 업그레이드 메뉴 판넬
    public GameObject currentUpgradeMenu;           // 프리펩으로 생성될 업그레이드 판넬
    public Button UnitUpgrade1Btn;
    public Button UnitUpgrade2Btn;

    [Header("====Wave Menu UI====")]
    public float waveCount = 20;
    public Text waveCountText;
    public Text waveStartText;
    public GameObject waveStartPanel;
    public GameObject waveResultPanel;
    public Image waveResultImage;
    public Text waveResultText;
    public bool isCurrentWaveSucceed;
    public Button nextWavBtn;
    public Button waveCheckBtn;
    public Button waveSkipBtn;
    public bool isCountDownIng = false;
    public Button waveRestartBtn = null;
    public Button lobbybtn = null;
    public GameObject waveStepSuccessPanel;



    public Sprite waveWinImage;
    public Sprite waveLoseImage;

    [Header("====Game Setting UI====")]
    public Button settingBtn;
    public Button settingCloseBtn;
    public GameObject settingPanel;

    [Header("====Unit GamoeObject====")]
    public GameObject UnitStateChangeBox;
    public GameObject currentUnitStateChangeBox;
    public GameObject unitMoveUI;

    [Header("====Castle HP====")]
    public Image baseHpBar;
    public Text baseHpTxt;

    public int curHaveGold;
    public int curUnitHp;


    private AllyMode allyMode;

    private Ingame_UnitCtrl selectedUnit;
    public GameObject unit;

    public Button unitUpgradeBtn;

    // 곧 지울거
    public Button commanderSkillBtn;


    private void Awake()
    {
        instance = this;
        unitSpawnManager = UnitSpawnManager.inst;
        unitSpawnBtn = unitSpawnPanel.GetComponentsInChildren<Button>();

        GameObject upgradeManagerObj = GameObject.Find("UD_Ingame_UnitUpgradeManager");

    }

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;

        unitDataManager = UnitDataManager.inst;
        inGameManager = InGameManager.inst;
        gameOrderSystem = GetComponent<GameOrderSystem>();



        // 저장된 커맨더 스킬 확인 용
        if (commanderSkillBtn != null)
        {
            commanderSkillBtn.onClick.AddListener(() =>
            {
                Dictionary<string, string> savedSkills = inGameManager.LoadCommandSkillList();

                foreach (var skill in savedSkills)
                {
                    Debug.Log($"이름: {skill.Value}");
                }
            });
        }


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
                if (waveCheckBtn != null)
                {
                    waveCheckBtn.onClick.AddListener(() =>
                    {
                        waveResultPanel.gameObject.SetActive(false);
                        Time.timeScale = 1.0f;
                    });
                }
                //EnemySpawner.inst.NextWave();
            });
        }


        if (settingBtn != null)
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


        // 웨이브 카운트 스킵
        if (waveSkipBtn != null)
        {
            waveSkipBtn.onClick.AddListener(() =>
            {
                waveCount = 0; // 버튼 클릭 시 카운트를 0으로 설정
                waveCountText.text = "적군 침공까지 0초"; // 즉시 0으로 카운트 표시
                //StartCoroutine(EnemySpawner.inst.StartWaveWithDelay(1f)); // 바로 1초 후 웨이브 시작
                waveCountText.gameObject.SetActive(false);
            });

        }


    }


    // Update is called once per frame
    void Update()
    {
        // 웨이브 카운트 다운 텍스트
        if (waveCountText != null && isCountDownIng) // 웨이브가 진행중이 아닐 때
        {
            waveCount -= Time.deltaTime;
            if (waveCount >= 0 && isCountDownIng)
            {
                waveCountText.gameObject.SetActive(true);
                waveCountText.text = "적군 침공까지 " + Mathf.Ceil(waveCount).ToString() + "초";

            }
            else if (waveCount < 0)
            {
                waveStartText.text = EnemySpawner.inst.currentWave.ToString() + "차 침공 시작";
                isCountDownIng = false;
                EnemySpawner.inst.isWaveing = true;
                waveCountText.gameObject.SetActive(false);
                StartCoroutine(EnemySpawner.inst.StartWaveWithDelay(1f)); // 1초의 지연 후 웨이브 시작
                waveCount = 20;
            }
        }

        if (BaseStatus.instance.BaseHPCur <= 0)
        {
            waveResultImage.sprite = waveLoseImage;
        }

        if (UnitSetModeText != null)
        {
            if (InGameManager.inst.UnitSetMode)
            {
                if (InGameManager.inst.AllyUnitSetMode)

                {
                    UnitSetModeText.color = Color.white;
                    UnitSetModeText.text = "UnitSetMode : ON";
                }
                else if (InGameManager.inst.EnemyUnitSetMode)
                {
                    UnitSetModeText.color = Color.red;
                    UnitSetModeText.text = "EnemySetMode : ON";
                }
            }
            else
            {
                UnitSetModeText.color = Color.black;
                UnitSetModeText.text = "SetModeOff";
            }
        }

        BaseHP();

        if (selectedUnit != null)
        {
            unitInfoPanel.SetActive(true);
        }
        else
        {
            unitInfoPanel.SetActive(false);
        }

        // UI 유닛 따라가기
        if (selectedUnit != null && currentSelectedUnitOptionBox != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(selectedUnit.transform.position);
            screenPos.x += 100;
            screenPos.y -= 50;

            currentSelectedUnitOptionBox.transform.position = screenPos;
        }


        if (waveStartText.gameObject.activeSelf == true)
        {
            float coolTime = 3.0f;

            coolTime -= Time.deltaTime;

            if (coolTime < 0)
            {
                waveStartText.gameObject.SetActive(false);
            }
        }
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

    public void UpdateUnitInfoPanel(Ingame_UnitCtrl selectedUnit, string newUnitCode)
    {
        if (selectedUnit == null)
        {
            Debug.LogError("선택된 유닛이 null입니다.");
            return;
        }

        // UnitDataManager에서 새로운 유닛 데이터를 불러옴
        Ingame_UnitCtrl unitData = selectedUnit;

        if (unitData == null)
        {
            Debug.LogError(" 유닛 데이터 없음");
            return;
        }

        // 유닛의 이름, 레벨, HP, 스킬 등 정보를 UI에 업데이트
        levelText.text = $"{selectedUnit.level} 티어";
        nameText.text = selectedUnit.name;
        gSkillText.text = selectedUnit.gSkillName;
        sSkillText.text = selectedUnit.sSkillName;

        // HP 정보 업데이트
        curUnitHp = selectedUnit.HP;
        hpText.text = $"{selectedUnit.HP} / {selectedUnit.HP}";

        // 방어 타입 정보 업데이트
        defeneTypeText.text = selectedUnit.defenstype;
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
        List<string> upgradeOptions = UnitUpgradeManager.Instance.GetUpgradeOptions(unit.unitCode);

        if (upgradeOptions == null || upgradeOptions.Count == 0)
        {
            Debug.LogError("업그레이드 가능한 옵션이 없습니다.");
            Destroy(currentUpgradeMenu);
            currentUpgradeMenu = null;
            return;
        }

        // 3티어인 경우 버튼 하나만 생성
        if (unit.level == 3)
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
                // 업그레이드 수행
                UnitUpgradeManager.Instance.PerformUpgrade(unit, upgradeOptions[0]);
            }
            Destroy(currentUpgradeMenu);
            currentUpgradeMenu = null;
        });

        // 두 번째 업그레이드 옵션 버튼 (2개 옵션이 있는 경우만)
        UnitUpgrade2Btn.onClick.RemoveAllListeners();
        UnitUpgrade2Btn.onClick.AddListener(() =>
        {
            if (upgradeOptions.Count > 1)
            {
                // 두 번째 업그레이드 수행
                UnitUpgradeManager.Instance.PerformUpgrade(unit, upgradeOptions[1]);
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
                rectTransform.sizeDelta = new Vector2(1.0f, 1.0f);

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





    // Base HP 연출
    void BaseHP()
    {
        if (baseHpBar != null)
        {
            baseHpBar.fillAmount = (float)BaseStatus.instance.BaseHPCur / (float)BaseStatus.instance.BaseHPMax;
            baseHpTxt.text = BaseStatus.instance.BaseHPCur + " / " + BaseStatus.instance.BaseHPMax;
        }
    }

    public void ShowUI(GameObject uiElement, float duration)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(true);  // UI 요소 활성화
            StartCoroutine(HideUIAfterDelay(uiElement, duration));  // 일정 시간 후 비활성화
        }
    }

    // 일정 시간이 지나면 UI 요소를 비활성화하는 코루틴
    IEnumerator HideUIAfterDelay(GameObject uiElement, float delay)
    {
        yield return new WaitForSeconds(delay);  // 지정된 시간만큼 대기
        uiElement.SetActive(false);  // UI 요소 비활성화
    }
}