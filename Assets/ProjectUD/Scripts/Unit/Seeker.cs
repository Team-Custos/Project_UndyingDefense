using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Seeker : AllyUnit  // 수행자 전용 클래스
{
    [SerializeField] private AnimatorOverrideController aOC;

    [SerializeField] private AnimationClip jabClip;
    [SerializeField] private AnimationClip hookClip;

    private bool skillFlag = true;

    public override void Initialize()
    {
        base.Initialize();

    }

    //public override 

    protected override void ActivateSkill(SkillBase skill, Unit target)
    {
        if (skill == GeneralSkill)
        {
            if (skillFlag)
                aOC["GeneralSkill"] = jabClip;
            else
                aOC["GeneralSkill"] = hookClip;

            skillFlag = !skillFlag;
        }

        base.ActivateSkill(skill, target);
    }
}

