using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 화살 오브젝트를 관리하기 위한 스크립트입니다.
public class ArrowCtrl : MonoBehaviour
{
    public int Atk = 1;
    public float speed = 1f;
    public bool isEnemyAttack = false;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            Destroy(this.gameObject);
        }
    }
}
