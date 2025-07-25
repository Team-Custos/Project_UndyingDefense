using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectPoolTest : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectpool = new List<GameObject>();

    private ObjectPool<GameObject> pools = new ObjectPool<GameObject>(null, null);

    public void Start()
    {
        
    }

}
