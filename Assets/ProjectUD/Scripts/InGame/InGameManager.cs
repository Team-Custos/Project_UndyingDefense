using InputEventInterface;
using UltEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class InGameManager : MonoBehaviour, IInputESC, IInputSpeedUp
{
    public float inGameGold;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private EnemyUnitSpawner enemyUnitSpawner;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;
    [SerializeField] private DollyCamera dollyCamera;
    [SerializeField] private AudioClip inGameIntro;
    [SerializeField] private SelectedUnitUI selectedUnitUI;

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
        ingameScreenUI.SetGoldTextUI(inGameGold);

        SoundManager.Instance.PlaySFX(inGameIntro);

        inputEventManager.OnESCTarget = this;
        inputEventManager.OnSpeedUpTarget = this;
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

        ingameScreenUI.SetGoldTextUI(inGameGold);
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

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (dollyCamera.IsCamPanning || !isGameStart)
                return;

            if(isGamePause)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }

            //ingameScreenUI.OnOffSettingUI(isGamePause);

        }
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
}