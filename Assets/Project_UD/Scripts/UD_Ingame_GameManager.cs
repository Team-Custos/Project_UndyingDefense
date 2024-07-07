using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_GameManager : MonoBehaviour
{
    public static UD_Ingame_GameManager inst;

    public bool AllyUnitSetMode = false;

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
