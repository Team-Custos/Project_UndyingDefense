using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSensor : MonoBehaviour
{

    public List<GameObject> detectedObjects = new List<GameObject>();
    public List<GameObject> ignoreList = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out UD_Ingame_EnemyCtrl unit))
        {
            if (ignoreList.Exists(x => x.gameObject == other.transform.root))
                return;

            detectedObjects.Add(unit.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.TryGetComponent(out UD_Ingame_EnemyCtrl unit))
        {
            if (ignoreList.Exists(x => x.gameObject == other.transform.root))
                return;

            detectedObjects.Remove(unit.gameObject);
        }
    }
}
