using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

//이 스크립트는 플레이어의 조작 이벤트를 위한 스크립트입니다.

public class GameOrderSystem : MonoBehaviour
{
    public static GameOrderSystem instance;

     InGameManager GAMEMANAGER;

    public GameObject clickedObj = null; //클릭한 오브젝트
    public GameObject selectedUnit = null; //선택한 유닛 (아군, 적군)

    public Vector3 clickedWorldPos = Vector3.zero; //클릭한 지점의 월드 좌표.

    public GameObject clickedPosIndicator = null; //클릭한 지점을 표시하기 위한 오브젝트 (삭제 예정)

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GAMEMANAGER = InGameManager.inst;
        Ingame_InputSystem.Instance.OnPrimaryPerformed += OnPrimaryButtonOrder;
        Ingame_InputSystem.Instance.OnSecondaryPerformed += OnSecondaryButtonOrder; // 오른쪽 클릭 이벤트 추가

    }

    private void OnDestroy()
    {
        Ingame_InputSystem.Instance.OnPrimaryPerformed -= OnPrimaryButtonOrder;
        Ingame_InputSystem.Instance.OnSecondaryPerformed -= OnSecondaryButtonOrder; // 오른쪽 클릭 이벤트 제거
    }

    private void OnPrimaryButtonOrder()//마우스 왼쪽 클릭 이벤트 처리 함수
    {
        // UI가 클릭되었는지 우선 확인
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // UI가 클릭된 경우 다른 이벤트가 발생하지 않도록 즉시 반환
            return;
        }

        if(selectedUnit == null)
        {
            Ingame_UIManager.instance.unitInfoPanel.SetActive(false);
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit) && (Input.GetMouseButtonDown(0)))
        {

            clickedObj = hit.collider.gameObject;
            clickedWorldPos = hit.point;

            //타일 클릭했을때
            if (clickedObj.CompareTag(CONSTANT.TAG_TILE))
            {
                InGameManager.inst.UnitSetMode = false;
                InGameManager.inst.AllyUnitSetMode = false;

                if (Ingame_UIManager.instance.currentSelectedUnitOptionBox != null)
                {
                    Ingame_UIManager.instance.DestroyUnitStateChangeBox();
                }

                if (Ingame_UIManager.instance.currentUpgradeMenu != null)
                {
                    Ingame_UIManager.instance.DestroyUnitUpgradeMenu();
                }


                Ingame_UIManager.instance.unitInfoPanel.SetActive(false);

                GridTile GridTile = clickedObj.GetComponent<GridTile>();
                //Debug.Log("클릭한 그리드 좌표 : " + GridTile.GridPos + ", 배치 가능 여부 : " + GridTile.isPlaceable);

                //if (GAMEMANAGER.UnitSetMode && GridTile.isPlaceable) // 배치 상태이고 배치 가능한 타일을 클릭
                //{
                //    if (GAMEMANAGER.AllyUnitSetMode)//아군 배치
                //    {
                //        UnitSpawnManager SpawnMgr = UnitSpawnManager.inst;
                //        GridTile.currentPlacedUnit = SpawnMgr.UnitSpawn(SpawnMgr.unitToSpawn, GridTile.transform.position.x, GridTile.transform.position.z);

                //        InGameManager.inst.UnitSetMode = false;
                //        InGameManager.inst.AllyUnitSetMode = false;
                //    }
                //    else if (GAMEMANAGER.EnemyUnitSetMode)// 적 배치
                //    {
                //        //EnemySpawner SpawnMgr = EnemySpawner.inst;
                //        //GridTile.currentPlacedUnit = SpawnMgr.EnemySpawn(1, GridTile.transform.position.x, GridTile.transform.position.z);
                //        //GridTile.isPlaceable = false;
                //        InGameManager.inst.UnitSetMode = false;
                //        InGameManager.inst.EnemyUnitSetMode = false;
                //    }
                //}
                //if (GridTile.currentPlacedUnit == null)
                //{
                //    GAMEMANAGER.AllTileSelectOff();
                //    GridTile.Selected = !GridTile.Selected;

                //    //if (selectedUnit != null)
                //    //{
                //    //    Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<Ingame_UnitCtrl>();
                //    //    AllyUnit.isSelected = false;

                //        //if (AllyUnit.Ally_Mode == AllyMode.Free)
                //        //{
                //        //    if (GridTile.isPlaceable == false)
                //        //    {
                //        //        return;
                //        //    }

                //        //    GridTile.SelectedTile(true);
                //        //    Debug.Log("Fefe");

                //        //    AllyUnit.moveTargetPos = GridTile.transform.position;
                //        //    AllyUnit.targetEnemy = null;
                //        //    AllyUnit.haveToMovePosition = true;
                //        //}
                //        selectedUnit = null;
                //    // }

                //    GridTile.Selected = false;
                //}
                if (selectedUnit != null) // 유닛이 선택된 상태에서 타일을 누르면 유닛 선택 해제
                {
                    //Vector3 GridTilePos = GridTile.transform.position;

                    Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<Ingame_UnitCtrl>();
                    AllyUnit.isSelected = false;

                    //Ingame_ParticleManager.Instance.StopUnitSelectEffect(selectedUnit);

                    //    if (AllyUnit.Ally_Mode == AllyMode.Free)
                    //    {
                    //        AllyUnit.haveToMovePosition = true;
                    //        AllyUnit.moveTargetPos = new Vector3(GridTilePos.x, 0, GridTilePos.z);
                    //    }

                    selectedUnit = null;
                }

            }
            //유닛 클릭했을 때
            else if (clickedObj.CompareTag(CONSTANT.TAG_UNIT))
            {
                SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_select);

                if (GAMEMANAGER.UnitSetMode && GAMEMANAGER.AllyUnitSetMode)
                {
                    GAMEMANAGER.UnitSetMode = false;
                    GAMEMANAGER.AllyUnitSetMode = false;
                }

                Ingame_UnitCtrl AllyUnit = hit.collider.GetComponent<Ingame_UnitCtrl>();

                Ingame_UnitCtrl[] allUnit = FindObjectsOfType<Ingame_UnitCtrl>();
                foreach (var unit in allUnit)
                {
                    unit.isSelected = false;
                }
                AllyUnit.isSelected = !AllyUnit.isSelected;

                if (AllyUnit.isSelected && AllyUnit.CompareTag(CONSTANT.TAG_UNIT))
                {
                    selectedUnit = AllyUnit.gameObject;

                    //Ingame_UIManager.instance.ShowUnitClickUI(AllyUnit);

                    // 파티클 제거
                    //Ingame_ParticleManager.Instance.StopUnitSelectEffect(selectedUnit);

                    if (Ingame_UIManager.instance.currentSelectedUnitOptionBox != null)
                    {
                        Destroy(Ingame_UIManager.instance.currentSelectedUnitOptionBox);
                        Ingame_UIManager.instance.currentSelectedUnitOptionBox = null;
                    }
                    
                    Ingame_UIManager.instance.CreateSeletedUnitdOptionBox(hit.point, AllyUnit);
                    Ingame_UIManager.instance.unitInfoPanel.SetActive(true);
                    Ingame_UIManager.instance.UpdateUnitInfoPanel(AllyUnit);

                    // 선택된 유닛이 아군인지 적군인지 확인하여 파티클 재생
                    bool isAlly = clickedObj.CompareTag(CONSTANT.TAG_UNIT);
                    //Ingame_ParticleManager.Instance.PlayUnitSelectEffect(clickedObj, isAlly);
                }
                else
                {
                    selectedUnit = null;
                }
            }
            //적 클릭했을 때
            else if (clickedObj.CompareTag(CONSTANT.TAG_ENEMY))
            {
                SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_select);

                if (Ingame_UIManager.instance.currentSelectedUnitOptionBox != null)
                {
                    Ingame_UIManager.instance.DestroyUnitStateChangeBox();
                }

                if (Ingame_UIManager.instance.currentUpgradeMenu != null)
                {
                    Ingame_UIManager.instance.DestroyUnitUpgradeMenu();
                }

                Ingame_UnitCtrl Enemy = hit.collider.GetComponent<Ingame_UnitCtrl>();

                // 모든 유닛의 선택 해제
                Ingame_UnitCtrl[] allEnemys = FindObjectsOfType<Ingame_UnitCtrl>();
                foreach (var unit in allEnemys)
                {
                    unit.isSelected = false;
                    //Ingame_UIManager.instance.ShowUnitClickUI(unit); // UI 업데이트
                }

                Enemy.isSelected = !Enemy.isSelected;

                if (Enemy.isSelected)
                {
                    selectedUnit = Enemy.gameObject;

                    // 선택된 적의 UI 업데이트
                    //Ingame_UIManager.instance.ShowUnitClickUI(Enemy);
                }
                else
                {
                    selectedUnit = null;
                }

                if (selectedUnit != null && selectedUnit.CompareTag(CONSTANT.TAG_UNIT))
                {
                    Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<Ingame_UnitCtrl>();
                    AllyUnit.isSelected = false;
                    AllyUnit.targetEnemy = clickedObj;

                    if (AllyUnit.Ally_Mode == AllyMode.Free)
                    {
                        AllyUnit.moveTargetPos = new Vector3(Enemy.transform.position.x, 0, Enemy.transform.position.z);
                    }
                    Enemy.isSelected = false;


                    selectedUnit = null;
                }


                Ingame_UIManager.instance.unitInfoPanel.SetActive(true);
                Ingame_UIManager.instance.UpdateUnitInfoPanel(Enemy);
            }
            //else if (clickedObj.tag == CONSTANT.TAG_ENEMY)
            //{
            //    Ingame_UnitCtrl Enemy = clickedObj.GetComponent<Ingame_UnitCtrl>();
            //    Enemy.isSelected = !Enemy.isSelected;

            //    if (selectedUnit != null)
            //    {
            //        Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<Ingame_UnitCtrl>();
            //        AllyUnit.isSelected = false;
            //        AllyUnit.targetEnemy = clickedObj.gameObject;

            //        if (AllyUnit.Ally_Mode == AllyMode.Free)
            //        {
            //            AllyUnit.moveTargetPos = new Vector3(Enemy.transform.position.x, 0, Enemy.transform.position.z);
            //        }
            //        Enemy.isSelected = false;

            //        selectedUnit = null;
            //    }
            //}
            //지형 클릭했을 때
            else if (clickedObj.CompareTag(CONSTANT.TAG_GROUND))
            {

                InGameManager.inst.UnitSetMode = false;
                InGameManager.inst.AllyUnitSetMode = false;

                

                //if (clickedPosIndicator != null)
                //{
                //    clickedPosIndicator.transform.position = clickedWorldPos;
                //    clickedPosIndicator.SetActive(true);
                //}

                if (selectedUnit != null)
                {
                    Ingame_UnitCtrl AllyUnit = selectedUnit.GetComponent<Ingame_UnitCtrl>();
                    AllyUnit.isSelected = false;

                    //if (AllyUnit.Ally_Mode == AllyMode.Free)
                    //{
                    //    AllyUnit.haveToMovePosition = true;
                    //    AllyUnit.moveTargetPos = new Vector3(clickedWorldPos.x, 0, clickedWorldPos.z);
                    //}

                    selectedUnit = null;
                }
            }
        }
    }



    private void OnSecondaryButtonOrder() //마우스 오른쪽 클릭 이벤트 처리 함수.
    {
        //To do : Secondary Button Event

        if (GAMEMANAGER.UnitSetMode) // 배치 모드일 때만 작동
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) && Input.GetMouseButtonDown(1))
            {
                GameObject clickedObj = hit.collider.gameObject;

                // 배치 가능한 타일을 우클릭한 경우
                if (clickedObj.CompareTag(CONSTANT.TAG_TILE))
                {
                    GridTile gridTile = clickedObj.GetComponent<GridTile>();

                    if (gridTile.isPlaceable) // 타일이 배치 가능할 때
                    {
                        if (GAMEMANAGER.AllyUnitSetMode) // 아군 배치
                        {
                            UnitSpawnManager spawnMgr = UnitSpawnManager.inst;
                            gridTile.currentPlacedUnit = spawnMgr.UnitSpawn(spawnMgr.unitToSpawn, gridTile.transform.position.x, gridTile.transform.position.z);

                            InGameManager.inst.UnitSetMode = false;
                            InGameManager.inst.AllyUnitSetMode = false;

                            SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_assignAble);

                            
                        }
                        else if (GAMEMANAGER.EnemyUnitSetMode) // 적군 배치
                        {
                            // EnemySpawner 관련 코드 추가 (예: EnemySpawn 호출)
                            InGameManager.inst.UnitSetMode = false;
                            InGameManager.inst.EnemyUnitSetMode = false;
                        }


                        selectedUnit = null; // 배치 후 선택 해제
                    }
                    else
                    {
                        SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_assignUnable);
                    }
                }
            }
        }
        else if (selectedUnit != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit) && Input.GetMouseButtonDown(1))
            {
                GameObject clickedObj = hit.collider.gameObject;

                // 타일을 오른쪽 클릭하여 유닛을 이동
                if (clickedObj.CompareTag(CONSTANT.TAG_TILE))
                {
                    GridTile gridTile = clickedObj.GetComponent<GridTile>();

                    if (gridTile.isPlaceable) // 타일이 배치 가능할 때
                    {
                        Ingame_UnitCtrl allyUnit = selectedUnit.GetComponent<Ingame_UnitCtrl>();
                        allyUnit.isSelected = false;

                        if (allyUnit.Ally_Mode == AllyMode.Free)
                        {
                            Ingame_ParticleManager.Instance.ShowUnitMoveIndicator(gridTile.transform);
                            allyUnit.moveTargetPos = gridTile.transform.position;
                            allyUnit.targetEnemy = null;
                            allyUnit.haveToMovePosition = true;

                            SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_assignAble);

                            gridTile.SelectedTile(true); // 타일 선택 효과
                            Ingame_UIManager.instance.DestroyUnitStateChangeBox();
                        }

                        selectedUnit = null; // 이동 후 선택 해제
                    }
                    else
                    {
                        SoundManager.instance.PlayUnitSFX(SoundManager.unitSfx.sfx_assignUnable);
                    }
                }
                else if (clickedObj.CompareTag(CONSTANT.TAG_GROUND)) // 지형 클릭 시
                {
                    Ingame_UnitCtrl allyUnit = selectedUnit.GetComponent<Ingame_UnitCtrl>();
                    allyUnit.isSelected = false;

                    if (allyUnit.Ally_Mode == AllyMode.Free)
                    {
                        allyUnit.haveToMovePosition = true;
                        allyUnit.moveTargetPos = new Vector3(hit.point.x, 0, hit.point.z);
                    }

                    selectedUnit = null;
                }
            }


        }

    }
}
