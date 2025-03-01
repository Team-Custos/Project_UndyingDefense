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
    public Text UnitUpgrade1Txt;
    public Button UnitUpgrade2Btn;
    public Text UnitUpgrade2Txt;
    public Button UnitUpgradeCloseeBtn;
    public GameObject unitUpgradeMenuConfirmBox;
    public GameObject currentunitUpgradeMenuConfirmBox;
    public Button UpgradeCheckCloseBtn;
    public Button UpgradeCheckOkBtn;
    public Button UpgradeCheckCancleBtn;


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

    public Button[] inGameCommandSkillBtn;


    private void Awake()
    {
        instance = this;
        inGameManager = InGameManager.inst;
        unitSpawnManager = UnitSpawnManager.inst;
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
                int idx = ii; // 지역 변수를 사용해 버튼 인덱스를 유지

                unitSpawnBtn[idx].onClick.AddListener(() =>
                {
                    // 현재 버튼이 활성화된 상태인지 확인
                    if (UnitSpawnManager.inst.unitToSpawn == unitSpawnBtn[idx].GetComponent<Ingame_UnitSpawnBtnStatus>().currentUnitType
                        && InGameManager.inst.UnitSetMode && InGameManager.inst.AllyUnitSetMode)
                    {
                        // 동일한 버튼을 다시 눌렀다면 소환 상태를 해제
                        InGameManager.inst.UnitSetMode = false;
                        InGameManager.inst.AllyUnitSetMode = false;

                        // 버튼 효과 초기화
                        UpdateButtonEffect((int)UnitType.None); // 모든 버튼 비활성화
                    }
                    else
                    {
                        // 다른 버튼을 눌렀거나 새로운 소환 상태 설정
                        InGameManager.inst.UnitSetMode = true;
                        InGameManager.inst.AllyUnitSetMode = true;

                        // 유닛 스폰 로직
                        UnitSpawnManager.inst.unitToSpawn = unitSpawnBtn[idx].GetComponent<Ingame_UnitSpawnBtnStatus>().currentUnitType;

                        if (idx == 0 || idx == 1)
                        {
                            SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_click);
                        }
                        else
                        {
                            SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_unableClick);
                        }

                        // 유닛 스폰 로직
                        //UnitSpawnManager.inst.unitToSpawn = unitSpawnBtn[idx].GetComponent<Ingame_UnitSpawnBtnStatus>().UnitCode;


                        // 이미지 상태 변경 로직
                        UpdateButtonEffect(idx);

                        DestroyUnitStateChangeBox();
                        DestroyUnitUpgradeMenu();
                        DestorypgradeMenuConfirmBox();
                    }
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

        


        if (settingBtn != null)
        {
            settingBtn.onClick.AddListener(() =>
            {
                SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_click);
                settingPanel.SetActive(true);
                //InGameManager.inst.isGamePause = true;
            });
        }

        if (settingCloseBtn != null)
        {
            settingCloseBtn.onClick.AddListener(() =>
            {
                SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_exit);
                settingPanel.SetActive(false);
                InGameManager.inst.isGamePause = false;
            });
        }

        unitSpawnBtn[2].interactable = false;
        unitSpawnBtn[3].interactable = false;


        if (settingLobbyBtn != null)
        {
            settingLobbyBtn.onClick.AddListener(() =>
            {
                InGameManager.inst.isGamePause = false;
                SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_exit);
                LoadingSceneManager.LoadScene("LobbyScene_LoPol");

            });


        }

        if (settingReStartBtn != null)
        {
            settingReStartBtn.onClick.AddListener(() =>
            {
                InGameManager.inst.isGamePause = false;
                SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_click);
                LoadingSceneManager.LoadScene(SceneManager.GetActiveScene().name);
            });

        }

        for (int i = 0; i < inGameCommandSkillBtn.Length; i++)
        {
            inGameCommandSkillBtn[i].image.sprite = UserDataModel.instance.skillDatas[i].commandSkillImage;

            int index = i;

            inGameCommandSkillBtn[i].onClick.AddListener(() =>
            {
                Debug.Log(UserDataModel.instance.skillDatas[index].commandSkillName);
            });
        }

        
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

        if(selectedUnit != null && currentUpgradeMenu != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(selectedUnit.transform.position);
            screenPos.x += 190;
            screenPos.y += 10;


            currentUpgradeMenu.transform.position = screenPos;
        }


        if (selectedUnit != null && currentunitUpgradeMenuConfirmBox != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(selectedUnit.transform.position);
            screenPos.x += 190;
            screenPos.y += 10;


            currentunitUpgradeMenuConfirmBox.transform.position = screenPos;
        }





        if (!InGameManager.inst.UnitSetMode && !InGameManager.inst.AllyUnitSetMode)
        {
            selectedBtnEffectImage[0].gameObject.SetActive(false);
            selectedBtnEffectImage[1].gameObject.SetActive(false);
        }


        goldTxt.text = InGameManager.inst.gold.ToString();


        if (unitSpawnBtn[0].interactable)
        {
            unitSpawnBtn[0].tag = "InteractiveUi";
        }
        else
        {
            unitSpawnBtn[0].tag = "UnInteractiveUi";
        }

        if (unitSpawnBtn[1].interactable)
        {
            unitSpawnBtn[1].tag = "InteractiveUi";
        }
        else
        {
            unitSpawnBtn[1].tag = "UnInteractiveUi";
        }
    }




    //void EnterUnitSpawnMode(int idx)
    //{
    //     선택된 버튼 인덱스를 현재 인덱스로 설정
    //    currentSelectedIndex = idx;

    //     유닛 스폰 설정
    //    UnitSpawnManager.inst.unitToSpawn = unitSpawnBtn[idx].GetComponent<Ingame_UnitSpawnBtnStatus>().UnitCode;
    //    InGameManager.inst.UnitSetMode = true;
    //    InGameManager.inst.AllyUnitSetMode = true;

    //    DestroyUnitStateChangeBox();
    //}

    // 유닛 생성 상태에서 동일한 버튼을 누르면 상태해제를 위한 함수
    //void ExitUnitSpawnMode()
    //{
    //    InGameManager.inst.UnitSetMode = false;
    //    InGameManager.inst.AllyUnitSetMode = false;

    //     선택된 버튼 인덱스를 초기화
    //    currentSelectedIndex = -1;

    //     유닛 대기 모드 해제
    //    InGameManager.inst.UnitSetMode = false;
    //    InGameManager.inst.AllyUnitSetMode = false;

    //    DestroyUnitStateChangeBox();
    //}

    public void SetSelectedUnit(Ingame_UnitCtrl unit)
    {
        selectedUnit = unit;
    }


    // 해당 인덱스에 맞는 이미지를 켜고 다른 이미지는 끄는 메서드
    public void UpdateButtonEffect(int activeIndex)
    {
        for (int i = 0; i < selectedBtnEffectImage.Length; i++)
        {
            // activeIndex가 -1이면 모든 버튼 비활성화, 아니면 선택된 버튼만 활성화
            selectedBtnEffectImage[i].gameObject.SetActive(i == activeIndex);
        }

        // 두 옵션이 모두 false인 경우 모든 이미지를 비활성화
        //if (InGameManager.inst.UnitSetMode && InGameManager.inst.AllyUnitSetMode)
        //{
        //    // activeIndex와 일치하는 인덱스의 이미지만 켜고 나머지는 끔
        //    for (int i = 0; i < selectedBtnEffectImage.Length; i++)
        //    {
        //        selectedBtnEffectImage[i].gameObject.SetActive(i == activeIndex);
        //    }
        //}
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
        unitInfoImage.sprite = selectedUnit.unitData.unitImage;
        levelText.text = selectedUnit.unitData.level + "티어";
        nameText.text = selectedUnit.unitData.name;
        gSkillText.text = selectedUnit.unitData.g_SkillName;
        sSkillText.text = selectedUnit.unitData.s_SkillName;
        defeneTypeText.text = selectedUnit.unitData.defenseType.ToString();
        gSkillInfoText.text = selectedUnit.unitData.g_SkillInfo;
        sSkillInfoText.text = selectedUnit.unitData.s_SkillInfo;
        gSkillImage.sprite = selectedUnit.unitData.g_SkillImage;

        if(selectedUnit.CompareTag(CONSTANT.TAG_UNIT))
        {
            sSkillImage.gameObject.SetActive(true);
            sSkillImage.sprite = selectedUnit.unitData.s_SkillImage;
        }
        else
        {
            sSkillImage.gameObject.SetActive(false);
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

        if (Ingame_UIManager.instance.currentUpgradeMenu != null)
        {
            Ingame_UIManager.instance.DestroyUnitUpgradeMenu();
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

        //if (unit.unitData.level >= 2)
        //{
        //    unitUpgradeBtn.interactable = false;
        //}
        //else
        //{
        //    unitUpgradeBtn.interactable = true;
        //}

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


    // 유닛의 상태에 따른 버튼 비활성화를 위한 코드 (Move, Chase시)
    //public void SetUnitButtonsInteractable(bool interactable)
    //{
    //    if (unitStateChageBtn != null)
    //        unitStateChageBtn.interactable = interactable;

    //    if (unitUpgradeBtn != null)
    //        unitUpgradeBtn.interactable = interactable;
    //}


    public string upgradeOption;

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
        upGrade1BtnImage.sprite = unit.unitData.uupgrade1Image;

        UnitUpgrade2Btn = currentUpgradeMenu.transform.Find("UnitUpgrade2Btn").GetComponent<Button>();
        Image upGrade2BtnImage = UnitUpgrade2Btn.GetComponent<Image>();
        upGrade2BtnImage.sprite = unit.unitData.uupgrade2Image;

        UnitUpgrade1Txt = currentUpgradeMenu.transform.Find("UnitUpgrade1Txt").GetComponent<Text>();
        UnitUpgrade1Txt.text = unit.unitData.upgrade1Cost.ToString();
        UnitUpgrade2Txt = currentUpgradeMenu.transform.Find("UnitUpgrade2Txt").GetComponent<Text>();
        UnitUpgrade2Txt.text = unit.unitData.upgrade2Cost.ToString();

        UnitUpgradeCloseeBtn = currentUpgradeMenu.transform.Find("UpgradeCloseBtn").GetComponent<Button>();


        // 3티어인 경우 버튼 하나만 생성
        if (unit.unitData.level == 3)
        {
            UnitUpgrade2Btn.gameObject.SetActive(false);

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x - 80, rectTransform.sizeDelta.y);

            RectTransform unitUpgrade1Rect = UnitUpgrade1Btn.GetComponent<RectTransform>();
            unitUpgrade1Rect.anchoredPosition = new Vector2(unitUpgrade1Rect.anchoredPosition.x + 60, unitUpgrade1Rect.anchoredPosition.y);
        }


        if(unit.unitData.upgrade1Cost > InGameManager.inst.gold)
        {
            UnitUpgrade1Btn.interactable = false;
        }
        else
        {
            UnitUpgrade1Btn.interactable = true;
        }

        if (unit.unitData.upgrade2Cost > InGameManager.inst.gold)
        {
            UnitUpgrade2Btn.interactable = false;
        }
        else
        {
            UnitUpgrade2Btn.interactable = true;
        }

        // 첫 번째 업그레이드 옵션 버튼 
        UnitUpgrade1Btn.onClick.RemoveAllListeners();
        UnitUpgrade1Btn.onClick.AddListener(() =>
        {
            upgradeOption = "1";

            CreateUpgradeconfirmedMenu(selectedUnit);

            //PerformUnitUpgrade("1");  // 첫 번째 업그레이드 경로

            
            DestroyUnitUpgradeMenu();
        });

        UnitUpgrade2Btn.onClick.RemoveAllListeners();
        UnitUpgrade2Btn.onClick.AddListener(() =>
        {
            upgradeOption = "2";

            //PerformUnitUpgrade("2");  // 두 번째 업그레이드 경로

            CreateUpgradeconfirmedMenu(selectedUnit);
            DestroyUnitUpgradeMenu();
        });


        UnitUpgradeCloseeBtn.onClick.AddListener(() =>
        {
            DestroyUnitUpgradeMenu();
        });

    }

    private void CreateUpgradeconfirmedMenu(Ingame_UnitCtrl unit)
    {
        GameObject canvas = GameObject.Find("Canvas");

        currentunitUpgradeMenuConfirmBox = Instantiate(unitUpgradeMenuConfirmBox, canvas.transform);

        RectTransform rectTransform = currentunitUpgradeMenuConfirmBox.GetComponent<RectTransform>();
        Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);
        screenPos.x += 200;
        rectTransform.position = screenPos;

        UpgradeCheckCloseBtn = currentunitUpgradeMenuConfirmBox.transform.Find("UpgradeCheckCloseBtn").GetComponent<Button>();
        UpgradeCheckOkBtn = currentunitUpgradeMenuConfirmBox.transform.Find("UpgradeCheckOkBtn").GetComponent<Button>();
        UpgradeCheckCancleBtn = currentunitUpgradeMenuConfirmBox.transform.Find("UpgradeCheckCancleBtn").GetComponent<Button>();


        UpgradeCheckCloseBtn.onClick.AddListener(() =>
        {
            Destroy(currentunitUpgradeMenuConfirmBox);
        });

        UpgradeCheckCancleBtn.onClick.AddListener(() =>
        {
            Destroy(currentunitUpgradeMenuConfirmBox);
        });

        UpgradeCheckOkBtn.onClick.AddListener(() =>
        {
            PerformUnitUpgrade(upgradeOption);
            DestorypgradeMenuConfirmBox();
        });
    }



    private void PerformUnitUpgrade(string upgradeOption)
    {
        if (selectedUnit != null)
        {
            UnitUpgradeManager.Instance.PerformUpgrade(selectedUnit, upgradeOption);
        }
        else
        {
            Debug.LogError("targetUnit이 설정되지 않았습니다!");
        }

        UpdateUnitInfoPanel(selectedUnit);
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

    public void DestorypgradeMenuConfirmBox()
    {
        if (currentunitUpgradeMenuConfirmBox != null)
        {

            Destroy(currentunitUpgradeMenuConfirmBox);

            currentunitUpgradeMenuConfirmBox = null;
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
            SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_pause);
            InGameManager.inst.isGamePause = true;
        }
        else
        {
            SoundManager.instance.PlayUISFx(SoundManager.uiSfx.sfx_click);
            InGameManager.inst.isGamePause = false;
        }

    }

    void RestartGame()
    {
        LoadingSceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}