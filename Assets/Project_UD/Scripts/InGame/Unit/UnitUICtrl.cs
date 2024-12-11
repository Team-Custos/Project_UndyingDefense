using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitUICtrl: MonoBehaviour
{

    public GameObject selecteParticle;
    public GameObject spawnEffect;
    public GameObject summonParticle;


    public float effectDuration = 2.0f;
    public Image unitHp;

    private Vector3 originalScale; // 유닛 원래 크기
    private Vector3 originalPosition; // 유닛 원래 위치

    public GameObject siegeEffect;

    // Start is called before the first frame update
    void Start()
    {

        PlaySummonEffect();
    }

    // Update is called once per frame
    void Update()
    {
        OnOffSelectedUnitUI();
    }


    // 선택된 유닛 표시 ui on/off 기능 (hp, 파티클)
    public void OnOffSelectedUnitUI()
    {
        if(this.gameObject == GameOrderSystem.instance.selectedUnit)
        {
            unitHp.gameObject.SetActive(true);
            selecteParticle.SetActive(true);
        }
        else
        {
            unitHp.gameObject.SetActive(false);
            selecteParticle.SetActive(false);
        }
    }

    void PlaySummonEffect()
    {
        if (spawnEffect != null)
        {
            //spawnEffect.SetActive(true);

            StartCoroutine(DisableSummonEffectAfterDelay(effectDuration));
        }
    }

    private IEnumerator DisableSummonEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(1.0f);

        spawnEffect.SetActive(true);

        yield return new WaitForSeconds(delay);

        spawnEffect.SetActive(false);
    }

    public void OnOffSiegeEffect(bool isSiege = true)
    {
        siegeEffect.SetActive(isSiege);
    }

}