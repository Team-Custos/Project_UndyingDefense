using UnityEngine;

public class BleedEffect : TickStackEffect
{
    [Header("■ Bleed Options")]
    [SerializeField] private float damagePerStack;

    [Header("■ VFX")]
    [SerializeField] private GameObject Vfx;

    private const float baseDamage = 1f;

    public override void Activate() 
    {
        OnTick();
        Vfx.SetActive(true);
    }

    protected override void OnMaxStack()
    {
        // 과다 출혈 효과
    }

    protected override void OnTick()
    {
        target.TakeDamage(baseDamage + (damagePerStack * stack));
    }

    public override void OnRemove()
    {
        Vfx.SetActive(false);
    }
}
