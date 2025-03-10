using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeCtrl : MonoBehaviour
{
    public Vector3 targetPos;
    public float height = 5f;

    public Rigidbody rb;  // Rigidbody를 할당
    public float jumpSpeed = 10f;  // 점프 시 수평 속도
    public float gravity = 9.81f;  // 중력 가속도 (Unity 기본 중력)

    public GameObject AttackTrigger;



    // Start is called before the first frame update
    void Start()
    {
        jumpSpeed = Mathf.Abs((transform.position.x - targetPos.x) + (transform.position.z - targetPos.z)) + 1;
        height = Mathf.Abs((transform.position.x - targetPos.x) + (transform.position.z - targetPos.z)) + 1;
        JumpTowards(targetPos,height);
    }
    
    // 특정 좌표로 점프하는 함수
    public void JumpTowards(Vector3 targetPosition, float jumpHeight)
    {
        // 1. 현재 위치와 목표 위치의 수평 거리 계산 (y축 제외)
        Vector3 direction = new Vector3(targetPosition.x - transform.position.x, 0, targetPosition.z - transform.position.z);
        float horizontalDistance = direction.magnitude;

        // 2. 점프하는데 걸리는 시간을 계산 (수평 속도 기준)
        float timeToTarget = horizontalDistance / jumpSpeed;

        // 3. 수직 속도 계산 (포물선 최고점에서의 높이 기반)
        float verticalSpeed = Mathf.Sqrt(2 * gravity * jumpHeight);

        // 4. 목표 지점까지의 수직 낙하 속도 계산 (점프 시간 기반)
        float fallSpeed = gravity * timeToTarget / 2;

        // 5. 최종 속도 벡터 구성 (수평 방향과 수직 속도를 결합)
        Vector3 finalVelocity = direction.normalized * jumpSpeed;
        finalVelocity.y = verticalSpeed - fallSpeed;

        // 6. Rigidbody에 속도 적용
        rb.velocity = finalVelocity;
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
