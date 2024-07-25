using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UD_Ingame_GridManager : MonoBehaviour
{
    public int _width, _height;

    [SerializeField] private UD_Ingame_GridTile _tilePrefab;

    Dictionary<Vector2, UD_Ingame_GridTile> _tiles = new Dictionary<Vector2, UD_Ingame_GridTile>
    { };

    public GameObject TILEPARENT;

    public GameObject[] Tiles_Obj;
    int Tiles_idx;

    public float tile_Offset;


 
    private void Start()
    {
        //GenerateGrid();
    }

    private void Update()
    {
        Tiles_Obj = GameObject.FindGameObjectsWithTag("Tile");

        

    }

    public bool IsTilesAllOff()
    {
        if (Tiles_Obj[Tiles_idx].GetComponent<UD_Ingame_GridTile>().Selected)
        {
            return false;
        }
        else { return true; }
    }


    public void GenerateGrid()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var spawnedTile = Instantiate(_tilePrefab, TILEPARENT.transform);
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.GetComponent<UD_Ingame_GridTile>().GridPos = new Vector2(x, y);

                spawnedTile.transform.position = new Vector3(x * tile_Offset, 0, y * tile_Offset);


                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

    }

    public UD_Ingame_GridTile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile))
        { return tile; }

        return null;
    }
}
