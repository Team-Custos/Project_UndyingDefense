using InputEventInterface;
using UltEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InGameManager : MonoBehaviour, IInputESC
{
    public float inGameGold;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private EnemyUnitSpawner enemyUnitSpawner;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;
    [SerializeField] private DollyCamera dollyCamera;
    [SerializeField] private AudioClip inGameIntro;

    [SerializeField] private AudioClip winSfx;
    [SerializeField] private AudioClip loseSfx;
    [SerializeField] private AudioClip winBgm;
    [SerializeField] private AudioClip loseBgm;
    private bool isGameStart = false;
    private bool isGamePause = false;
    private float timeRecord = 0f;   // 현재 게임 플레이 시간 기록용 변수

    public bool IsGameStart => isGameStart;
    public float TimeRecord => timeRecord;
    [Header("■ PlayerPrefs Event")]
    [SerializeField] private UltEvent gameWin;
    [SerializeField] private UltEvent gameFinish;
    [SerializeField] private string id;

    [Header("StagePrefsData")]
    [SerializeField] private StagePrefsData stagePrefsData;


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
    }

    private void Update()
    {
        if (isGameStart)
        {
            timeRecord += Time.deltaTime;

            int minutes = Mathf.FloorToInt(timeRecord / 60f);
            int seconds = Mathf.FloorToInt(timeRecord % 60f);
            string text = $"{minutes:00}:{seconds:00}";

            ingameScreenUI.SetRecordTextUI(text);
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

    public void PauseGame()
    {
        SoundManager.Instance.PlayUIClickSFX();
        if (!isGamePause)
        {
            Time.timeScale = 0f;
            isGamePause = true;
        }
        else
        {
            Time.timeScale = 1f;
            isGamePause = false;
        }
    }

    public void OnESC(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (dollyCamera.IsCamPanning || !isGameStart)
                return;


            //if (enemyUnitSpawner.IsGameOver)
            //    return;

            //inputEventManager.OnESCTarget = this;

            //SoundManager.Instance.playCancleSFX();
            ingameScreenUI.OnOffSettingUI();
        }
    }

    public void LoseGame()
    {
        isGameStart = false;
        //UserDataModel.instance.SetGameFinished(true);

        //UserDataModel.instance.SetGameFinished(true);
        //---
        gameFinish.Invoke();
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
        //UserDataModel.instance.SetGameFinished(true);
        //UserDataModel.instance.SetGameWin(true);
        //UserDataModel.instance.SetGameFinished(true);
        //---
        if(gameFinish != null && PlayerPrefs.GetInt("IsGeumsanFinished") == 0)
            gameFinish.Invoke();
        if(gameWin != null)
            gameWin.Invoke();
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
}