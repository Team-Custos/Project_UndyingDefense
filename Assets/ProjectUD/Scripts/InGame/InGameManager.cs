using InputEventInterface;
using UltEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


// 조작 중 상태 
public enum OperateState
{
    DEFAULT,      // 기본 상태
    SPAWN,        // 소환 조작 중 상태 
    CS_Area,      // 지휘관 스킬 영역 지정 중 상태
    CS_Target,    // 지휘관 스킬 대상 지정 중 상태
    ALLYUNIT,     // 아군 유닛 선택 중 상태
    UPGRADE       // 승급 진행 상태 중 상태
}


public class InGameManager : MonoBehaviour, IInputClick, IInputESC, IInputSpeedUp, IInputRightClick
{
    public float inGameGold;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private EnemyUnitSpawner enemyUnitSpawner;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;
    [SerializeField] private DollyCamera dollyCamera;
    [SerializeField] private AudioClip inGameIntro;
    [SerializeField] private SelectedUnitUI selectedUnitUI;
    [SerializeField] private SelectedUnitManager selectedUnitManager;
    [SerializeField] private CommandSkillTargetingController commandSkillTargetingController;
    [SerializeField] private Camera mainCamera;


    [SerializeField] private AudioClip inGameBgm;
    [SerializeField] private AudioClip winSfx;
    [SerializeField] private AudioClip loseSfx;
    [SerializeField] private AudioClip winBgm;
    [SerializeField] private AudioClip loseBgm;
    private bool isGameStart = false;
    private bool isGamePause = false;
    private float timeRecord = 0f;   // 현재 게임 플레이 시간 기록용 변수
    private string recordText = "";

    private bool isFastForward = false;

    public bool IsGameStart => isGameStart;
    public bool IsGamgePause => isGamePause;
    public float TimeRecord => timeRecord;
    [Header("■ PlayerPrefs Event")]
    [SerializeField] private UltEvent gameWin;
    [SerializeField] private UltEvent gameFinish;
    [SerializeField] private string id;

    [Header("StagePrefsData")]
    [SerializeField] private StagePrefsData stagePrefsData;

    [Header("공로포인트")]
    [SerializeField] private float winPoint;
    [SerializeField] private float losePoint;

    [Header("ClickState")]
    private OperateState operateState;

    protected static AudioClip coinDropSFX;
    protected static AudioClip CoinDropSFX
    {
        get
        {
            if (coinDropSFX == null)
            {
                coinDropSFX = Resources.Load<AudioClip>("Sound/SFX/효과음/캐릭터/DeathSFX/sfx_coinDrop");
            }
            return coinDropSFX;
        }
    }


    private void Start()
    {
        ingameScreenUI.UpdateGoldTextUI(inGameGold);

        SoundManager.Instance.PlaySFX(inGameIntro);

        inputEventManager.OnClickTarget = this;
        inputEventManager.OnESCTarget = this;
        inputEventManager.OnSpeedUpTarget = this;
        inputEventManager.OnRightClickTarget = this;
        UpdateOperateState(OperateState.DEFAULT);

    }

    private void Update()
    {
        if (isGameStart)
        {
            timeRecord += Time.deltaTime;

            int minutes = Mathf.FloorToInt(timeRecord / 60f);
            int seconds = Mathf.FloorToInt(timeRecord % 60f);
            int milliseconds = Mathf.FloorToInt((timeRecord % 1f) * 100f);

            recordText = $"{minutes:00} : {seconds:00} : {milliseconds:00}";

            ingameScreenUI.SetRecordTextUI(recordText);
        }
    }

    public void SetGold(float gold, bool plus)
    {
        if (plus)
        {
            inGameGold += gold;
            SoundManager.Instance.PlaySFX(CoinDropSFX);
        }
        else
        {
            inGameGold -= gold;
        }

        ingameScreenUI.UpdateGoldTextUI(inGameGold);
    }

