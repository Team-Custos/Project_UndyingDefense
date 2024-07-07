using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    Ally,
    Enemy,
}

public interface IState
{
    public void EnterState();

    public void UpdateState();
    public void ExitState();

}

public class UD_Ingame_UnitCtrl : MonoBehaviour
{
    


    MeshRenderer MeshRenderer;


    public UnitType UnitType;



    public Color32 colorAlly = Color.blue;
    public Color32 colorEnemy = Color.red;


    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (UnitType)
        {
            case UnitType.Enemy:
                MeshRenderer.material.color = colorEnemy;
                break;
            case UnitType.Ally:
                MeshRenderer.material.color = colorAlly;
                break;

        }


    }
}
