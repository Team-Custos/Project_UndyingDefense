using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 유닛의 SFX를 관리하기 위한 스크립트입니다.

[System.Serializable]
public class ATTACKEDSOUND //피격 사운드
{
    public AttackType type; //피격 공격 속성.
    public AudioClip[] hitSound; //피격 사운드. (일반)
    public AudioClip hitSoundCrit; //피격 사운드. (치명타)
}


public class UnitSoundManager : MonoBehaviour
{
    public AudioSource ATTACK_SFX;//공격 사운드를 재생할 컴포넌트
    public AudioSource HIT_SFX;//피격 사운드를 재생할 컴포넌트
    public AudioSource DEBUFF_SFX;//디버프 사운드를 재생할 컴포넌트
    public AudioSource DEAD_SFX;//사망 사운드를 재생할 컴포넌트

    [Header("====ATTACKED_SFX====")]
    public ATTACKEDSOUND[] HitSound;

    [Header("====DEAD_SFX====")]
    public AudioClip[] DeadSound;

    public void PlaySFX(AudioSource SFX,AudioClip SFX2Play)
    {
        SFX.clip = SFX2Play;
        SFX.Play();

        //재생중일때 실행여부 판단?
        if (!SFX.isPlaying)
        {
            
        }
    }

}
