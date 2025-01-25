using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBarController : MonoBehaviour
{
    public Image hpBar;
    private Ingame_UnitCtrl unitCtrl;

    [SerializeField] private int maxHp;
    [SerializeField] private int HP;
    private float reverseHp;

    // Start is called before the first frame update
    void Start()
    {
        maxHp = GameOrderSystem.instance.selectedUnit.GetComponent<Ingame_UnitCtrl>().unitData.maxHP;
        HP = GameOrderSystem.instance.selectedUnit.GetComponent<Ingame_UnitCtrl>().unitData.maxHP;

        reverseHp = 1 / (float)maxHp;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
