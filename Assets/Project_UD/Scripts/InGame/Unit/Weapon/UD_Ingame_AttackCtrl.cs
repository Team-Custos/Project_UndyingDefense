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

public class UD_Ingame_AttackCtrl : MonoBehaviour
{
    public AttackType Type;
    public AttackMethod MethodType;

    public int Atk = 1;
    public float speed = 1f;
    public float CritPercent = 1;
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
                transform.Translate(Vector3.forward * speed * 0.5f);
                break;
            case AttackMethod.Melee:
                break;
            case AttackMethod.Trap:
                break;
        }        
    }

    private void OnDestroy()
    {
        
    }
}
