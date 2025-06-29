using UnityEngine;
<<<<<<< Updated upstream
//using AttackType = AttackSkill.AttackType;
=======
using AttackType = AttackData.AttackType;
>>>>>>> Stashed changes

public class TrapCtrl : AttackSkill
{
    private Animator modelAnimator;
<<<<<<< Updated upstream
    [SerializeField] private AttackType attackType;
    [SerializeField] private Effect effectToAdd;
=======
    [SerializeField] private AttackData attackData;
    [SerializeField] private GameObject bindEffectPrefab;

>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
           
            calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;

            target.TakeDamage(calcDamage);
            target.AddEffect(target, effectToAdd);
=======

            // calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;
            // calcDamage *= unit.AttackDamageMultiplier;
            calcDamage *= target.DamageTakenMult;

            target.TakeDamage(calcDamage);
            target.AddEffect(bindEffectPrefab);
>>>>>>> Stashed changes
        }
    }

}
