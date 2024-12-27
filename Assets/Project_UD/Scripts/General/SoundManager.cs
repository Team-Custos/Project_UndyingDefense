using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GlobalSoundManager;
public class SoundManager : MonoBehaviour
{ 
    public enum unitSfx
    {
        sfx_allySpawn,              //  병사 스폰
        sfx_allySummon,             //  병사 소환진
        sfx_assignAble,             //  병사 배치
        sfx_assignUnable,           //  병사 배치 불가능
        sfx_enemySpawn,             //  적 스폰
        sfx_enemySummon,            //  적 소환진
        sfx_select,                 //  캐릭터 선택
        sfx_toFree,                 //  시즈 모드에서 프리 모드로 전환
        sfx_toSiege,                //  프리 모드에서 시즈 모드로 전환
        sfx_coinDrop,               //  적 처치 후 엽전 획득
        sfx_upgrade                 //  유닛 업그레이드
    }

    public enum waveSfx
    {
        sfx_waveWin,                //  웨이브 방어 성공
        sfx_wavePrepare,            //  웨이브 준비 단계 알림
        sfx_waveStart,              //  웨이브 시작 알림
        sfx_battleLose,             //  전투 패배
        sfx_battleWin               //  전투 승리
    }

    public enum uiSfx
    {
        sfx_bookClose,              //  도움말, 캐릭터 도감 창 닫기
        sfx_bookOpen,               //  도움말, 캐릭터 도감 창 열기
        sfx_click,                  //  일반 선택
        sfx_exit,                   //  나가기, 취소
        sfx_pause,                  //  일시 정지
        sfx_unableClick,            //  선택 불가
    }

    public static SoundManager instance;

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] AudioClip[] unitSfxClip;
    [SerializeField] AudioClip[] waveSfxClip;
    [SerializeField] AudioClip[] uiSfxClip;

    [SerializeField] AudioSource IngameSfxSource;


    public void PlayUnitSFX(unitSfx unitsfx)
    {
        IngameSfxSource.PlayOneShot(unitSfxClip[(int)unitsfx]);
    }

    public void PlayWaveSFX(waveSfx wavesfx)
    {
        IngameSfxSource.PlayOneShot(waveSfxClip[(int)wavesfx]);
    }

    public void PlayUISFx(uiSfx uisfx)
    {
        IngameSfxSource.PlayOneShot(uiSfxClip[(int)uisfx]);
    }
}
