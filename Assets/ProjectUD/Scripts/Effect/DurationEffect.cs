using UnityEngine;

<<<<<<< HEAD
=======
<<<<<<< Updated upstream
>>>>>>> KimJK
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
             target.EffectList.Remove(this);
             target.UpdateState();

            onRemove.Invoke();
            onRemove.Clear();
        }

        stack = 0;
        gameObject.SetActive(false);
<<<<<<< HEAD
=======
=======
public abstract class DurationEffect : MonoBehaviour
{
    [Header("■ Data")]
    [SerializeField] protected string id;
    [SerializeField] protected string effectName;
    [SerializeField, TextArea] protected string description;

    [Header("■ Options")]
    [SerializeField] protected float duration;

    protected float durationCheck;      // 지속 시간을 체크하는 변수
    protected GameObject prefab;
    protected Unit target;
    protected ObjectPoolWithList<DurationEffect> pool;

    public string Name => effectName;
    public string Description => description;
    public GameObject Prefab => prefab;

    public void Initialize(GameObject prefab, ObjectPoolWithList<DurationEffect> pool)
    {
        this.prefab = prefab;
        this.pool = pool;
    }

    public virtual void SetTarget(Unit target) // 처음 효과가 유닛에 추가되었을 때
    {
        this.target = target;
    }

    public virtual void Reapply(GameObject effectPrefab) // 효과를 재적용하는 함수.
    {
        durationCheck = 0f;
    }

    protected virtual void Update()
    {
        durationCheck += Time.deltaTime;
        if (durationCheck >= duration)
        {
            durationCheck = 0f;

            target.RemoveEffect(this);
            Remove();
            transform.SetParent(null);
            gameObject.SetActive(false);
        }
    }

    public abstract void Activate();
    public abstract void Remove();

    private void OnDisable()
    {
        if (pool == null)
            return;

        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);

        pool.List.Remove(this);
        pool.Pool.Release(this);
>>>>>>> Stashed changes
>>>>>>> KimJK
    }
}
