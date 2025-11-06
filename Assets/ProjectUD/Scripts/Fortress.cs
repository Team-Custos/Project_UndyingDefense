using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static WaveManager;

public class Fortress : MonoBehaviour
{
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private EnemyUnitSpawner enemyUnitSpawner;
    [SerializeField] private AllyUnitSpawner allyUnitSpawner;
    [SerializeField] private WaveManager waveManager;

    [Header("■ UI")]
    [SerializeField] private IngameScreenUI ingameUI;

    [Header("■ Options")]
    [SerializeField] private float maxHp;
    [SerializeField] private PositionsParentCtrl hitPositions;
    [SerializeField] private ListedPositions linePositions;

    [Header("■ Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] damageSound;
    
    private bool isFortressAttacked = false;


    private float hp;

    private void Start()
    {
        hp = maxHp;
        ingameUI.SetHP(hp, maxHp);
    }

    public void TakeDamage(float damage)
    {
        if(hp <= 0f)
            return;

        hp -= damage;

        if (!isFortressAttacked)
        {
            ingameUI.ShowPerWaveNotice();
            isFortressAttacked = true;
        }

        if (hp <= 0f)
        {
            hp = 0f;
            // 게임 오버

            ingameUI.ShowResult(0, false);
            SoundManager.Instance.StopBGM();

            inGameManager.LoseGame();
            //inGameManager.WinGame();
            //waveManager.PlayLoseSfx();

            
        }

        //waveManager.OnFortressAttacked();

        ingameUI.SetHP(hp, maxHp);

        if(hp > 0f)
            SoundManager.Instance.PlaySFX(this.transform.position, damageSound);
        //audioSource.PlayOneShot(damageSound[Random.Range(0, damageSound.Length)]);//데미지 사운드 출력
    }

    public Vector3 GetPosition(int index)
    {
        return hitPositions.Position(index);
        //return linePositions[index];
    }

    public void ResetFortressState()
    {
        isFortressAttacked = false;
    }
}
