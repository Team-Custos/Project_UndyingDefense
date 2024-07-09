using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class UnitSpawnData
{
    public int spriteType;
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
    public static UD_Ingame_UnitSpawnManager inst;

    public UnitSpawnData[] spawnData;

    public GameObject Test_Ally;
    public GameObject Test_Enemy;


    // Start is called before the first frame update
    void Start()
    {
        inst = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //¿Ø¥÷ º“»Ø
    public void UnitSpawn(bool IsAlly , float X, float Y)
    {
        if (IsAlly)
        {
            GameObject Obj = Instantiate(Test_Ally);
            Obj.transform.position = new Vector3(X, 0, Y);
        }
        else
        {
            GameObject Obj = Instantiate(Test_Enemy);
            Obj.transform.position = new Vector3(X, 0, Y);
        }


    }
}
