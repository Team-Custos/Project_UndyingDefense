using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_UnitSkillManager : MonoBehaviour
{

    public GameObject Bow;
    public GameObject Sword;

    public UD_Ingame_UnitCtrl UnitCtrl;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnitGeneralSkill(int SkillCode, Vector3 TargetPos)
    {
        switch (SkillCode)
        {
            case 101://검 베기
                //근거리 공격 정해지는 대로 작업.

                break;
            case 102://활 쏘기
                if (Bow != null)
                {
                    Bow.transform.LookAt(TargetPos);
                    Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(UnitCtrl.weaponCooldown, UnitCtrl.attackPoint, true);
                }
                break;  
        }
    }

    public void UnitSpecialSkill(int SkillCode, Vector3 TargetPos)
    {
        switch (SkillCode)
        {
            case 101://낫 찍기
                // 검베기와 같은 처리. 스탯만 다르게.
                break;
            case 102://덫 설치
                

                break;
        }
    }

    public void EnemyGeneralSkill(int SkillCode, Vector3 TargetPos)
    {
        switch (SkillCode)
        {
            case 101://검 베기
                break;
            case 102://활 쏘기
                if (Bow != null)
                {
                    Bow.transform.LookAt(TargetPos);
                    Bow.GetComponent<UD_Ingame_BowCtrl>().ArrowShoot(UnitCtrl.weaponCooldown, UnitCtrl.attackPoint, true);
                }
                break;
        }
    }
}
