using UnityEngine;
using UnityEngine.Events;

public class ProjectileCtrl : MonoBehaviour
{
    protected Unit targetUnit = null;
    protected Fortress fortress = null;
    [SerializeField] protected Rigidbody rb;  // Rigidbody를 할당


    //[SerializeField] protected AttackSkill attackSkil;
    [SerializeField] protected float hitDistance;   // 피격 판정을 위한 거리
    [SerializeField] protected float speed = 1f;

    public void SetTarget(Unit target)
    {
        targetUnit = target;
    }
    public void SetTarget(Fortress target)
    {
        fortress = target;
    }

}
