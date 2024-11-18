using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 모델의 애니메이터 파라미터를 관리하기 위한 스크립트입니다.

public class UnitAnimationParaCtrl : MonoBehaviour
{
    public Ingame_UnitCtrl unitCtrl;
    public Animator animator;
    bool isDead = false;

    private void Start()
    {
        animator = unitCtrl.VisualModel.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        animator = unitCtrl.VisualModel.GetComponentInChildren<Animator>();

        if (unitCtrl.HP <= 0 && unitCtrl.isDead)
        {
            if (!isDead)
            {
                isDead = true;
                animator.SetTrigger(CONSTANT.ANITRIGGER_DEAD);
            }
        }

        animator.SetBool(CONSTANT.ANIBOOL_RUN, unitCtrl.haveToMovePosition);
    }
}
