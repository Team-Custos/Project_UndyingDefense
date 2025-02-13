using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ATTACKSOUND //피격 사운드
{
    public AttackType type; //피격 공격 속성.
    public AudioClip[] hitSound; //피격 사운드. (일반)
    public AudioClip hitSoundCrit; //피격 사운드. (치명타)
}

[System.Serializable]
public class UnitSound_Rebuild : MonoBehaviour
{
    public AudioSource SFXSource;

    [Header("====ATTACKED_SFX====")]
    public ATTACKSOUND[] HitSound;

    [Header("====DEAD_SFX====")]
    public AudioClip[] DeadSound;

    public void PlaySFX(AudioClip SFX2Play)
    {
        SFXSource.PlayOneShot(SFX2Play);
    }
}
