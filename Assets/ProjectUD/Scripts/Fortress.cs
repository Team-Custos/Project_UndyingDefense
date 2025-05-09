using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Fortress : MonoBehaviour
{
    [SerializeField] private EnemyUnitSpawner enemyUnitSpawner;

    [Header("■ UI")]
    [SerializeField] private IngameScreenUI ingameUI;

    [Header("■ Options")]
    [SerializeField] private float maxHp;
    [SerializeField] private ListedPositions linePositions;

    [Header("■ Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] damageSound;

    private bool isGameOver = true;

    private float hp;

    private void Start()
    {
        hp = maxHp;
        ingameUI.SetHP(hp, maxHp);
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        //데미지를 입을때 사운드 출력

        if (hp <= 0f)
        {
            hp = 0f;
            // 게임 오버

            if(isGameOver)
            {
                ingameUI.ShowResult(0, false);
                enemyUnitSpawner.GameLose();
                isGameOver = false;
            }
        }

        enemyUnitSpawner.OnFortressAttacked();

        ingameUI.SetHP(hp, maxHp);

        if(hp > 0f)
            audioSource.PlayOneShot(damageSound[Random.Range(0, damageSound.Length)]);//데미지 사운드 출력
    }

    public Vector3 GetPosition(int index)
    {
        return linePositions[index];
    }

    
}
