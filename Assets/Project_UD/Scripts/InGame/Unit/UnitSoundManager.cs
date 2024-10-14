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
    AudioSource SFX;

    [Header("====ATTACK_SFX====")]
    public AudioClip ATK_SLASH;
    public AudioClip ATK_CRUSH;
    public AudioClip ATK_PIERCE;

    [Header("====ATTACKED_SFX====")]
    public ATTACKEDSOUND[] HitSound;

    void Awake()
    {
        SFX = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySFX(AudioClip SFX2Play)
    {
        SFX.clip = SFX2Play;
        SFX.Play();

        //재생중일때 실행여부 판단?
        if (!SFX.isPlaying)
        {
            
        }
    }

}