    public void ReLoadeCurrentScene()
    {
        SoundManager.Instance.PlayUIClickSFX();
        Time.timeScale = 1.0f;
        LoadingSceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLobbyScene()
    {
        SoundManager.Instance.PlayUIClickSFX();
        Time.timeScale = 1.0f;
        LoadingSceneManager.LoadScene("LobbyScene_LoPol");
    }

    public void StartGame()
    {
        isGameStart = true;
    }

    public void ExitGame()
    {
        SoundManager.Instance.PlayUIClickSFX();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }

    public void PauseGame()   // 게임 일시 정지
    {
        //SoundManager.Instance.PlayUIClickSFX();

        //CancleClickState(ClickState.UI_SETTING);
        //UpdateClickState(ClickState.UI_SETTING);

        isGamePause = true;
        ingameScreenUI.OnOffSettingUI(isGamePause);
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()  // 게임 재개
    {
        isGamePause = false;
        ingameScreenUI.OnOffSettingUI(isGamePause);

        if(isFastForward)
            Time.timeScale = 2f;
        else
            Time.timeScale = 1.0f; 
    }


    public void LoseGame()
    {
        isGameStart = false;
        selectedUnitUI.HideUntInfo();
        ingameScreenUI.ShowResult(losePoint, false, "");
        PlayerPrefsData.instance.SetPoint(losePoint);
        //UserDataModel.instance.SetGameFinished(true);

        //UserDataModel.instance.SetGameFinished(true);
        //---
        gameFinish.Invoke();
        // 남원성 해금 test_260512
        //gameWin.Invoke();
        //---
        SoundManager.Instance.PlaySFX(loseSfx);
        Invoke(nameof(PlayLoseBGM), loseSfx.length);

        enemyUnitSpawner.StopActivateEnemy();
        allyUnitSpawner.StopActivateAlly();
        ingameScreenUI.OnOffInGameUI(false);
    }

    public void WinGame()
    {
        isGameStart = false;
        selectedUnitUI.HideUntInfo();
        ingameScreenUI.ShowResult(winPoint, true, recordText);
        PlayerPrefsData.instance.SetPoint(winPoint);
        //UserDataModel.instance.SetGameFinished(true);
        //UserDataModel.instance.SetGameWin(true);
        //UserDataModel.instance.SetGameFinished(true);
        //---
        if (gameFinish != null && PlayerPrefs.GetInt("IsGeumsanFinished") == 0)
            gameFinish.Invoke();
        if(gameWin != null)
            gameWin.Invoke();

        if(stagePrefsData.IsNewRecord(timeRecord, id))
            ingameScreenUI.ShowNewRecordUI(true);
        else
            ingameScreenUI.ShowNewRecordUI(false);

        stagePrefsData.SetRecordTime(timeRecord, id);
        //---
        SoundManager.Instance.PlaySFX(winSfx);
        Invoke(nameof(PlayWinBGM), loseSfx.length);

        allyUnitSpawner.StopActivateAlly();
        ingameScreenUI.OnOffInGameUI(false);

    }

    private void PlayLoseBGM()
    {
        SoundManager.Instance.PlayBGM(loseBgm);
    }

    private void PlayWinBGM()
    {
        SoundManager.Instance.PlayBGM(winBgm);
    }

    public void ToggleGameSpeed()
    {
        if (isGamePause || dollyCamera.IsCamPanning)
            return;

        isFastForward = !isFastForward;

        Time.timeScale = isFastForward ? 2f : 1f;

        ingameScreenUI.UpdateSpeedButtonAni(isFastForward);

    }

    public void OnSpeedUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ToggleGameSpeed();
        }
    }

