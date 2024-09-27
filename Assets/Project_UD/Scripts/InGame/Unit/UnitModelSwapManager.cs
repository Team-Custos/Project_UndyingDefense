using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UnitModelSwapManager : MonoBehaviour
{
    public static UnitModelSwapManager inst;
    public GameObject[] AllyModel;
    public GameObject[] EnemyModel;
    public Animator[] Animator;

    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
