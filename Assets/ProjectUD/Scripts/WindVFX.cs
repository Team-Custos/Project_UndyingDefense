using UnityEngine;

public class WindVFX : VFX
{
    public void SetDirection(Unit unit, Unit target)
    {
        Vector3 dir = (target.transform.position - transform.position).normalized;
        //unit.AddVFX(gameObject, dir);
    }
}
