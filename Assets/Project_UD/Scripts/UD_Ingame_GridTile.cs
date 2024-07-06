using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_GridTile : MonoBehaviour
{
    public bool Selected = false;

    public Color32 colorDefault = Color.white;
    public Color32 colorHighlit = Color.white;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        GetComponent<MeshRenderer>().material.color = colorHighlit;
    }

    private void OnMouseExit()
    {
        GetComponent<MeshRenderer>().material.color = colorDefault;
    }

    private void OnMouseDown()
    {
        UD_Ingame_InputSystem.inst.TileClick();
        Debug.Log(gameObject.name);
    }
}
