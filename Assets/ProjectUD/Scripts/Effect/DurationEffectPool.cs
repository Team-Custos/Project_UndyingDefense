using System.Collections.Generic;
using UnityEngine;

public class DurationEffectPool : MonoBehaviour
{
    private Dictionary<GameObject, ObjectPoolWithList<DurationEffect>> poolDic 
        = new Dictionary<GameObject, ObjectPoolWithList<DurationEffect>>();

    public DurationEffect GetDurationEffect(GameObject effectPrefab)
    {
        DurationEffect effect = null;
        if (!poolDic.ContainsKey(effectPrefab))
            poolDic.Add(effectPrefab, new ObjectPoolWithList<DurationEffect>(() => CreateDurationEffect(effectPrefab)));

        effect = poolDic[effectPrefab].Pool.Get();
        poolDic[effectPrefab].List.Add(effect);

        return effect;
    }

    private DurationEffect CreateDurationEffect(GameObject effectPrefab)
    {
        GameObject obj = Instantiate(effectPrefab);
        obj.SetActive(false);
        DurationEffect effect = obj.GetComponent<DurationEffect>();
        effect.Initialize(effectPrefab, poolDic[effectPrefab]);

        return effect;
    }
}
