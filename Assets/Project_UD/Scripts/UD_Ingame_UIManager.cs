using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UD_Ingame_UIManager : MonoBehaviour
{
    public Button allyUnitSetMode = null;
    public Text UnitSetModeText = null;


    // Start is called before the first frame update
    void Start()
    {
        if (allyUnitSetMode != null)
        {
            allyUnitSetMode.onClick.AddListener(() => 
            {
                UD_Ingame_GameManager.inst.AllyUnitSetMode = !UD_Ingame_GameManager.inst.AllyUnitSetMode;
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (UnitSetModeText != null)
        {
            if (UD_Ingame_GameManager.inst.UnitSetMode)
            {
                UnitSetModeText.text = "UnitSetMode : ON";
                if (UD_Ingame_GameManager.inst.AllyUnitSetMode)
                {
                    UnitSetModeText.color = Color.cyan;
                }
                else if (UD_Ingame_GameManager.inst.EnemyUnitSetMode)
                {
                    UnitSetModeText.color = Color.red;
                }
            }
            else
            {
                UnitSetModeText.text = "UnitSetMode : OFF";
                UnitSetModeText.color = Color.white;
            }
        }



    }
}
