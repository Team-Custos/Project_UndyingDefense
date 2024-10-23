using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ingame_ParticleManager : MonoBehaviour
{
    public static Ingame_ParticleManager Instance;

    public ParticleSystem allySummonEffect;
    public ParticleSystem enemySummonEffect;

    public ParticleSystem[] AttackVFX;
    public ParticleSystem[] AttackCritVFX;


    private void Awake()
    {
        Instance = this;
    }

    public void PlaySummonParticleEffect(Transform tr, bool isAlly)
    {
        ParticleSystem summonEffect;

        // 파티클 아군, 적 구분
        if (isAlly)
        {
            summonEffect = allySummonEffect;
        }
        else
        {
            summonEffect = enemySummonEffect;
        }

        ParticleSystem effectInstance = Instantiate(summonEffect, tr.position, tr.rotation);

        effectInstance.Play();

        Destroy(effectInstance.gameObject, effectInstance.main.duration);
    }

    public void PlayAttackedParticleEffect(Transform AttackedUnit, AttackType attackType, bool Crit)
    {
        ParticleSystem Effect;

        if (Crit)
        {
            Effect = AttackCritVFX[(int)attackType];
        }
        else
        {
            Effect = AttackVFX[(int)attackType];
        }

        ParticleSystem effectInstance = Instantiate(Effect, AttackedUnit);

        effectInstance.Play();

        Destroy(effectInstance.gameObject, effectInstance.main.duration);
    }
}
