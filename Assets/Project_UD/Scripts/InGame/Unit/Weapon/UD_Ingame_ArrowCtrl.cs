using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_ArrowCtrl : MonoBehaviour
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
}
