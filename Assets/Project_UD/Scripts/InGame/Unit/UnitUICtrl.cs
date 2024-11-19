using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUICtrl : MonoBehaviour
{
    public GameObject selecteParticle;
    public Image unitHp;


    // Start is called before the first frame update
    void Start()
    {
        
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
}
