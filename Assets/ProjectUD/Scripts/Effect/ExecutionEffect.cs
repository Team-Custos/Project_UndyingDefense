using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionEffect : MonoBehaviour
{
    [SerializeField] private GameObject startVfx;
    [SerializeField] private GameObject loopVfx;
    [SerializeField] private GameObject endVfx;

    private EnemyUnit target;


    public void ActivateExecution()
    {
        startVfx.SetActive(true);
        loopVfx.SetActive(true);
        endVfx.SetActive(false);
    }


    //public void SetTarget(EnemyUnit enemy)
    //{
    //    target = enemy;
    //    transform.SetParent(target.transform);
    //}

    public void OnTargetDead()
    {
        endVfx.SetActive(true);
        loopVfx.SetActive(false);
        transform.SetParent(null);
    }
}
