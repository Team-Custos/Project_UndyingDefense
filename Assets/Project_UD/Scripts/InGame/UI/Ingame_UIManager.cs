using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using static UnityEngine.UI.CanvasScaler;
using static UnitExcelDataManager;
using UnityEngine.Rendering.Universal;
using DG.Tweening.Core.Easing;
using System.Threading.Tasks;
using Unity.Burst.CompilerServices;

//이 스크립트는 인게임의 전반적인 UI를 관리하기 위한 스크립트입니다.

public class Ingame_UIManager : MonoBehaviour
{
    public static Ingame_UIManager instance;

    private UnitSpawnManager unitSpawnManager;
    private InGameManager inGameManager;

    private Camera mainCamera;

    [Header("====Test UI====")]
    public Button EnemyTestModeBtn = null;
    public Text UnitSetModeText = null;

    [Header("====UnitSpawnDeck====")]
    public GameObject unitSpawnPanel;
    public Button[] unitSpawnBtn;
    public GameObject[] unitSpawnBtnPanel;
    private int currentSelectedIndex = -1; // 현재 선택된 버튼의 인덱스 (-1은 선택 없음 의미)

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
    public Sprite enemyArcherImage;
    public Sprite enenmyWarriorImage;
    public Sprite minByeongImage;
    public Sprite hunterImage;
    
    [Header("====Game Option====")]
    public Button endGameBtn;
    public Button restartGameBtn;
    public Button pauseGameBtn;

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


    [Header("====Game Setting UI====")]
    public Button settingBtn;
    public Button settingCloseBtn;
    public GameObject settingPanel;
    public Button settingReStartBtn;
    public Button settingLobbyBtn;

    [Header("====Unit GamoeObject====")]
    public GameObject UnitStateChangeBox;
    public GameObject currentUnitStateChangeBox;
    public GameObject unitMoveUI;

    [Header("====Castle HP====")]
    public Image baseHpBar;
    public Text baseHpTxt;

    public Text goldTxt;

    // 소환 버튼 선택 효과 이미지 (추후 파티클로 대체)
    public Image[] selectedBtnEffectImage;


    private Ingame_UnitCtrl selectedUnit;

    public Button unitUpgradeBtn;

    private Transform clickUITransform;
    public GameObject clickUI;

    // 곧 지울거
    public Button commanderSkillBtn;



    private void Awake()
    {
        instance = this;
        unitSpawnManager = UnitSpawnManager.inst;
        unitSpawnBtn = unitSpawnPanel.GetComponentsInChildren<Button>();

    }

