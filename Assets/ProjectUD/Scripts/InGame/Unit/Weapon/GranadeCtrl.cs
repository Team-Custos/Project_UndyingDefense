using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeCtrl : ProjectileCtrl
{
    [SerializeField] private GameObject AttackTrigger;
    private float time = 1f;

    // 특정 좌표로 점프하는 함수
    public void JumpTowards(Vector3 targetPos, float height)
    {
        Vector3 startPos = transform.position;
        float duration = 1f;

        float g = Mathf.Abs(Physics.gravity.y);

        float timeToPeak = duration / 2f;
        float vy = g * timeToPeak;

        Vector3 displacementXZ = new Vector3(
            targetPos.x - startPos.x,
            0,
            targetPos.z - startPos.z
        );

        Vector3 velocityXZ = displacementXZ / duration;
        Vector3 velocity = new Vector3(velocityXZ.x, vy, velocityXZ.z);

        rb.velocity = velocity;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == CONSTANT.TAG_TILE)
        {
            Debug.Log("Granade Hit Ground");
            Destroy(gameObject);
            Instantiate(AttackTrigger, transform.position, Quaternion.identity);
        }

    }
}
