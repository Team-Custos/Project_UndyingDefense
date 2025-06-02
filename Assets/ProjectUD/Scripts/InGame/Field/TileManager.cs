using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tileParent;
    [SerializeField] private int xPos = -49;
    [SerializeField] private int yPos = -15;
    [SerializeField] private int gridWidth = 40;
    [SerializeField] private int gridHeight = 12;
    [SerializeField] private int tileSize = 1;
    [SerializeField] private MeshRenderer tileMeshRenderer;
    [SerializeField] private LayerMask obstacleLayer;
    private Vector3 checkBoxSize = new Vector3(1f, 1f, 1f);


    // Start is called before the first frame update
    void Start()
    {
        tileMeshRenderer.sortingOrder = -1;
        GenerateTileMap();
    }

    private void GenerateTileMap()
    {
        for (int x = xPos; x < gridWidth; x+=2)
        {
            for (int y = yPos; y < gridHeight; y+=2)
            {
                Vector3 tilePosition = new Vector3(x * tileSize, 0, y * tileSize);


                if (!Physics.CheckBox(tilePosition, checkBoxSize * 0.5f, Quaternion.identity, obstacleLayer))
                {
                    Instantiate(tilePrefab, tilePosition, Quaternion.identity, tileParent);
                }
            }
        }
    }
}
