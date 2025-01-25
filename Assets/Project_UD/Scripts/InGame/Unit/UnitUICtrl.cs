using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitUICtrl: MonoBehaviour
{
    // 유닛에 필요한 UI를 관리하는 스크립트(hp바, 선택 ui, 이동 ui, 소환 이펙트 등)

    Ingame_UnitCtrl unitCtrl;
    
    [Header("Unit Select Ui")]
    public Image hpBar;                     // hp바
    public GameObject selectedUI;      // 선택 ui

    [Header("Unit Summon Effect")]
    public GameObject spawnEffect;          // 소환 이펙트
    public GameObject summonParticle;       // 스폰 이펙트

    public float effectDuration = 2.0f;
    public Image unitMoveImage;

    public GameObject siegeEffect;

    public UnitState unitState;

    // Start is called before the first frame update
    void Start()
    {
        unitCtrl = GetComponent<Ingame_UnitCtrl>();

        PlaySummonEffect();
    }

    // Update is called once per frame
    void Update()
    {
        OnOffSelectedUnitUI();
    }


    // 선택된 유닛 표시 ui on/off 기능 (hp, 선택 ui)
    public void OnOffSelectedUnitUI()
    {
        if(this.gameObject == GameOrderSystem.instance.selectedUnit)
        {
            hpBar.gameObject.SetActive(true);
            selectedUI.SetActive(true);
        }
        else
        {
            hpBar.gameObject.SetActive(false);
            selectedUI.SetActive(false);
        }
    }


    // 유닛이 이동시 나타나는 이미지
    public void OnOffUnitMoveUI(bool isMove = true)
    {
        if(isMove)
        {
            unitMoveImage.gameObject.SetActive(isMove);
        }
        else
        {
            unitMoveImage.gameObject.SetActive(!isMove);
        }
    }


    // 유닛 소환 이펙트, 업그레이드 이펙트
    void PlaySummonEffect()
    {
        if (spawnEffect != null && !UnitUpgradeManager.Instance.isUpgrade)
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