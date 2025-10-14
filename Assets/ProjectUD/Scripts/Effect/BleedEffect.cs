using UnityEngine;

public class BleedEffect : TickStackEffect
{
    [Header("■ Bleed Options")]
    [SerializeField] private float damagePerStack;

    [Header("■ VFX")]
    [SerializeField] private GameObject Vfx;
    [SerializeField] private GameObject overBleedVfx;

    private const float baseDamage = 1f;

    public override void Activate() 
    {
        //OnTick();
        Vfx.SetActive(true);
    }

    protected override void OnMaxStack()
    {
        // 과다 출혈 효과
        target.AddInstantEffect(overBleedVfx);
        target.TakeDamage(20);
    }

    protected override void OnTick()
    {
        target.TakeDamage(baseDamage + (damagePerStack * stack));
        float a = baseDamage + (damagePerStack * stack);
    }

    public override void OnRemove()
    {
        Vfx.SetActive(false);
    }
}
