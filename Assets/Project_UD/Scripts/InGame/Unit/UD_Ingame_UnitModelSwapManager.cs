using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UD_Ingame_UnitModelSwapManager : MonoBehaviour
{
    public static UD_Ingame_UnitModelSwapManager inst;
    public GameObject[] AllyModel;
    public GameObject[] EnemyModel;

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
