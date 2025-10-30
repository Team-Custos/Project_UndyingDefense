using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSkill_FireOilAnimationEvent : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireOilVFX;
    [SerializeField] private AudioClip OilBottleSFX;

    public void PlayVFX()
    {
        fireOilVFX.Play();
        SoundManager.Instance.PlaySFX(OilBottleSFX, fireOilVFX.gameObject.transform.position);
    }
}
