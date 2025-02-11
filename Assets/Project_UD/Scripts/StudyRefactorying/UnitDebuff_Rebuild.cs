using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitDebuff
{
    None,
    Bleed,
    Dizzy,
    Stun,
    Trapped,
    Burn,
    Inferno,
    Poison
}

public abstract class UnitDebuff_Rebuild : MonoBehaviour
{
    public UnitDebuff Debuff; //이름
    public int stackLimit; //스택 최대 수
    public float duration; //지속 시간
    private float Cur_duration = 0; //현재 지속 시간

    protected UnitCtrl_ReBuild target;

    public abstract void ApplyDebuff(UnitCtrl_ReBuild target);

    public virtual void RemoveDebuff()
    {
        //디버프 해제
        Destroy(gameObject);
    }

    public UnitCtrl_ReBuild GetTarget()
    {
        return target;
    }

    public void SetTarget(UnitCtrl_ReBuild target)
    {
        if (target != null)
        {
            this.target = target;
        }
    }


    private void Awake()
    {
        Cur_duration = duration;
    }

    private void Start()
    {
        
    } 

    public virtual void Update()
    {
        Cur_duration -= Time.deltaTime;
        Debug.Log("Debuff Duration : " + Cur_duration);
        if (Cur_duration <= 0)
        {
            Debug.Log("Debuff End");
            RemoveDebuff();
        }
    }
}
