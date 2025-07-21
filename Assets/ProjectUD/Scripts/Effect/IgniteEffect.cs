using System.Runtime.InteropServices;
using UnityEngine;

public class IgniteEffect : TickStackEffect
{
    [Header("■ Ignite Options")]
    [SerializeField] private float damagePerStack;

    [Header("■ VFX")]
    [SerializeField] private GameObject Vfx;
    [SerializeField] private GameObject infernoVfx;

    [SerializeField] private InfernoEffect infernoEffect;


    protected override void OnMaxStack()
    {
        infernoEffect.Activate();
        SetCaster(target);
        target.AddEffect(infernoVfx);
    }

    public override void Activate() 
    {
        Vfx.SetActive(true);
    }
    protected override void OnTick()
    {
        target.TakeDamage(damagePerStack);
    }

    public override void OnRemove() 
    {
        Vfx.SetActive(false);
    }
}
