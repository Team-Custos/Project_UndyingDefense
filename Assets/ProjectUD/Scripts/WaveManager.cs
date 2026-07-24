using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using static StagePrefsData;

public class WaveManager : MonoBehaviour
{
    public enum waveSfx
    {
        sfx_waveWin,                //  웨이브 방어 성공
        sfx_wavePrepare,            //  웨이브 준비 단계 알림
        sfx_waveStart,              //  웨이브 시작 알림
        sfx_battleLose,             //  전투 패배
        sfx_battleWin               //  전투 승리
    }

    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private EnemyUnitSpawner enemyUnitSpawner;
    [SerializeField] private IngameScreenUI ingameScreenUI;
    [SerializeField] private UpgradeMenuUI upgradeMenuUI;
    [SerializeField] private Fortress fortress;
    [SerializeField] private AudioClip[] waveSfxClip;
    [SerializeField] private WaveData[] waveDatas; // 웨이브 데이터
    [SerializeField] private DollyCamera dollyCamera;
    [SerializeField] private WaveDataLoader waveDataLoader;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;

    [Header("■ Wave Options")] // 웨이브에 사용되는 변수들
    [SerializeField] private bool isTutorial = false;
    private int curWave = 0; // 현재 웨이브
    private float waveTimer = 20f;
    private bool isWaveEnd = true;  // 웨이브가 끝났는지 여부
    private bool isWaveWait = true; // 웨이브 준비 상태
    private float waveDelay = 4.0f;
    private bool isFortressAttacked = false;
    public bool IsWaveEnd => isWaveEnd;
    public int CurWave => curWave;

    [Header("StagePrefsData")]
    [SerializeField] private StagePrefsData stagePrefsData;

    private void Update()
    {
        if (isWaveWait)
            return;

        if (isWaveEnd) // 웨이브 대기 상태
        {
            waveDelay -= Time.deltaTime;

            if(waveDelay <= 0f)
            {
                waveTimer -= Time.deltaTime;
                ingameScreenUI.ShowTimer();
                ingameScreenUI.SetNoticeText("웨이브 시작까지 " + (int)waveTimer + "초");

                if (waveTimer <= 0f)
                {
                    ingameScreenUI.HideTimer();
                    isWaveEnd = false;
                    curWave++;
                    enemyUnitSpawner.StartSpawn(waveDatas[curWave - 1]);

                    ingameScreenUI.SetWaveNumber(curWave, waveDatas.Length, false);
                    //--Localize
                    //ingameScreenUI.ShowNotice(curWave + "차 침공 시작");
                    ShowWaveStart("NTF_battleWaveStart", curWave);
                    allyUnitSpawner.SetIdleState(false);

                    SoundManager.Instance.PlayUISFX(waveSfxClip[(int)waveSfx.sfx_waveStart]);
                    fortress.ResetFortressState();
                    waveTimer = 20f;
                    waveDelay = 4f;
                }
            }
        }
    }
    //--Localize Smart용 함수 추가
    public void ShowWaveStart(string key, int waveNum)
    {
        // string table = "IngameUI"; // 씌는 Table 이름
        // string key = "NTF_battleWaveStart";

        //LocalizationSettings.StringDatabase
        //    .GetLocalizedStringAsync("IngameUI", key, waveNum)
        //    .Completed += handle =>
        //    {
        //        //yourTextMeshPro.text = handle.Result;
        //        ingameScreenUI.ShowNotice(handle.Result);
        //    };
    }


    public void SetWaverTimerEnd()
    {
        waveTimer = 0.0f;
    }

    public void SetWaveEnd()
    {
        if (curWave >= waveDataLoader.WaveDataList.Count)//waveDatas.Length)
        {
            SoundManager.Instance.StopBGM();
            inGameManager.WinGame();

            if(isTutorial)
                SetTutorialEnd();
        }
        else
        {
            //--Localize
            ingameScreenUI.ShowNotice(LocalizationSettings.StringDatabase.
                    GetLocalizedString("IngameUI", "NTF_battleWaveWin", LocalizationSettings.SelectedLocale));
            //ingameScreenUI.ShowNotice("방어 성공");
            //---
            //---
            SoundManager.Instance.PlayUISFX(waveSfxClip[(int)waveSfx.sfx_waveWin]);

            inGameManager.SetGold(waveDatas[curWave - 1].Reward, true);
            ingameScreenUI.SetspawnBtnPriceTextColor();
            upgradeMenuUI.UpdateUpgradeCostTxt();


            isWaveEnd = true;
            isFortressAttacked = false;


            allyUnitSpawner.SetIdleState(isWaveEnd);
        }
    }

    private void SetTutorialEnd()   // 나중에 StagePrefsData로 메서드 옮기기
    {
        PlayerPrefs.SetInt("IsTutorialEnd", 1);
        StageData stagedata = stagePrefsData.GetStageData("UNQ_gumsanCastle");
        if (!stagedata.isOpen)
        {
            stagedata.isOpen = true;
        }
        //stagePrefsData.SetStageDictionary("UNQ_gumsan", true, false, 0);  // struct 일때 사용했던 코드
        stagePrefsData.SaveStageData();
    }

    public void StartWave()
    {
        isWaveWait = false;
    }

    public void PlayLoseSfx()
    {
        SoundManager.Instance.PlayUISFX(waveSfxClip[(int)waveSfx.sfx_battleLose]);
    }

}
