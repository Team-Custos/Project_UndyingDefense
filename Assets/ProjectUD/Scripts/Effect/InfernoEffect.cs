using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class InfernoEffect : TickEffect
{
    [SerializeField] private float damage;
    [SerializeField] private GameObject igniteEffect;

    [Header("■ VFX")]
    [SerializeField] private GameObject Vfx;

    public override void Activate()
    {
        Vfx.SetActive(true);

        Collider[] hits = Physics.OverlapSphere(transform.position, 5f, LayerMask.GetMask("EnemyUnit"));

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Unit unit))
            {
                // 자기 자신 제외
                if (unit == this.GetComponent<Unit>())
                    continue;

                if (Random.value < 0.5f)
                {
                    unit.AddEffect(igniteEffect);
                }
            }
        }
    }

    public override void OnRemove()
    {
        Vfx.SetActive(false);
    }

    protected override void OnTick()
    {
        target.TakeDamage(damage);
    }
}
