using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WSWhitehouse.TagSelector;

//이 스크립트는 공격 범위를 관리하기 위한 스크립트입니다.
public class RangeCtrl : MonoBehaviour
{
    [TagSelector] public string tagToSearch; //찾을 오브젝트의 태그 설정.

    public float radius = 5;//공격 범위 수치.

    public List<GameObject> detectedObjects = new List<GameObject>();//감지된 게임 오브젝트.
    public List<GameObject> ignoreList = new List<GameObject>();//무시할 게임 오브젝트 (자기 자신 등)

    public GameObject FinalTarget;//최종 타겟.
    public GameObject Obj_Nearest = null; //가장 가까이 있는 오브젝트.
    public SphereCollider RangeCollider;

    private void Awake()
    {
        RangeCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        RangeCollider.radius = radius;//공격 범위를 콜라이더 반지름에 적용.
        if (detectedObjects.Count <= 0)
        {
            Obj_Nearest = null;
        }

        FinalTarget = NearestObjectSearch();
    }

    public GameObject NearestObjectSearch()//가까이 있는 오브젝트를 검색.
    {
        if (Obj_Nearest == null) //없어졌을경우.
        {
            ListTargetDelete(Obj_Nearest);//리스트 초기화.
        }
        else if (Obj_Nearest.GetComponent<Ingame_UnitCtrl>().isDead)
        {
            ListTargetDelete(Obj_Nearest);
        }

        if (detectedObjects.Count > 0) //한 게임 오브젝트라도 탐지 되었을때,
        {
            //가장 가까이 있는 오브젝트를 찾기 위한 정렬.
            Obj_Nearest = detectedObjects[0];
            float Obj_Distance_Nearest = Vector3.Distance(Obj_Nearest.transform.position, transform.position);

            if (detectedObjects.Count > 1)//2개 이상 있을경우 최단거리 계산을 위함. 
            {
                for (int i = 1; i < detectedObjects.Count - 1; i++) //정렬.
                {
                    if (detectedObjects[i] != null)
                    {
                        float Obj_Distance = Vector3.Distance(detectedObjects[i].transform.position, transform.position);
                        if (Obj_Distance_Nearest > Obj_Distance)
                        {
                            //Obj_Nearest = detectedObjects[i];

                            if (detectedObjects[i].activeInHierarchy == false && detectedObjects.Contains(detectedObjects[i]))
                            {
                                detectedObjects.Remove(detectedObjects[i]);
                                continue;
                            }
                            else
                            {
                                Obj_Nearest = detectedObjects[i];
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            return Obj_Nearest;
        }
        else
        {
            return null;
        }
    }

    public void ListTargetDelete(GameObject latestTarget)//리스트 초기화.
    {
        if (detectedObjects.Contains(latestTarget))
        {
            detectedObjects.Remove(latestTarget);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.root.TryGetComponent(out UnitCtrl_ReBuild unit))
        {
            if (other.CompareTag(tagToSearch.ToString()))//찾을 태그에 맞을 경우. (외부 스크립트 사용.)
            {
                if (detectedObjects.Contains(unit.gameObject) == false)
                {
                    detectedObjects.Add(unit.gameObject); //리스트에 추가.
                }
            }
            else return;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.TryGetComponent(out UnitCtrl_ReBuild unit))
        {
            if (other.CompareTag(tagToSearch))//찾을 태그에 맞을 경우. (외부 스크립트 사용.)
            {
                if (Obj_Nearest == unit.gameObject)
                {
                    Obj_Nearest = null; //가까이 있는 오브젝트 변수를 초기화.
                }

                ListTargetDelete(unit.gameObject);//리스트에서 삭제.
            }
            else return;
        }
    }

    private void RemoveDetectedUnit(Ingame_UnitCtrl ctrl)
    {
        ListTargetDelete(ctrl.gameObject);
        GridManager.inst.SetTilePlaceable(ctrl.transform.position, true, true);
    }

}
