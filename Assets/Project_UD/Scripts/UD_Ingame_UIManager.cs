using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UD_Ingame_UIManager : MonoBehaviour
{
    public Button allyUnitSetMode = null;


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
        if (UD_Ingame_GameManager.inst.AllyUnitSetMode)
        {
            allyUnitSetMode.GetComponentInChildren<Text>().text = "Ally Unit Set Mode OFF";
        }
        else
        {
            allyUnitSetMode.GetComponentInChildren<Text>().text = "Ally Unit Set Mode ON";
        }


    }
}
