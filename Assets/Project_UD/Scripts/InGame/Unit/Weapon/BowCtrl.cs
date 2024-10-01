using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowCtrl : MonoBehaviour
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

    public void ArrowShoot(float ShootCooldown, int atk, bool isEnemyAttack)
    {
        if (ShootCooldown_Cur <= 0)
        {
            GameObject arrow_Obj = Instantiate(Arrow);
            arrow_Obj.transform.SetPositionAndRotation(this.ShootPos.position, this.transform.rotation);
            AttackCtrl arrowCtrl = arrow_Obj.GetComponent<AttackCtrl>();

            arrowCtrl.Atk = atk;
            arrowCtrl.isEnemyAttack = isEnemyAttack;

            ShootCooldown_Cur = ShootCooldown;
        }
        else
        {
            ShootCooldown_Cur -= Time.deltaTime;
        }
    }
}
