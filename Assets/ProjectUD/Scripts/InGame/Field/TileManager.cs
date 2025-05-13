using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab; // 타일 프리팹
    [SerializeField] private Transform tileParent;
    [SerializeField] private int gridWidth = 40;
    [SerializeField] private int gridHeight = 12;
    [SerializeField] private int tileSize = 1; // 타일 간격
    [SerializeField] private MeshRenderer tileMeshRenderer; // 타일 메쉬 렌더러

    private Vector3 cellSize = new Vector3(2f, 2f, 1f);

    // Start is called before the first frame update
    void Start()
    {
        tileMeshRenderer.sortingOrder = -1;
        GenerateTileMap();
    }

    private void GenerateTileMap()
    {
        for (int x = -55; x < gridWidth; x+=2)
        {
            for (int y = -15; y < gridHeight; y+=2)
            {
                Vector3 spawnPosition = new Vector3(x * tileSize, 0, y * tileSize);
                Instantiate(tilePrefab, spawnPosition, Quaternion.identity, tileParent);
            }
        }
    }
}
