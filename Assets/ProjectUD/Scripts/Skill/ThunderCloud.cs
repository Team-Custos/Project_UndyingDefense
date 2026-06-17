using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThunderCloud : MonoBehaviour
{
    private Unit unit;
    [SerializeField] private AttackSkill skill;

    [SerializeField] private float duration;
    private float durationCheck;
    [SerializeField] private float tickTime;
    private float tickCheck;

    private float damage;

    private const int maxTargetCount = 10;
    [SerializeField] private float radius;

    private Collider[] targets;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject vfx;

    public void Initialize(Unit unit)
    {
        this.unit = unit;
    }


    private void Update()
    {
        if(durationCheck < duration)
        {
            durationCheck += Time.deltaTime;
            tickCheck += Time.deltaTime;

            if (tickCheck >= tickTime)
            {
                tickCheck -= tickTime;
                Attack();
                // 공격
            }
        }
        else
        {
            // 파괴 -> 풀링 작업 필요
            Destroy(gameObject);
        }

    }

    private void Attack()
    {
        if (targets == null)
            targets = new Collider[maxTargetCount];

        int targetCount = Physics.OverlapSphereNonAlloc(transform.position, radius, targets, targetLayer);

        if (targetCount <= 0)
            return;

        int index = Random.Range(0, targetCount);

        Unit target = targets[index].GetComponent<Unit>();

        float dist = Vector3.Distance(transform.position, target.transform.position);

        if (target != null && dist <= radius)
        {
            skill.Attack(unit, target);

            target.AddVFX(vfx, transform.position);
        }
    }
}
