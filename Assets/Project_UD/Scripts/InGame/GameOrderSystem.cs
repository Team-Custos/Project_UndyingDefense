using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GameOrderSystem : MonoBehaviour
{
    InGameManager GAMEMANAGER;

    public GameObject clickedObj = null;
    public GameObject selectedUnit = null;


    public Vector3 clickedWorldPos = Vector3.zero;

    public GameObject clickedPosIndicator = null;

    private void Start()
    {
        GAMEMANAGER = InGameManager.inst;
        Ingame_InputSystem.Instance.OnPrimaryPerformed += OnPrimaryButtonOrder;
    }

    private void OnDestroy()
    {
        Ingame_InputSystem.Instance.OnPrimaryPerformed -= OnPrimaryButtonOrder;
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
            if (clickedObj.tag == CONSTANT.TAG_TILE)
            {

                GridTile GridTile = clickedObj.GetComponent<GridTile>();
                //Debug.Log("클릭한 그리드 좌표 : " + GridTile.GridPos + ", 배치 가능 여부 : " + GridTile.isPlaceable);

                if (GAMEMANAGER.UnitSetMode && GridTile.isPlaceable)
                {
                    if (GAMEMANAGER.AllyUnitSetMode)//아군 배치
                    {
                        UnitSpawnManager SpawnMgr = UnitSpawnManager.inst;
                        GridTile.currentPlacedUnit = SpawnMgr.UnitSpawn(SpawnMgr.unitToSpawn, GridTile.transform.position.x, GridTile.transform.position.z);
                        //GridTile.isPlaceable = false;
                        InGameManager.inst.UnitSetMode = false;
                        InGameManager.inst.AllyUnitSetMode = false;
                    }
                    else if (GAMEMANAGER.EnemyUnitSetMode)// 적 배치
                    {
                        EnemySpawner SpawnMgr = EnemySpawner.inst;
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
                        Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<Ingame_UnitCtrl>();
                        AllyUnit.isSelected = false;

                        if (AllyUnit.Ally_Mode == AllyMode.Free)
                        {
                            AllyUnit.moveTargetPos = GridTile.transform.position;
                            AllyUnit.targetEnemy = null;
                            AllyUnit.haveToMovePosition = true;
                        }

                        selectedUnit = null;
                    }

                }
            }
            //유닛 클릭했을 때
            else if (clickedObj.tag == CONSTANT.TAG_UNIT)
            {
                Ingame_UnitCtrl AllyUnit = hit.collider.GetComponent<Ingame_UnitCtrl>();

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
            else if (clickedObj.tag == CONSTANT.TAG_ENEMY)
            {
                Ingame_UnitCtrl Enemy = clickedObj.GetComponent<Ingame_UnitCtrl>();
                Enemy.isSelected = !Enemy.isSelected;

                if (selectedUnit != null)
                {
                    Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<Ingame_UnitCtrl>();
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
            else if (clickedObj.tag == CONSTANT.TAG_GROUND)
            {
                if (clickedPosIndicator != null)
                {
                    clickedPosIndicator.transform.position = clickedWorldPos;
                    clickedPosIndicator.SetActive(true);
                }

                if (selectedUnit != null)
                {
                    Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<Ingame_UnitCtrl>();
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
