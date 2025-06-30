using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionsParentCtrl : MonoBehaviour
{
    [SerializeField] private Transform[] hitPositions;

    public Vector3 Position(int index)
    {
        if (index >= hitPositions.Length)
            index = index % hitPositions.Length;

        return hitPositions[index].position;
    }
}
