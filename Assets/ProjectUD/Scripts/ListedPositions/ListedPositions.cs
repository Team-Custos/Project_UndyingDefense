using UnityEngine;

public abstract class ListedPositions : MonoBehaviour
{
    [SerializeField] protected int count;
    public abstract Vector3 this[int index] { get; }
}
