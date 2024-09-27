using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackType
{
    Pierce,
    Slash,
    Crush,
    Magic
}

public enum AttackMethod
{
    Arrow,
    Melee,
    Trap,
}

public class AttackCtrl : MonoBehaviour
{
    public AttackType Type;
    public AttackMethod MethodType;

    public int Atk = 1;
    public float speed = 1f;
    public float CritPercentAdd = 0;
    public bool isEnemyAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        if (MethodType == AttackMethod.Arrow)
        {
            Destroy(gameObject, 3f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (MethodType)
        {
            case AttackMethod.Arrow:
                transform.Translate(0.5f * speed * Vector3.forward);
                break;
            case AttackMethod.Melee:
                break;
            case AttackMethod.Trap:
                CritPercentAdd = 5;
                break;
        }        
    }

    private void OnDestroy()
    {
        
    }
}
