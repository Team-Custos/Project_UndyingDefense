using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
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

    public enum uiSfx
    {
        sfx_bookClose,              //  도움말, 캐릭터 도감 창 닫기
        sfx_bookOpen,               //  도움말, 캐릭터 도감 창 열기
        sfx_click,                  //  일반 선택
        sfx_exit,                   //  나가기, 취소
        sfx_pause,                  //  일시 정지
        sfx_unableClick,            //  선택 불가
    }

    [SerializeField] private AudioSource bgmAudio;
    [SerializeField] private AudioSource sfxAudio;

    [SerializeField] AudioClip[] unitSfxClip;
    [SerializeField] AudioClip[] uiSfxClip;

    public void PlayBGM(AudioClip clip)
    {
        if (bgmAudio.clip == clip)
            return;

        bgmAudio.clip = clip;
        bgmAudio.Play();
    }

    public void StopBGM()
    {
        bgmAudio.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxAudio.PlayOneShot(clip);
    }
}
