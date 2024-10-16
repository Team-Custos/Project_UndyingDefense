using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ATTACKEDSOUND
{
    public AttackType type;
    public AudioClip[] hitSound;
    public AudioClip hitSoundCrit;
}


public class UnitSoundManager : MonoBehaviour
{
    public AudioSource ATTACK_SFX;
    public AudioSource HIT_SFX;
    public AudioSource DEBUFF_SFX;

    [Header("====ATTACKED_SFX====")]
    public ATTACKEDSOUND[] HitSound;

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
