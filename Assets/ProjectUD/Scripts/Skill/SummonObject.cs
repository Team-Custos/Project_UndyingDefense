using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SummonObject : MonoBehaviour
{
    protected Unit unit;

    public virtual void Initialize(Unit unit)
    {
        this.unit = unit;
    }

    public virtual void Destroy()
    {
        // 파괴 -> 풀링 작업 필요
        Destroy(gameObject);
    }
}

public interface ISummonAttack
{
    void Attack(AttackData.AttackType attackType);
}

public interface ISummonDuration
{
    void Duration(float time);
}
