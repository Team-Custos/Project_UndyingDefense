using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationParaCtrl : MonoBehaviour
{
    public Ingame_UnitCtrl unitCtrl;
    public Animator animator;


    // Update is called once per frame
    void Update()
    {
        animator = unitCtrl.VisualModel.GetComponentInChildren<Animator>();

        if (unitCtrl.HP <= 0)
        {
            animator.SetTrigger(CONSTANT.ANITRIGGER_DEAD);
        }

        animator.SetBool(CONSTANT.ANIBOOL_RUN, unitCtrl.haveToMovePosition);
    }
}
