using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingame_ParticleManager : MonoBehaviour
{
    public static Ingame_ParticleManager Instance;

    public ParticleSystem allySummonEffect;
    public ParticleSystem enemySummonEffect;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySummonParticleEffect(Transform tr, bool isAlly)
    {
        ParticleSystem summonEffect;

        // 파티클 아군, 적 구분
        if (isAlly)
        {
            summonEffect = allySummonEffect;
        }
        else
        {
            summonEffect = enemySummonEffect;
        }

        ParticleSystem effectInstance = Instantiate(summonEffect, tr.position, tr.rotation);

        effectInstance.Play();

        Destroy(effectInstance.gameObject, effectInstance.main.duration);
    }
}
