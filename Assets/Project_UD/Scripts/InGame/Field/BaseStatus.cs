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

    public InGame_BGMManager bGMManager;


    private void Awake()
    {
        instance = this;
        baseBoxCollider = GetComponent<BoxCollider>();
        if (baseBoxCollider == null)
        {
            Debug.LogError("BoxCollider가 없습니다.");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        BaseHPCur = BaseHPMax;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3 GetNearestPosition(Vector3 from)
    {
        return baseBoxCollider.ClosestPointOnBounds(from);
    }

    public void ReceiveDamage(int Damage)//성이 데미지 받았을때의 함수
    {
        StartCoroutine(HitEffect());

        BaseHPCur -= Damage;

        if (BaseHPCur <= 0)
        {
            BaseHPCur = 0;
            OnBaseDestroyed();
        }

        WaveManager.inst.OnBaseAttacked(); // Base 공격 알림
    }

    private IEnumerator HitEffect()//피격 이펙트
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.LogError("MeshRenderer가 없습니다.");
            yield break;
        }

        meshRenderer.material.color = new Color32(255, 0, 0, 255);
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material.color = new Color32(255, 255, 255, 255);
    }

    private void OnBaseDestroyed()
    {
        InGameManager.inst.isGamePause = true;

        if (bGMManager != null)
        {
            bGMManager.PauseBGM();
        }

        SoundManager.instance.PlayWaveSFX(SoundManager.waveSfx.sfx_battleLose);
        //Ingame_WaveUIManager.instance.waveResultLosePanel.SetActive(true);
    }
}