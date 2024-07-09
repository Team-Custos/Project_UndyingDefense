using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_EnemyCtrl : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //스폰 하기전 스폰 데이터 값을 불러옴.
    public void Init(EnemySpawnData data)
    {
        speed = data.speed;
        maxHealth = data.HP;
        health = data.HP;
    }
}