    // Start is called before the first frame update
    void Start()
    {

        mainCamera = Camera.main;

        inGameManager = InGameManager.inst;


        if(GameOrderSystem.instance.selectedUnit != null)
        {
            clickUITransform = GameOrderSystem.instance.selectedUnit.transform.Find("Canvas/ClickUI");
            clickUI = clickUITransform.gameObject;
        }


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
                int idx = ii; // 지역 변수를 사용해 버튼 인덱스를 유지

                unitSpawnBtn[idx].onClick.AddListener(() =>
                {
                    if(InGameManager.inst.UnitSetMode && InGameManager.inst.AllyUnitSetMode)
                    {
                        //유닛 생성모드일때 다른 버튼을 누르면 그 버튼에 해당되는 유닛 생성
                    }

                    // 유닛 스폰 로직
                    UnitSpawnManager.inst.unitToSpawn = unitSpawnBtn[idx].GetComponent<Ingame_UnitSpawnBtnStatus>().UnitCode;
                    InGameManager.inst.UnitSetMode = !InGameManager.inst.UnitSetMode; 
                    InGameManager.inst.AllyUnitSetMode = !InGameManager.inst.AllyUnitSetMode;

                    // 이미지 상태 변경 로직
                    UpdateButtonEffect(idx);

                    // 기타 로직
                    DestroyUnitStateChangeBox();
                });
            }
        }


        //for (int ii = 0; ii < unitSpawnBtn.Length; ii++)
        //{
        //    if (unitSpawnBtn[ii] != null)
        //    {
        //        int idx = ii;
        //        unitSpawnBtn[idx].onClick.AddListener(() =>
        //        {
        //            UnitSpawnManager.inst.unitToSpawn = unitSpawnBtn[idx].GetComponent<Ingame_UnitSpawnBtnStatus>().UnitCode;
        //            InGameManager.inst.UnitSetMode = !InGameManager.inst.UnitSetMode;
        //            InGameManager.inst.AllyUnitSetMode = !InGameManager.inst.AllyUnitSetMode;

        //            DestroyUnitStateChangeBox();
        //        });
        //    }
        //}

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

        


        if (settingBtn != null)
        {
            settingBtn.onClick.AddListener(() =>
            {
                settingPanel.SetActive(true);
                InGameManager.inst.isGamePause = true;
            });
        }

        if (settingCloseBtn != null)
        {
            settingCloseBtn.onClick.AddListener(() =>
            {
                settingPanel.SetActive(false);
                InGameManager.inst.isGamePause = false;
            });
        }



        unitSpawnBtn[2].interactable = false;
        unitSpawnBtn[3].interactable = false;
    }


    // Update is called once per frame
    void Update()
    {
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
            screenPos.y -= 20;

            currentSelectedUnitOptionBox.transform.position = screenPos;
        }






        if (!InGameManager.inst.UnitSetMode && !InGameManager.inst.AllyUnitSetMode)
        {
            // 현재 사용되는 버튼이 두개라서 for문 사용 안함
            selectedBtnEffectImage[0].gameObject.SetActive(false);
            selectedBtnEffectImage[1].gameObject.SetActive(false);
        }


        goldTxt.text = InGameManager.inst.gold.ToString();
    }



    // 
    void EnterUnitSpawnMode(int idx)
    {
        // 선택된 버튼 인덱스를 현재 인덱스로 설정
        currentSelectedIndex = idx;

        // 유닛 스폰 설정
        UnitSpawnManager.inst.unitToSpawn = unitSpawnBtn[idx].GetComponent<Ingame_UnitSpawnBtnStatus>().UnitCode;
        InGameManager.inst.UnitSetMode = true;
        InGameManager.inst.AllyUnitSetMode = true;

        DestroyUnitStateChangeBox();
    }

    // 유닛 생성 상태에서 동일한 버튼을 누르면 상태해제를 위한 함수
    void ExitUnitSpawnMode()
    {
        InGameManager.inst.UnitSetMode = false;
        InGameManager.inst.AllyUnitSetMode = false;

        // 선택된 버튼 인덱스를 초기화
        currentSelectedIndex = -1;

        // 유닛 대기 모드 해제
        //InGameManager.inst.UnitSetMode = false;
        //InGameManager.inst.AllyUnitSetMode = false;

        DestroyUnitStateChangeBox();
    }

    public void SetSelectedUnit(Ingame_UnitCtrl unit)
    {
        selectedUnit = unit;
    }


    // 해당 인덱스에 맞는 이미지를 켜고 다른 이미지는 끄는 메서드
    void UpdateButtonEffect(int activeIndex)
    {
        // 두 옵션이 모두 false인 경우 모든 이미지를 비활성화
        if (InGameManager.inst.UnitSetMode && InGameManager.inst.AllyUnitSetMode)
        {
            // activeIndex와 일치하는 인덱스의 이미지만 켜고 나머지는 끔
            for (int i = 0; i < selectedBtnEffectImage.Length; i++)
            {
                selectedBtnEffectImage[i].gameObject.SetActive(i == activeIndex);
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

    public void UpdateUnitInfoPanel(Ingame_UnitCtrl selectedUnit)
    {
        if (selectedUnit == null)
        {
            return;
        }

        Ingame_UnitCtrl unitData = selectedUnit;

        if (unitData == null)
        {
            return;
        }


        // 유닛의 이름, 레벨, HP, 스킬 등 정보를 UI에 업데이트
        levelText.text = selectedUnit.unitData.level + "티어";
        nameText.text = selectedUnit.unitData.name;
        gSkillText.text = selectedUnit.unitData.g_SkillName;
        sSkillText.text = selectedUnit.unitData.s_SkillName;
        defeneTypeText.text = selectedUnit.unitData.defenseType.ToString();
        gSkillInfoText.text = selectedUnit.unitData.g_SkillInfo;
        sSkillInfoText.text = selectedUnit.unitData.s_SkillInfo;

        if(selectedUnit.unitData.unitCode == "1")
        {
            unitInfoImage.sprite = minByeongImage;
        }
        else if(selectedUnit.unitData.unitCode == "2")
        {
            unitInfoImage.sprite = hunterImage;
        }
        else if(selectedUnit.unitData.unitCode == "80")
        {
            unitInfoImage.sprite = enenmyWarriorImage;
        }
        else if(selectedUnit.unitData.unitCode == "81")
        {
            unitInfoImage.sprite = enemyArcherImage;
        }

        if (selectedUnit.HP <= 0)
            selectedUnit.HP = 0;

        // HP 정보 업데이트
        //hpText.text = selectedUnit.HP + "/" + unitData.maxHp;
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

        SetSelectedUnit(unit);

        currentSelectedUnitOptionBox = Instantiate(slectedUnitOptionBox) as GameObject;

        GameObject canvas = GameObject.Find("Canvas");
        currentSelectedUnitOptionBox.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = currentSelectedUnitOptionBox.GetComponent<RectTransform>();
        screenPos.x += 100;
        screenPos.y -= 20;

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
                    Ingame_ParticleManager.Instance.PlayUnitModeChangeParticleEffect(unit.transform, -0.8f);

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
            unitUpgradeBtn.interactable = false;

            unitUpgradeBtn.onClick.RemoveAllListeners();
            unitUpgradeBtn.onClick.AddListener(() =>
            {
                if (currentSelectedUnitOptionBox != null)
                {
                    Destroy(currentSelectedUnitOptionBox);
                    currentSelectedUnitOptionBox = null;
                }

                //CreateUpgradeMenu(unit);
            });
        }
    }


    // 유닛의 상태에 따른 버튼 비활성화를 위한 코드 (Move, Chase시)
    //public void SetUnitButtonsInteractable(bool interactable)
    //{
    //    if (unitStateChageBtn != null)
    //        unitStateChageBtn.interactable = interactable;

    //    if (unitUpgradeBtn != null)
    //        unitUpgradeBtn.interactable = interactable;
    //}


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
        List<string> upgradeOptions = UnitUpgradeManager.Instance.GetUpgradeOptions(unit.unitData.unitCode);

        if (upgradeOptions == null || upgradeOptions.Count == 0)
        {
            Debug.LogError("업그레이드 가능한 옵션이 없습니다.");
            Destroy(currentUpgradeMenu);
            currentUpgradeMenu = null;
            return;
        }

        // 3티어인 경우 버튼 하나만 생성
        if (unit.unitData.level == 3)
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




    
    public void ShowUnitMoveUI(GameObject unit, bool move)
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

        unitMoveImage.SetActive(move);
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
        if (InGameManager.inst.isGamePause == false)
        {
            InGameManager.inst.isGamePause = true;
        }
        else
        {
            InGameManager.inst.isGamePause = false;
        }

    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}