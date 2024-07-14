using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_EnemyCtrl : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;

    MeshRenderer MeshRenderer;

    public Color32 colorEnemy = Color.red;

    public GameObject Selected_Particle;

    public bool isSelected = false;

    public Vector3 moveTargetPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        MeshRenderer.material.color = colorEnemy;
        Selected_Particle.SetActive(isSelected);
    }

    //스폰 하기전 스폰 데이터 값을 불러옴.
    public void Init(EnemySpawnData data)
    {
        speed = data.speed;
        maxHealth = data.HP;
        health = data.HP;
    }
}
