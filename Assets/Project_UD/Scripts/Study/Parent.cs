using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parent : MonoBehaviour
{
    protected float hp;

    public virtual void Attack()
    {
        Debug.Log("주먹 공격!");
    }
}

public class Child : Parent
{
    public override void Attack()
    {
        Debug.Log("발차기 공격!");
    }
}

public class Test : MonoBehaviour
{
    void Start()
    {
        Parent parent = new Parent();
        parent.Attack();
        Parent child = new Child();
        child.Attack();
    }
}
