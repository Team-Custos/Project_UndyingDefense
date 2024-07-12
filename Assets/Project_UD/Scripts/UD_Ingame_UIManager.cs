using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UD_Ingame_UIManager : MonoBehaviour
{
    public static UD_Ingame_UIManager instance;

    public Button allyUnitSetMode = null;

<<<<<<< Updated upstream
    public int gitTest1 = 0;


=======
    public GameObject unitDeployCheckBox = null;

    private void Awake()
    {
        instance = this;
    }
>>>>>>> Stashed changes

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

    public void CreateDeployCheckBox(Vector3 pos)
    {
        GameObject unitDeploy = Instantiate(unitDeployCheckBox) as GameObject;
        unitDeploy.transform.position = pos;
    }
}
