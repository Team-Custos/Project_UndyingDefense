using UnityEngine;
using AttackType = AttackData.AttackType;

public class TrapCtrl : AttackSkill
{
    private Animator modelAnimator;
    [SerializeField] private AttackData attackData;
    [SerializeField] private GameObject bindEffectPrefab;

    private float damage;

    private void Start()
    {
        modelAnimator = GetComponent<Animator>();
    }

    public void SetDamage(float amount)
    {
        damage = amount;
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit target = null;
        if (other.TryGetComponent(out Unit targetUnit))
        {
            
            float calcDamage = damage;

            // calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;
            // calcDamage *= unit.AttackDamageMultiplier;
            calcDamage *= target.DamageTakenMult;

            target.TakeDamage(calcDamage);
            target.AddEffect(bindEffectPrefab, target);
        }
    }

}
