using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum AttackType
{
    Pierce = 0,
    Slash = 1,
    Crush = 2,
    Magic = 3,
    UnKnown = 99,
}

public enum AttackMethod
{
    Arrow,
    Granade,
    Trap,
}

public class AttackCtrl : MonoBehaviour
{
    public AttackMethod MethodType;

    public AttackType Type;
    public UnitDebuff Debuff2Add;
    public int Damage = 1;
    public float moveSpeed = 1f;
    public float Crit = 0;
    public bool isEnemyAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        if (MethodType == AttackMethod.Arrow)
        {
            Destroy(gameObject, 3f);
        }
        else if (MethodType == AttackMethod.Granade)
        {
            Destroy(gameObject, 0.1f);
        }

    }

    // Update is called once per frame
    void Update()
    {
        switch (MethodType)
        {
            case AttackMethod.Arrow:
                transform.Translate(0.5f * moveSpeed * Vector3.forward);
                break;
            case AttackMethod.Granade:

                break;
            case AttackMethod.Trap:
                //CritPercentAdd = 5;
                break;
        }        
    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (this.MethodType == AttackMethod.Granade)
        {
            if (other.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
            {
                Ingame_UnitCtrl EnemyCtrl = other.gameObject.GetComponent<Ingame_UnitCtrl>();

                if (EnemyCtrl != null)
                {
                    Debug.Log("Granade Hit!");
                    EnemyCtrl.ReceivePhysicalDamage(Damage, Crit, Type, Debuff2Add);
                }
            }
        }
    }
}
