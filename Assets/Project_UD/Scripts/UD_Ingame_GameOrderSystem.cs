using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UD_Ingame_GameOrderSystem : MonoBehaviour
{
    UD_Ingame_GameManager GAMEMANAGER;

    public GameObject clickedObj = null;
    public GameObject selectedUnit = null;

    public Vector3 clickedWorldPos = Vector3.zero;

    public GameObject clickedPosIndicator = null;


    private void Start()
    {
        GAMEMANAGER = UD_Ingame_GameManager.inst;
        UD_Ingame_InputSystem.Instance.OnPrimaryPerformed += OnPrimaryButtonOrder;
    }

    private void OnDestroy()
    {
        UD_Ingame_InputSystem.Instance.OnPrimaryPerformed -= OnPrimaryButtonOrder;
    }

    private void OnPrimaryButtonOrder()
    {
        //Debug.Log("OnPrimaryButtonOrder");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit))
        {
            clickedObj = hit.collider.gameObject;
            clickedWorldPos = hit.point;



            //Ÿ�� Ŭ��������
            if (clickedObj.tag == UD_CONSTANT.TAG_TILE)
            {
                UD_Ingame_GridTile GridTile = hit.collider.GetComponent<UD_Ingame_GridTile>();

                if (GridTile.currentPlacedUnit != null)
                {
                    GridTile.currentPlacedUnit.transform.Translate(Vector3.forward);
                }
                else if (GAMEMANAGER.AllyUnitSetMode && GridTile.isPlaceable)
                {
                    GridTile.currentPlacedUnit = UD_Ingame_UnitSpawnManager.inst.UnitSpawn(true, this.transform.position.x, this.transform.position.z);
                    GridTile.isPlaceable = false;
                }
                else
                {
                    Debug.Log("s");
                    GridTile.Selected = !GridTile.Selected;
                }
            }
            //���� Ŭ��������
            else if (clickedObj.tag == UD_CONSTANT.TAG_UNIT)
            {
                UD_Ingame_UnitCtrl AllyUnit = hit.collider.GetComponent<UD_Ingame_UnitCtrl>();

                AllyUnit.isSelected = !AllyUnit.isSelected;

                if (AllyUnit.isSelected)
                {
                    selectedUnit = AllyUnit.gameObject;
                }
                else
                {
                    selectedUnit = null;
                }


            }
            else if (clickedObj.tag == UD_CONSTANT.TAG_ENEMY)
            {
                if (selectedUnit != null)
                {
                    UD_Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<UD_Ingame_UnitCtrl>();
                    AllyUnit.isSelected = false;
                    AllyUnit.moveTargetPos = new Vector3(clickedWorldPos.x, 0, clickedWorldPos.z);
                    AllyUnit.targetEnemy = clickedObj.gameObject;

                    selectedUnit = null;
                }
            }
            else if (clickedObj.tag == UD_CONSTANT.TAG_GROUND)
            {
                if (clickedPosIndicator != null)
                {
                    clickedPosIndicator.transform.position = clickedWorldPos;
                    clickedPosIndicator.SetActive(true);
                }

                if (selectedUnit != null)
                {
                    UD_Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<UD_Ingame_UnitCtrl>();
                    AllyUnit.isSelected = false;
                    selectedUnit = null;

                    AllyUnit.moveTargetPos = new Vector3(clickedWorldPos.x, 0, clickedWorldPos.z);


                }
            }

            
           
        }
    }

    private void OnSecondaryButtonOrder()
    {
        //To do : Secondary Button Event




    }
}