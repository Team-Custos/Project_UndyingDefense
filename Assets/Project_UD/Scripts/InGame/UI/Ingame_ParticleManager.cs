using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.UI.CanvasScaler;

//이 스크립트는 유닛의 VFX를 관리하는 스크립트 입니다.

public class Ingame_ParticleManager : MonoBehaviour
{
    public static Ingame_ParticleManager Instance;

    public ParticleSystem allySummonEffect; // 유닛 소환 이펙트
    public ParticleSystem enemeySummonEffect; // 적 유닛 소환 이펙트

    public ParticleSystem modeChangeEffect; // 유닛 모드 전환 이펙트 

    public ParticleSystem unitSpawnDeckEffect; // 유닛 소환 모드시 덱에 나타나는 이펙트

    public ParticleSystem[] AttackVFX;
    public ParticleSystem[] AttackCritVFX;

    public ParticleSystem allySelectEffect;    // 아군 유닛 선택 이펙트
    public ParticleSystem enemySelectEffect;    // 적 유닛 선택 이펙트

    public ParticleSystem allySiegeEffect;     // 아군 유닛 시즈모드 이펙트

    public GameObject siegeEffect;


    public GameObject spawnCoinEffect;         // 적 유닛 사망 후 코인 생성 이펙트
    public GameObject enemyGhostEffect;        // 적 사망 후 유령 이펙트

    public GameObject unitMoveIndicator;       // 유닛이 이동하는 곳을 알려주는 이펙트
    private GameObject currentMoveIndicator = null;

    private Dictionary<GameObject, ParticleSystem> activeEffects = new Dictionary<GameObject, ParticleSystem>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // 활성화된 파티클의 위치를 지속적으로 업데이트
        //foreach (var entry in activeEffects)
        //{
        //    GameObject unit = entry.Key;
        //    ParticleSystem effect = entry.Value;

        //    if (unit != null && effect != null)
        //    {
        //        // 유닛 위치를 따라가도록 파티클 위치를 업데이트
        //        effect.transform.position = unit.transform.position + new Vector3(0, -0.9f, 0);
        //    }
        //}
    }

    // 기존 적 소환 이펙트를 유닛 모드 전환 이펙트로 사용
    // 추후 적 소환 이펙트 추가 예정
    public void PlaySummonParticleEffect(Transform tr, bool isAlly = true)
    {
        //ParticleSystem summonEnemyEffect;
        ParticleSystem smmonAllyEffect;

        if (isAlly)
        {
            smmonAllyEffect = Instantiate(allySummonEffect, tr.position, tr.rotation);
            smmonAllyEffect.Play();
            SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_allySummon);
            Destroy(smmonAllyEffect.gameObject, smmonAllyEffect.main.duration);
        }
        //else
        //{
        //    summonEnemyEffect = Instantiate(enemeySummonEffect, tr.position, tr.rotation);
        //    summonEnemyEffect.Play();
        //    Destroy(summonEnemyEffect.gameObject, summonEnemyEffect.main.duration - 3.5f) ;
        //}



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



    private Dictionary<GameObject, ParticleSystem> siegeEffects = new Dictionary<GameObject, ParticleSystem>();

    public void PlaySiegeModeEffect(GameObject unit, bool isSiege)
    {
        if (allySiegeEffect == null)
        {
            Debug.LogError("allySiegeEffect is not assigned.");
            return;
        }

        if (isSiege)
        {
            if (!siegeEffects.ContainsKey(unit))
            {
                // 파티클 인스턴스 생성 및 저장
                ParticleSystem instance = Instantiate(allySiegeEffect, unit.transform);
                instance.transform.localPosition = new Vector3(0, -0.89f, 0);
                siegeEffects[unit] = instance;
            }

            // 파티클 활성화
            ParticleSystem effect = siegeEffects[unit];
            if (!effect.gameObject.activeSelf)
            {
                effect.gameObject.SetActive(true);
                effect.Play();
            }
        }
        else
        {
            // Siege 모드가 아닐 경우 파티클 비활성화
            if (siegeEffects.ContainsKey(unit))
            {
                ParticleSystem effect = siegeEffects[unit];
                if (effect.gameObject.activeSelf)
                {
                    effect.Stop();
                    effect.gameObject.SetActive(false);
                }
            }
        }
    }

    

    public void EnemyDeathEffect(Transform enemyPos)
    {
        StartCoroutine(PlayEnemyDeathEffects(enemyPos));
    }

    // 적 유닛이 죽은 후 생성되는 프리팹 이펙트(유령, 골드), 두 이펙트 간 텀을 두기 위해 코루틴 사용
    private IEnumerator PlayEnemyDeathEffects(Transform enemyPos)
    {
        GameObject ghostPrefab = Instantiate(enemyGhostEffect as GameObject);
        ghostPrefab.transform.position = enemyPos.transform.position;
        Destroy(ghostPrefab, 1.0f);

        yield return new WaitForSeconds(0.5f);

        GameObject coinPrefab = Instantiate(spawnCoinEffect as GameObject);
        coinPrefab.transform.position = ghostPrefab.transform.position + new Vector3(0, 1.0f, 0);
        SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_coinDrop);

        Destroy(coinPrefab, 1.0f);
    }


    // 유닛이 이동할 곳을 알려주는 오브젝트 생성
    public void ShowUnitMoveIndicator(Transform moveTr)
    {
        if (currentMoveIndicator != null)
        {
            Destroy(currentMoveIndicator);
        }

        currentMoveIndicator = Instantiate(unitMoveIndicator as GameObject);

        Vector3 newPosition = moveTr.position;
        newPosition.y += 1.0f;  // y축 위치 조정
        currentMoveIndicator.transform.position = newPosition;

        Destroy(currentMoveIndicator, 1.0f);
    }
}
