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

    [SerializeField] private SphereCollider sphereCollider;

    public List<Ingame_UnitCtrl> detectedObjects = new List<Ingame_UnitCtrl>();//감지된 게임 오브젝트.

    public Ingame_UnitCtrl FinalTarget;//최종 타겟.

    private Ingame_UnitCtrl nearestUnit;
    public Ingame_UnitCtrl NearestUnit => nearestUnit; //가장 가까이 있는 오브젝트.

    // Update is called once per frame
    void Update()
    {
        nearestUnit = NearestObjectSearch();
    }

    public void SetRadius(float value)
    {
        sphereCollider.radius = value;
    }

    public Ingame_UnitCtrl NearestObjectSearch()//가까이 있는 오브젝트를 검색.
    {
        Ingame_UnitCtrl result = null;

        float minDist = float.MaxValue;

        for (int i =0; i < detectedObjects.Count; i++)
        {
            Ingame_UnitCtrl unit = detectedObjects[i];

            //현재 죽어있는지 또는 활성화 되어 있는지 확인
            if(unit == null || unit.HP <= 0f || !unit.gameObject.activeInHierarchy)
            {
                detectedObjects.Remove(unit);
                i--;
                continue;
            }

            float dist = Vector3.Distance(detectedObjects[i].transform.position, transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                result = unit;
            }
        }

        return result;

        //if (Obj_Nearest == null) //없어졌을경우.
        //{
        //    ListTargetDelete(Obj_Nearest);//리스트 초기화.
        //}

        //if (Obj_Nearest.GetComponent<Ingame_UnitCtrl>().isDead)
        //{
        //    ListTargetDelete(Obj_Nearest);
        //}

        //if (detectedObjects.Count >= 1) //게임 오브젝트가 1개 이상일 때
        //{
        //    //가장 가까이 있는 오브젝트를 찾기 위한 정렬.
        //    if (Obj_Nearest == null)
        //    {
        //        Obj_Nearest = detectedObjects[0];
        //    }

        //    if (detectedObjects.Count > 1)//2개 이상 있을경우 최단거리 계산을 위함. 
        //    {
        //        float Obj_Distance_Nearest = Vector3.Distance(Obj_Nearest.transform.position, transform.position);
        //        for (int i = 1; i < detectedObjects.Count - 1; i++) //정렬.
        //        {

        //            if (detectedObjects[i] != null)
        //            {
        //                float Obj_Distance = Vector3.Distance(detectedObjects[i].transform.position, transform.position);
        //                if (Obj_Distance_Nearest > Obj_Distance)
        //                {
        //                    if (!detectedObjects[i].activeInHierarchy)
        //                    {
        //                        detectedObjects.Remove(detectedObjects[i]);
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        Obj_Nearest = detectedObjects[i];
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                continue;
        //            }
        //        }
        //    }

        //    return Obj_Nearest;
        //}
        //else if (detectedObjects.Count == 1)
        //{
        //    return detectedObjects[0];
        //}
        //else
        //{
        //    return null;
        //}
    }

    public void ListTargetDelete(Ingame_UnitCtrl latestTarget)//리스트 초기화.
    {
        if (detectedObjects.Contains(latestTarget))
        {
            detectedObjects.Remove(latestTarget);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToSearch))   // 탐색하려고 하는 태그와 동일하다면
        {
            if (other.TryGetComponent(out Ingame_UnitCtrl unit))
            {
                if (unit.HP > 0)
                {
                    detectedObjects.Add(unit);
                }

            }
        }

        //if (other.transform.root.TryGetComponent(out Ingame_UnitCtrl unit))
        //{
        //    if (other.CompareTag(tagToSearch.ToString()))//찾을 태그에 맞을 경우. (외부 스크립트 사용.)
        //    {
        //        if (detectedObjects.Contains(unit.gameObject) == false)
        //        {
        //            if (unit.HP > 0)
        //            {
        //                detectedObjects.Add(unit.gameObject); //리스트에 추가.
        //            }
        //        }
        //    }
        //    else return;
        //}
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagToSearch))//찾을 태그에 맞을 경우.
        {
            if (other.TryGetComponent(out Ingame_UnitCtrl unit))
            {
                detectedObjects.Remove(unit);
            }

        }

        //if (other.transform.root.TryGetComponent(out Ingame_UnitCtrl unit))
        //{
        //    if (other.CompareTag(tagToSearch))//찾을 태그에 맞을 경우. (외부 스크립트 사용.)
        //    {
        //        if (Obj_Nearest == unit.gameObject)
        //        {
        //            Obj_Nearest = null; //가까이 있는 오브젝트 변수를 초기화.
        //        }

        //        ListTargetDelete(unit.gameObject);//리스트에서 삭제.
        //    }
        //    else return;
        //}
    }


}
