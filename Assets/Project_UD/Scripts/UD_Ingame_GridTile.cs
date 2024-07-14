using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_GridTile : MonoBehaviour
{
    UD_Ingame_GameManager GAMEMANAGER;

    public bool Selected = false;
    public Vector2 GridPos = Vector2.zero;

    public Color32 colorDefault = Color.white;
    public Color32 colorHighlit = Color.white;

    public Color32 colorSelected = Color.white;

    MeshRenderer MeshR;

    public GameObject currentPlacedUnit;

    bool mouseHover = false;
    public bool isPlaceable = true;


    // Start is called before the first frame update
    void Start()
    {
        GAMEMANAGER = UD_Ingame_GameManager.inst;
        MeshR = GetComponent<MeshRenderer>();
        Selected = false;

        GridPos = new Vector2((int)(transform.position.x / 2), (int)(transform.position.z / 2));

        this.gameObject.name = this.name + " " + GridPos.x + " " + GridPos.y;
    }

    // Update is called once per frame
    void Update()
    {
        


        if (Selected)
        {
            MeshR.material.color = colorSelected;
        }
        else if(!mouseHover)
        {
            MeshR.material.color = colorDefault;
        }
        
        

    }

    private void OnMouseOver()
    {
        mouseHover = true;
        MeshR.material.color = colorHighlit;
    }


    private void OnMouseExit()
    {
        mouseHover = false;
        GetComponent<MeshRenderer>().material.color = colorDefault;
    }



    private void OnMouseDown()
    {
        if (GAMEMANAGER.AllyUnitSetMode && isPlaceable)
        {
            
            isPlaceable = false;
        }
        else
        {
            Selected = !Selected;
        }


        //Debug.Log(gameObject.name + " Selected");
    }
}
