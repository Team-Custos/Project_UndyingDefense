using UnityEngine;
using UnityEngine.UI;

public class Fortress : MonoBehaviour
{
    [Header("■ UI")]
    [SerializeField] private IngameScreenUI ingameUI;

    [Header("■ Options")]
    [SerializeField] private float maxHp;
    [SerializeField] private ListedPositions linePositions;

    private float hp;

    public void TakeDamage(float damage)
    {
        hp -= damage;
        //데미지를 입을때 사운드 출력

        if (hp <= 0f)
        {
            hp = 0f;
            // 게임 오버
        }

        ingameUI.SetHP(hp, maxHp);
    }

    public Vector3 GetPosition(int index)
    {
        return linePositions[index];
    }
}
