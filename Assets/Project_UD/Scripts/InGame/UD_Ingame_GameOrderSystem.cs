using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && (Input.GetMouseButtonDown(0)))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            clickedObj = hit.collider.gameObject;
            clickedWorldPos = hit.point;

            //타일 클릭했을때
            if (clickedObj.tag == UD_CONSTANT.TAG_TILE)
            {
                UD_Ingame_GridTile GridTile = hit.collider.GetComponent<UD_Ingame_GridTile>();

                if (GAMEMANAGER.UnitSetMode && GridTile.isPlaceable)
                {
                    if (GAMEMANAGER.AllyUnitSetMode)//아군 배치
                    {
                        UD_Ingame_UnitSpawnManager SpawnMgr = UD_Ingame_UnitSpawnManager.inst;
                        GridTile.currentPlacedUnit =
                            SpawnMgr.UnitSpawn(SpawnMgr.unitToSpawn, GridTile.transform.position.x, GridTile.transform.position.z);
                        GridTile.isPlaceable = false;
                        UD_Ingame_GameManager.inst.UnitSetMode = false;
                        UD_Ingame_GameManager.inst.AllyUnitSetMode = false;
                    }
                    else if (GAMEMANAGER.EnemyUnitSetMode)// 적 배치
                    {
                        UD_Ingame_EnemySpawner SpawnMgr = UD_Ingame_EnemySpawner.inst;
                        GridTile.currentPlacedUnit = SpawnMgr.EnemySpawn(1, GridTile.transform.position.x, GridTile.transform.position.z);
                        //GridTile.isPlaceable = false;
                        //UD_Ingame_GameManager.inst.UnitSetMode = false;
                        //UD_Ingame_GameManager.inst.EnemyUnitSetMode = false;
                    }
                }
                else if (GridTile.currentPlacedUnit == null)
                {
                    GAMEMANAGER.AllTileSelectOff();
                    GridTile.Selected = !GridTile.Selected;

                    if (selectedUnit != null)
                    {
                        UD_Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<UD_Ingame_UnitCtrl>();
                        AllyUnit.isSelected = false;

                        if (AllyUnit.Ally_Mode == AllyMode.Free)
                        {
                            AllyUnit.haveToMovePosition = true;
                            AllyUnit.moveTargetPos = GridTile.transform.position;
                        }

                        selectedUnit = null;
                    }

                }
                

            }
            //유닛 클릭했을 때
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
            //적 클릭했을 때
            else if (clickedObj.tag == UD_CONSTANT.TAG_ENEMY)
            {
                UD_Ingame_UnitCtrl Enemy = clickedObj.GetComponent<UD_Ingame_UnitCtrl>();
                Enemy.isSelected = !Enemy.isSelected;

                if (selectedUnit != null)
                {
                    UD_Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<UD_Ingame_UnitCtrl>();
                    AllyUnit.isSelected = false;
                    AllyUnit.targetEnemy = clickedObj.gameObject;

                    if (AllyUnit.Ally_Mode == AllyMode.Free)
                    {
                        AllyUnit.moveTargetPos = new Vector3(Enemy.transform.position.x, 0, Enemy.transform.position.z);
                    }
                    Enemy.isSelected = false;

                    selectedUnit = null;
                }
            }
            //지형 클릭했을 때
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

                    if (AllyUnit.Ally_Mode == AllyMode.Free)
                    {
                        AllyUnit.haveToMovePosition = true;
                        AllyUnit.moveTargetPos = new Vector3(clickedWorldPos.x, 0, clickedWorldPos.z);
                    }

                    selectedUnit = null;
                }
            }
        }
    }

    private void OnSecondaryButtonOrder()
    {
        //To do : Secondary Button Event




    }

    
}