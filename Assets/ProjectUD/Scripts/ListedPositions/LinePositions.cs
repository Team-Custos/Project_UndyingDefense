using UnityEngine;

public class LinePositions : ListedPositions
{
    [SerializeField] private float intervalDistance;

    public override Vector3 this[int index]
    {
        get
        {
            if(index >= count)   
                index = index % count;

            return transform.position + (transform.forward * index * intervalDistance);
        }
    }
}
