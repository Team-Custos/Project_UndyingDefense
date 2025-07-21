using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class InfernoEffect : TickEffect
{
    [SerializeField] private float damage;
    [SerializeField] private GameObject igniteEffect;
    [SerializeField] private LayerMask targetLayerMask;

    [Header("■ VFX")]
    [SerializeField] private GameObject vfx;

    private Collider[] hits = new Collider[10];

    public override void Activate()
    {
        vfx.SetActive(true);

        int count = Physics.OverlapSphereNonAlloc(transform.position, 5f, hits, targetLayerMask);

        for (int i = 0; i < count; i++)
        {
            if (hits[i].TryGetComponent(out Unit unit))
            {
                // 발동 유닛 예외처리 필요
                if(caster != null && unit == caster)
                    continue;

                if (Random.value <= 0.5f)
                {
                    unit.AddEffect(igniteEffect);
                }
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
}
