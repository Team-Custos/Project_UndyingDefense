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

public abstract class UnitEffect : MonoBehaviour
{
    [SerializeField] private UnitDebuff debuffType;
    [SerializeField] private int stackLimit; //스택 최대 수
    private int curStack = 1; //현재 스택 수
    public float duration; //지속 시간
    private float Cur_duration = 0; //현재 지속 시간
    [SerializeField] private UnitEffect NextEffect; //승격 효과
    [SerializeField] private GameObject[] vfxByStack; //스택에 따른 VFX. 

    protected Unit target;

    public abstract void ApplyDebuff(Unit target);

    public virtual void RemoveDebuff()
    {
        //디버프 해제
        target.RemoveEffect(this);
        Destroy(gameObject);
    }

    public Unit GetTarget()
    {
        return target;
    }

    public void SetTarget(Unit target)
    {
        if (target != null)
        {
            this.target = target;
        }
    }

    public bool IsSameEffect(UnitEffect effect)
    {
        return effect.debuffType == debuffType;
    }

    public void ChangeVFX(GameObject NextVFX)
    {
        Destroy(transform.GetChild(0).gameObject);
        GameObject vfxObj = Instantiate(NextVFX);
        vfxObj.transform.parent = transform;
        vfxObj.transform.localPosition = Vector3.zero;
    }

    public void addStack()
    {
        curStack++;
        if (curStack >= stackLimit)
        {
            curStack = stackLimit;
            if (NextEffect != null)
            {
                target.RemoveEffect(this);
                target.AddEffect(NextEffect);
            }
        }
        else
        {
            if (vfxByStack.Length > 0)
            {
                ChangeVFX(vfxByStack[curStack - 2]);
            }
        }
    }

    public void InitDuration()
    {
        Cur_duration = duration;
    }


    private void Awake()
    {
        Cur_duration = duration;
    }
    protected virtual void Update()
    {
        Cur_duration -= Time.deltaTime;
        //Debug.Log("Debuff Duration : " + Cur_duration);
        if (Cur_duration <= 0)
        {
            Debug.Log("Debuff End");
            RemoveDebuff();
        }
    }
}
