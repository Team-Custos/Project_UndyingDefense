using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class EnemySpawnData
{
    public int spriteType;
    public float spawnTime;
    public int HP;
    public float speed;
    public EnemyType enemyType;

}

public enum EnemyType
{
    Basic,
    what
}

public class UD_Ingame_MobSpawner : MonoBehaviour
{
    
    UD_Ingame_GridManager gridManager;

    int mobSpawnPosX = 0;
    int gridHeight = 0;

    int pointidx = 0;

    public Transform[] spawnPoint;
    public EnemySpawnData[] spawnData;

    bool isMobSpawnerPosSet = false;

    int level;
    float timer;

    private void Awake()
    {
       
        
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
                Debug.Log("Idx : " + idx);
                if (gridManager.Tiles_Obj[idx].GetComponent<UD_Ingame_GridTile>().GridPos.x == mobSpawnPosX - 1)
                {
                    Debug.Log("PointIdx : " + pointidx);
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
            Spawn();
        }
    }

    void Spawn()
    {
        //GameObject enemy = Game_Manager.instance.pool.Get(0);
        //enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position;
        //enemy.GetComponent<Enemy_Ctrl>().Init(spawnData[level]);
    }
}
