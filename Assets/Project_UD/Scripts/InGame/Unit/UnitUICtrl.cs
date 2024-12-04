using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUICtrl : MonoBehaviour
{

    public GameObject selecteParticle;
    public GameObject spawnEffect;
    public GameObject summonParticle;


    public float effectDuration = 2.0f;
    public Image unitHp;


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
            // 소환 효과 활성화
            spawnEffect.SetActive(true);

            // 코루틴 시작
            StartCoroutine(DisableSummonEffectAfterDelay(effectDuration));
        }
    }

    private IEnumerator DisableSummonEffectAfterDelay(float delay)
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(delay);

        // 소환 효과 비활성화
        spawnEffect.SetActive(false);
    }

}
