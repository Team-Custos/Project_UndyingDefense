using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 성을 관리 하기위한 스크립트입니다.
public class BaseStatus : MonoBehaviour
{
    public int BaseHPMax = 0;
    public int BaseHPCur = 0;

    public static BaseStatus instance;

    public BoxCollider baseBoxCollider;


    private void Awake()
    {
        instance = this;
        baseBoxCollider = GetComponent<BoxCollider>();
    }


    // Start is called before the first frame update
    void Start()
    {
        BaseHPCur = BaseHPMax;
    }

    // Update is called once per frame
    void Update()
    {
        //if (BaseHPCur <= 0 && !isBaseHpZero)
        //{
        //    Debug.Log("Base HP reached 0.");
        //    isBaseHpZero = true; // 플래그 설정을 먼저 수행
        //    BaseHPCur = 0;
        //    Time.timeScale = 0.0f;
        //    SoundManager.instance.PlayWaveSFX(SoundManager.waveSfx.sfx_battleWin);
        //    Ingame_WaveUIManager.instance.waveResultLosePanel.SetActive(true);
        //}
    }

    public Vector3 GetNearestPosition(Vector3 from)
    {
        return baseBoxCollider.ClosestPointOnBounds(from);
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

        if (BaseHPCur <= 0)
        {
            InGameManager.inst.isGamePause = true;
            BaseHPCur = 0;
            SoundManager.instance.PlayWaveSFX(SoundManager.waveSfx.sfx_battleLose);
            Ingame_WaveUIManager.instance.waveResultLosePanel.SetActive(true);
        }

        EnemySpawner.inst.OnBaseAttacked(); // Base 공격 알림
    }
}