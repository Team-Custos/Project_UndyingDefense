using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitGrid : MonoBehaviour
{
    [SerializeField] private AllyUnit allyUnit;

    public AllyUnit AllyUnit => allyUnit;
    public List<Tile> overlappedTiles { set; get; } = new List<Tile>();
    private Tile targetTile;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tile"))
        {
            Tile tile = other.GetComponent<Tile>();
            if (tile != null && !overlappedTiles.Contains(tile))
            {
                overlappedTiles.Add(tile);
                //Debug.Log("Enter 타일: " + tile.transform.position);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Tile"))
        {
            Tile tile = other.GetComponent<Tile>();
            if (tile != null && overlappedTiles.Contains(tile))
            {
                overlappedTiles.Remove(tile);
            }
        }
    }

    public Tile GetAvailableTile()
    {
        List<Tile> availableTiles = new List<Tile>();

        UnitGrid unitGrid = allyUnit.GetComponentInChildren<UnitGrid>();

        // 비어있는 타일 확인
        for (int i = 0; i < overlappedTiles.Count; i++)
        {
            Tile tile = overlappedTiles[i];
            if (tile != null)
            {
                // 타일이 비어있다면
                if (tile.TileAllyUnit == null)
                {
                    availableTiles.Add(tile);
                }
                //else
                //{
                //    Debug.Log("이동 불가 : " + tile.transform.position);
                //}
            }
        }


        if (availableTiles.Count <= 0)
            return null;


        // 비어있는 타일중 가까운 곳으로
        availableTiles.Sort((a, b) => Vector3.Distance(transform.position, a.transform.position)
            .CompareTo(Vector3.Distance(transform.position, b.transform.position)));

        // 거리에 따라 4개만
        //if (availableTiles.Count > 4)
        //{
        //    availableTiles = availableTiles.GetRange(0, 4);
        //}

        availableTiles[0].SetAllyUnit(allyUnit);

        targetTile = availableTiles[0];

        return targetTile;
    }

    

    public void ClearTile()
    {
        if(targetTile != null)
        {

            targetTile.ClearUnit();
            targetTile = null;
        }
    }

    public void SetTargetTile(Tile tile)
    {
        targetTile = tile;
    }
}
