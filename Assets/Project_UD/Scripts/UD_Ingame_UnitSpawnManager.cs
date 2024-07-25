using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class UnitSpawnData
{
    public int modelType;
    public float spawnTime;
    public int HP;
    public float speed;
    public UnitType unitType;

}

public enum UnitType
{
    Basic,
    what
}


public class UD_Ingame_UnitSpawnManager : MonoBehaviour
{
    UD_Ingame_GridManager GRIDMANAGER;
    public static UD_Ingame_UnitSpawnManager inst;

    public int unitType = 0;
    public UnitSpawnData[] spawnData;

    public GameObject Test_Ally;
    public GameObject Test_Enemy;


    // Start is called before the first frame update
    void Start()
    {
        GRIDMANAGER = UD_Ingame_GameManager.inst.gridManager;
        inst = this;

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    //¿Ø¥÷ º“»Ø
    public GameObject UnitSpawn(bool IsAlly , float X, float Y)
    {
        GameObject Obj = null;

        if (IsAlly)
        {
            Obj = Instantiate(Test_Ally);
            Obj.transform.position = new Vector3(X, 0, Y);
            Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2 ((int)(X/2), (int)(Y/2));
            
        }
        else
        {
            Obj = Instantiate(Test_Enemy);
            Obj.transform.position = new Vector3(X, 0, Y);
            Obj.GetComponent<UD_Ingame_UnitCtrl>().Init(spawnData[unitType]);
        }

        return Obj;
    }
}
