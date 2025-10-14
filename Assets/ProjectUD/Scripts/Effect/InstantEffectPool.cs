using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantEffectPool : MonoBehaviour
{
    private Dictionary<GameObject, ObjectPoolWithList<InstantEffect>> poolDic
        = new Dictionary<GameObject, ObjectPoolWithList<InstantEffect>>();

    public InstantEffect GetInstantEffect(GameObject effectPrefab)
    {
        InstantEffect effect = null;
        if (!poolDic.ContainsKey(effectPrefab))
            poolDic.Add(effectPrefab, new ObjectPoolWithList<InstantEffect>(() => CreateInstantEffect(effectPrefab)));

        effect = poolDic[effectPrefab].Pool.Get();
        poolDic[effectPrefab].List.Add(effect);

        return effect;
    }

    private InstantEffect CreateInstantEffect(GameObject effectPrefab)
    {
        GameObject obj = Instantiate(effectPrefab);
        obj.SetActive(false);
        InstantEffect effect = obj.GetComponent<InstantEffect>();
        effect.Initialize(poolDic[effectPrefab]);

        return effect;
    }
}
