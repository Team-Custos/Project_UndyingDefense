using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tileParent;
    [SerializeField] private int gridWidth = 40;
    [SerializeField] private int gridHeight = 12;
    [SerializeField] private int tileSize = 1;
    [SerializeField] private MeshRenderer tileMeshRenderer;
    [SerializeField] private LayerMask obstacleLayer;
    private Vector3 checkBoxSize = new Vector3(1f, 1f, 1f);

    [SerializeField] private Vector3 startPosition = new Vector3(-49, 0, -15); // 타일 생성 시작 좌표


    // Start is called before the first frame update
    void Start()
    {
        tileMeshRenderer.sortingOrder = -1;
        GenerateTileMap();
    }

    private void GenerateTileMap()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 tilePosition = startPosition + new Vector3(x * tileSize, 0, y * tileSize);

                // 장애물 체크 후 타일 생성
                if (!Physics.CheckBox(tilePosition, checkBoxSize * 0.5f, Quaternion.identity, obstacleLayer))
                {
                    Instantiate(tilePrefab, tilePosition, Quaternion.identity, tileParent);
                }
            }
        }

    }
}
