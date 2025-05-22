using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutEnd : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Fefnodj");

        // 페이드 아웃 애니메이션이 끝나면 씬 로드
        //Ingame_SceneManager.inst.GoToTitle();
    }
}
