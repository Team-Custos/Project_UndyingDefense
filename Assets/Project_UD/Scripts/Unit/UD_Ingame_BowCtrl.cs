using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_BowCtrl : MonoBehaviour
{
    public GameObject Arrow;

    float ShootCooldown_Cur = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ArrowShoot(float ShootCooldown)
    {
        if (ShootCooldown_Cur <= 0)
        {
            GameObject arrow_Obj = Instantiate(Arrow.gameObject);
            arrow_Obj.transform.position = this.transform.position;
            arrow_Obj.transform.rotation = this.transform.rotation;

            UD_Ingame_ArrowCtrl arrowCtrl = arrow_Obj.GetComponent<UD_Ingame_ArrowCtrl>();

            ShootCooldown_Cur = ShootCooldown;
        }
        else
        {
            ShootCooldown_Cur -= Time.deltaTime;
        }
    }
}
