using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApocalypsCtrl : MonoBehaviour
{
    // ProjectileCtrl를 상속?? -> rigidbody는 필요 없음
    // 돌이 떨어지는 타이밍에 맞춰 데미지, 상태 부여 및 사운드 재생
    // AttackData, AttackSkill, Attack() 필요

    [SerializeField] private float time;
    private float timeCheck = 0f;

    [SerializeField] private AttackSkill attackSkill;

    private void Update()
    {
        timeCheck += Time.deltaTime;

        if(timeCheck >= time)
        {
            // 데미지 공격
            //attackSkill.Attack();
        }
    }
}
