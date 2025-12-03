using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class InfernoEffect : TickEffect
{
    [SerializeField] private float damage;
    [SerializeField] private GameObject igniteEffect;
    [SerializeField] private LayerMask targetLayerMask;

    [Header("■ VFX")]
    [SerializeField] private GameObject vfx;

    [Header("■ Sound")]
    [SerializeField] private AudioClip infernoSound;

    private Collider[] hits = new Collider[10];

    private void Start()
    {
        //int count = Physics.OverlapSphereNonAlloc(transform.position, 5f, hits, targetLayerMask);

        //for (int i = 0; i < count; i++)
        //{
        //    if (hits[i].TryGetComponent(out Unit unit))
        //    {
        //        // 발동 유닛 예외처리 필요
        //        if(caster != null && unit == caster)
        //            continue;

        //        //if(Random.value <= 0.5f) // 50% 확률로 작열 효과 적용
        //        //{
        //        //    unit.AddEffect(igniteEffect);
        //        //}
        //        unit.AddEffect(igniteEffect);
        //    }
        //}
    }

    public override void Activate()
    {
        vfx.SetActive(true);
        SoundManager.Instance.PlaySFX(infernoSound, target.transform.position);

        int count = Physics.OverlapSphereNonAlloc(transform.position, 5f, hits, targetLayerMask);

        for (int i = 0; i < count; i++)
        {
            if (hits[i].TryGetComponent(out Unit unit))
            {
                // 발동 유닛 예외처리 필요
                if (caster != null && unit == caster)
                    continue;

                if (Random.value <= 0.5f) // 50% 확률로 작열 효과 적용
                {
                    unit.AddEffect(igniteEffect, unit);
                    
                }

                //if (!unit.HasEffect<InfernoEffect>()) //Random.value <= 0.5f)
                //{
                //    unit.AddEffect(igniteEffect);
                //    Debug.Log("작열 없음");
                //}
                //else
                //{
                //    Debug.Log("작열 있음");
                //}
            }
        }


        //foreach (var hit in hits)
        //{
        //    if (hit.TryGetComponent(out Unit unit))
        //    {
        //        // 자기 자신 제외
        //        if (unit == this.GetComponent<Unit>())
        //            continue;

        //        if (Random.value < 0.5f)
        //        {
        //            unit.AddEffect(igniteEffect);
        //        }
        //    }
        //}
    }

    public override void OnRemove()
    {
        vfx.SetActive(false);
    }

    protected override void OnTick()
    {
        target.TakeDamage(damage);
    }

    public override bool IsSameType(GameObject effectPrefab)
    {
        return prefab == effectPrefab || igniteEffect == effectPrefab;
    }
}
