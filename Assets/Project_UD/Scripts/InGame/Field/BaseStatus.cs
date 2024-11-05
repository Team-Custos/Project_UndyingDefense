using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 성을 관리 하기위한 스크립트입니다.
public class BaseStatus : MonoBehaviour
{
    public int BaseHPMax = 0;
    public int BaseHPCur = 0;

    public static BaseStatus instance;

    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        BaseHPCur = BaseHPMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (BaseHPCur <= 0)
        {
            BaseHPCur = 0;
        }
    }

    public void ReceiveDamage(int Damage)//성이 데미지 받았을때의 함수
    {
        IEnumerator HitEffect()//피격 이펙트
        {
            this.GetComponent<MeshRenderer>().material.color = new Color32(255, 0, 0, 255);

            yield return new WaitForSeconds(0.1f);

            this.GetComponent<MeshRenderer>().material.color = new Color32(255, 255, 255, 255);
        }

        StartCoroutine(HitEffect());

        BaseHPCur -= Damage;
    }
}