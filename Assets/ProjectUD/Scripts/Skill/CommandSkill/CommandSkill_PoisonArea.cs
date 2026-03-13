using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSkill_PoisonArea : MonoBehaviour
{
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject poisonEffectPrefab;
    private Collider[] buffer = new Collider[10];

    private float duration = 5f;

    private float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayer.value) == 0)
            return;

        if (!other.TryGetComponent(out Unit target))
            return;

        if (target.IsDead)
            return;

        if (Random.value < 0.5f)
        {
            target.AddEffect(poisonEffectPrefab, target, target.transform.position);
        }
        else
            Debug.Log("독 효과 미 적용");

    }

}
