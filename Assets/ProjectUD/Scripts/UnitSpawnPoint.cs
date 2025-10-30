using UnityEngine;

public class UnitSpawnPoint : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;

    private AllyUnit unit;
    private ObjectPoolWithList<UnitSpawnPoint> pool;

    private bool isUnitSpawned;
    private float timeCheck;
    private const float spawnTime = 3f;

    [SerializeField] private AudioClip allySpawn;

    public void Initialize(ObjectPoolWithList<UnitSpawnPoint> pool)
    {
        this.pool = pool;
    }

    public void Initialize(AllyUnit unit)
    {
        this.unit = unit;
        isUnitSpawned = false;
        timeCheck = 0f;
    }

    private void Update()
    {
        if(!isUnitSpawned)
        {
            if (timeCheck < spawnTime)
            {
                timeCheck += Time.deltaTime;
            }
            else
            {
                isUnitSpawned = true;
                unit.transform.position = transform.position;
                unit.Initialize();

                unit.gameObject.SetActive(true);
                SoundManager.Instance.PlaySFX(allySpawn, this.transform.position);
            }
        }

        if (ps.isStopped)
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (pool == null)
            return;

        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);

        pool.Pool.Release(this);
        pool.List.Remove(this);
    }
}
