using UnityEngine;
//using AttackType = AttackSkill.AttackType;

public class TrapCtrl : AttackSkill
{
    private Animator modelAnimator;
    [SerializeField] private AttackType attackType;
    [SerializeField] private Effect effectToAdd;
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
           
            calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;

            target.TakeDamage(calcDamage);
            target.AddEffect(target, effectToAdd);
        }
    }

}
