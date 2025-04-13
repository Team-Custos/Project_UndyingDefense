using UnityEngine;
using UnityEngine.Events;

public class ProjectileCtrl : MonoBehaviour
{
    protected Unit targetUnit = null;
    protected Rigidbody rb;  // Rigidbody를 할당

    [SerializeField] protected float speed = 1f;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    public float GetSpeed()
    {
        return speed;
    }

    public void SetTarget(Unit target)
    {
        targetUnit = target;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
