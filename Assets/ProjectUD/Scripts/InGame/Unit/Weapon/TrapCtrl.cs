using UnityEngine;
<<<<<<< HEAD
//using AttackType = AttackSkill.AttackType;
=======
<<<<<<< Updated upstream
//using AttackType = AttackSkill.AttackType;
=======
using AttackType = AttackData.AttackType;
>>>>>>> Stashed changes
>>>>>>> KimJK

public class TrapCtrl : AttackSkill
{
    private Animator modelAnimator;
<<<<<<< HEAD
    [SerializeField] private AttackType attackType;
    [SerializeField] private Effect effectToAdd;
=======
<<<<<<< Updated upstream
    [SerializeField] private AttackType attackType;
    [SerializeField] private Effect effectToAdd;
=======
    [SerializeField] private AttackData attackData;
    [SerializeField] private GameObject bindEffectPrefab;

>>>>>>> Stashed changes
>>>>>>> KimJK
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
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
>>>>>>> KimJK
           
            calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;

            target.TakeDamage(calcDamage);
            target.AddEffect(target, effectToAdd);
<<<<<<< HEAD
=======
=======

            // calcDamage -= calcDamage * target.DamageReductionMultiplier * 0.01f;
            // calcDamage *= unit.AttackDamageMultiplier;
            calcDamage *= target.DamageTakenMult;

            target.TakeDamage(calcDamage);
            target.AddEffect(bindEffectPrefab);
>>>>>>> Stashed changes
>>>>>>> KimJK
        }
    }

}
