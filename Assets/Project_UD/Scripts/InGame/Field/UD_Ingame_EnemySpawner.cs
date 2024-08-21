using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class EnemySpawnData
{
    public int modelType;
    public float spawnTime;
    public int HP;
    public float speed;
    public int atk;
    public float sightRange;
    public float attackRange;

    public int generalSkill;
    public int specialSkill;
    public UnitType enemyType;

}

//public enum EnemyType
//{
//    Warrior,
//    Archer,
//}

public class UD_Ingame_EnemySpawner : MonoBehaviour
{
    public static UD_Ingame_EnemySpawner inst;

    UD_Ingame_GridManager gridManager;

    int mobSpawnPosX = 0;
    int gridHeight = 0;

    int pointidx = 0;

    public Transform[] spawnPoint;
    public EnemySpawnData[] spawnData;

    bool isMobSpawnerPosSet = false;

    int level;
    float timer;

    public GameObject Test_Enemy;

    public int enemyToSpawn = 0;

    private void Awake()
    {
       inst = this;
        
    }

    private void Start()
    {
        gridManager = UD_Ingame_GameManager.inst.gridManager;

        mobSpawnPosX = gridManager._width;
        gridHeight = gridManager._height;

        spawnPoint = new Transform[gridHeight];
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMobSpawnerPosSet)
        {
            for (int idx = 0; idx < gridManager.Tiles_Obj.Length; idx++)
            {
                //Debug.Log("Idx : " + idx);
                if (gridManager.Tiles_Obj[idx].GetComponent<UD_Ingame_GridTile>().GridPos.x == mobSpawnPosX - 1)
                {
                    //Debug.Log("PointIdx : " + pointidx);
                    spawnPoint[pointidx] = gridManager.Tiles_Obj[idx].transform;
                    if (pointidx >= gridHeight - 1)
                    {
                        isMobSpawnerPosSet = true;
                    }
                    else
                    {
                        pointidx++;
                    }
                    continue;
                }
            }
        }

        

        timer += Time.deltaTime;
        //level = Mathf.FloorToInt(Game_Manager.instance.gameTime / 10f);

        if (level >= spawnData.Length - 1)
        {
            level = spawnData.Length - 1;
        }

        if (timer > spawnData[level].spawnTime)
        {
            timer = 0;
            //EnemySpawn();
        }
    }

    //Àû ¼ÒÈ¯
    public GameObject EnemySpawn(int enemyType,float X, float Y)
    {
        Debug.Log(new Vector3(X, 0, Y));
        GameObject Obj = Instantiate(Test_Enemy);
        Obj.transform.position = new Vector3(X, 0, Y);
        Obj.GetComponent<UD_Ingame_UnitCtrl>().unitPos = new Vector2(X, Y);
        Obj.GetComponent<UD_Ingame_UnitCtrl>().EnemyInit(spawnData[enemyType]);
        return Obj;
    }
}
