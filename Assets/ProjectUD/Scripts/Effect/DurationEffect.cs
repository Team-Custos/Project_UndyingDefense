using UnityEngine;

public class DurationEffect : Effect
{
    [Header("■ Duration Effect Options")]
    [SerializeField] protected float duration;

    protected float durationCheck;      // 지속시간을 체크하는 변수

    protected virtual void Update()
    {
        if (durationCheck < duration)
        {
            durationCheck += Time.deltaTime;
        }
        else
        {
            durationCheck -= duration;
            if (endVFX != null)
            {
                GameObject endVFXObj = Instantiate(endVFX.gameObject);
                endVFXObj.transform.localPosition = Vector3.up;
            }
            Remove();
        }
    }

    public override void Activate() // 효과를 발동할 때
    {
        base.Activate();
        durationCheck = 0f;
    }

    public override void Remove()
    {
        if (onRemove != null)
        {
            // target.EffectList.Remove(this);
            // target.UpdateState();

            onRemove.Invoke();
            onRemove.Clear();
        }

        stack = 0;
        gameObject.SetActive(false);
    }
}
