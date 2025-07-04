using UnityEngine;

public class VFXTowardCamera : MonoBehaviour
{
    [SerializeField] private float offset;

    private void LateUpdate()
    {
        Vector3 dir = (Camera.main.transform.position - transform.position).normalized;
        transform.position = transform.parent.position + dir * offset;
    }
}
