using System.Collections.Generic;
using UnityEngine;

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

    [Header("■ Wave Options")] // 웨이브에 사용되는 변수들
    [SerializeField] private bool isInfiniteMode = false;
    private int curWave = 0; // 현재 웨이브
    private float waveTimer = 20f;
    private bool isWaveEnd = true;  // 웨이브가 끝났는지 여부
    private bool isWaveWait = true; // 웨이브 준비 상태
    private float waveDelay = 4.0f;
    private bool isFortressAttacked = false;
    private int infiniteWaveCount = 1;
    public bool IsWaveEnd => isWaveEnd;
    public int CurWave => curWave;


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

                    if(!isInfiniteMode)
                    {
                        ingameScreenUI.SetWaveNumber(curWave, waveDatas.Length, false);
                        ingameScreenUI.ShowNotice(curWave + "차 침공 시작");
                    }
                    else
                    {
                        ingameScreenUI.SetWaveNumber(infiniteWaveCount, 0, true);
                        ingameScreenUI.ShowNotice(infiniteWaveCount + "차 침공 시작");
                    }
                    
                    SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_waveStart]);
                    fortress.ResetFortressState();
                    waveTimer = 20f;
                    waveDelay = 4f;
                }
            }
        }
    }


    public void SetWaverTimerEnd()
    {
        waveTimer = 0.0f;
    }

    public void SetWaveEnd()
    {
        if (curWave >= waveDatas.Length)
        {
            if(!isInfiniteMode)
            {
                ingameScreenUI.ShowResult(inGameManager.inGameGold, true);
                inGameManager.WinGame();
                SoundManager.Instance.StopBGM();
                SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_battleWin]);
            }
            else
            {
                ingameScreenUI.ShowNotice("방어 성공");
                SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_waveWin]);

                inGameManager.SetGold(waveDatas[curWave - 1].Reward, true);
                ingameScreenUI.SetspawnBtnPriceTextColor();
                upgradeMenuUI.UpdateUpgradeCostTxt();

                curWave = 0;
                isWaveEnd = true;
                isFortressAttacked = false;
                infiniteWaveCount++;
            }
            
        }
        else
        {
            ingameScreenUI.ShowNotice("방어 성공");
            SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_waveWin]);

            inGameManager.SetGold(waveDatas[curWave - 1].Reward, true);
            ingameScreenUI.SetspawnBtnPriceTextColor();
            upgradeMenuUI.UpdateUpgradeCostTxt();

            isWaveEnd = true;
            isFortressAttacked = false;

            if (isInfiniteMode)
                infiniteWaveCount++;
        }
    }

    public void StartWave()
    {
        isWaveWait = false;
    }

    public void PlayLoseSfx()
    {
        SoundManager.Instance.PlaySFX(waveSfxClip[(int)waveSfx.sfx_battleLose]);
    }

}