    public void PlayInGameBGM()
    {
        SoundManager.Instance.PlayBGM(inGameBgm);
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (inputEventManager.IsPointerOnUIElements())
                    return;

                if (hit.collider.CompareTag("Unit"))    // 유닛 클릭
                {
                    Unit unit = hit.collider.GetComponent<Unit>();

                    if (unit.IsDead)
                    {
                        return;
                    }

                    AllyUnit allyUnit = unit as AllyUnit;

                    if (allyUnit != null)
                    {
                        if (allyUnit.IsChange || allyUnit.IsUpgrade)
                            return;

                        UpdateOperateState(OperateState.ALLYUNIT);
                    }
                    else
                        UpdateOperateState(OperateState.DEFAULT);


                    SoundManager.Instance.PlayUIClickSFX();


                    if (selectedUnitManager.SelectedUnit != null)   // 기존에 선택한 유닛이 잇음
                    {
                        if (unit != selectedUnitManager.SelectedUnit)    // 선택한 유닛이 새 유닛
                        {
                            // 기존 유닛 해제
                            selectedUnitManager.DeSelecteUnit();

                            // 새 유닛 설정
                            selectedUnitManager.SetSelectedUnit(unit);
                        }

                    }
                    else
                    {
                        // 새 유닛 설정
                        selectedUnitManager.SetSelectedUnit(unit);
                    }

                    if(allyUnit != null)
                        selectedUnitUI.ShowAllyUI(allyUnit);
                    else
                        selectedUnitUI.HideAllyUI();


                    selectedUnitUI.UpdateUnitInfo(unit);
                    selectedUnitUI.ShowHp(unit);
                    inputEventManager.OnClickTarget = selectedUnitManager;
                }

            }
        }
    }

    private void UpdateGameState()
    {
        if (dollyCamera.IsCamPanning || !isGameStart)
            return;

        if (isGamePause)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(operateState == OperateState.DEFAULT)
            {
                if (selectedUnitManager.SelectedUnit is EnemyUnit)
                    selectedUnitManager.DeSelecteUnit();
                else
                UpdateGameState();
            }
            else
                CancelCurrentOperate();
        }
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (operateState == OperateState.DEFAULT)
            {
                if (selectedUnitManager.SelectedUnit is EnemyUnit)
                    selectedUnitManager.DeSelecteUnit();

            }
            else
                CancelCurrentOperate();
        }
    }

    // 조작 중 상태 변경 ex) 유닛 소환 -> 지휘관 스킬
    public void UpdateOperateState(OperateState nextState)
    {
        // 같은 상태로 전환 시 취소X
        if(operateState == nextState)
            return;

        // 기존 조작 중 상태 취소
        CancelOperateState(nextState);

        operateState = nextState;
        Debug.Log($"현재 조작 중 상태 : {operateState}");
    }

    // 기존 조작 중 상태 취소 : 상태 변경 시 or 취소(우클릭/ESC)
    public void CancelOperateState(OperateState nextState)
    {
        if (operateState == nextState)
            return;

        switch (operateState)
        {
            case OperateState.ALLYUNIT:
                if(nextState != OperateState.UPGRADE)
                    selectedUnitManager.DeSelecteUnit();
                break;

            case OperateState.SPAWN:
                allyUnitSpawner.CancelSpawn();
                break;

            case OperateState.CS_Area:
                commandSkillTargetingController.CancleAreaSkill();
                break;

            case OperateState.CS_Target:
                commandSkillTargetingController.CancleTargetSkill();
                break;

            case OperateState.UPGRADE:
                selectedUnitManager.CancleUpgrade();
                if(nextState != OperateState.ALLYUNIT)
                    selectedUnitManager.DeSelecteUnit();
                break;
        }

        operateState = nextState;
    }

    private void CancelCurrentOperate()
    {
        OperateState cancelState = GetCancelState();

        CancelOperateState(cancelState);
    }


    // 상태 취소 시 전환될 상태 반환
    private OperateState GetCancelState()
    {
        switch (operateState)
        {
            case OperateState.UPGRADE:
                return OperateState.ALLYUNIT;

            default:
                inputEventManager.OnClickTarget = this;
                return OperateState.DEFAULT;
        }
    }
}