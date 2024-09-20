using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_BowCtrl : MonoBehaviour
{
    public GameObject Arrow;
    public Transform ShootPos;

    float ShootCooldown_Cur = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ArrowShoot(bool isEnemyAttack)
    {
        GameObject arrow_Obj = Instantiate(Arrow);
        arrow_Obj.transform.SetPositionAndRotation(this.ShootPos.position, this.transform.rotation);
        UD_Ingame_AttackCtrl arrowCtrl = arrow_Obj.GetComponent<UD_Ingame_AttackCtrl>();

        arrowCtrl.isEnemyAttack = isEnemyAttack;
    }
}
