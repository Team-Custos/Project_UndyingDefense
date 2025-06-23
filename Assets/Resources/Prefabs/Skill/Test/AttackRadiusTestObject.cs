using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRadiusTestObject : MonoBehaviour
{
    private enum ShapeType
    {
        Sphere,
        Cube
    }

    [SerializeField] private float attackRadius = 5f; // The radius of the attack area
    [SerializeField] private float attackRangeX = 0f;
    [SerializeField] private float attackRangeY = 0f;
    [SerializeField] private float attackRangeZ = 0f; // The range of the attack area
    [SerializeField] private ShapeType attackShape = ShapeType.Sphere; // The shape of the attack area


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (attackShape == ShapeType.Sphere)
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        else if (attackShape == ShapeType.Cube)
            Gizmos.DrawWireCube(transform.position, new Vector3(attackRangeX,attackRangeY,attackRangeZ));
    }
}
