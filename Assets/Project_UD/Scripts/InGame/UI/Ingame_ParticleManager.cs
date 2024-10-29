using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ingame_ParticleManager : MonoBehaviour
{
    public static Ingame_ParticleManager Instance;

    public ParticleSystem allySummonEffect; // 유닛 소환 이펙트
    public ParticleSystem modeChangeEffect; // 유닛 모드 전환 이펙트 

    public ParticleSystem unitSpawnDeckEffect; // 유닛 소환 모드시 덱에 나타나는 이펙트

    public ParticleSystem[] AttackVFX;
    public ParticleSystem[] AttackCritVFX;


    private void Awake()
    {
        Instance = this;
    }


    // 기존 적 소환 이펙트를 유닛 모드 전환 이펙트로 사용
    // 추후 적 소환 이펙트 추가 예정
    public void PlaySummonParticleEffect(Transform tr, bool isAlly)
    {
        ParticleSystem effectInstance = Instantiate(allySummonEffect, tr.position, tr.rotation);

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

    public void PlayUnitModeChangeParticleEffect(Transform tr, float ypos)
    {
        Vector3 spawnPosition = tr.position;
        spawnPosition.y += ypos; // y 축 조정

        ParticleSystem modeChangeInstance = Instantiate(modeChangeEffect, spawnPosition, tr.rotation);
        modeChangeInstance.Play();

        Destroy(modeChangeInstance.gameObject, modeChangeInstance.main.duration);
    }
}
