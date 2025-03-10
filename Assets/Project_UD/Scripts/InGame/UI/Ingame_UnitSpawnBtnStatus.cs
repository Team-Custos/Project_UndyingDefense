using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//이 스크립트는 배치할 병사의 버튼을 관리하기 위한 스크립트입니다.

public class Ingame_UnitSpawnBtnStatus : MonoBehaviour
{
    private Button button;
    public Text costText;

    public Color Color_UnableToCost = Color.red;
    public Color Color_ableToCost = Color.white;

    public bool AbleToSpawn = true;
    public UnitType currentUnitType = 0;//현재 버튼으로 스폰 시킬 병사.

    private void Start()
    {
        button = GetComponent<Button>();
        costText = GetComponentInChildren<Text>();

    }

    private void Update()
    {
        button.interactable = (UnitSpawnManager.inst.unitDatas[currentUnitType.GetHashCode()].cost <= InGameManager.inst.gold);

        if (button.interactable)
        {
            costText.color = Color_ableToCost;
        }
        else
        {
            costText.color = Color_UnableToCost;
        }

        costText.text = UnitSpawnManager.inst.unitDatas[currentUnitType.GetHashCode()].cost.ToString();
    }

}
