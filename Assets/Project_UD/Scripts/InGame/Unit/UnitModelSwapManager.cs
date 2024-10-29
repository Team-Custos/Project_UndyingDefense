using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

//이 스크립트는 유닛의 모델을 관리하기 위한 스크립트입니다.

public class UnitModelSwapManager : MonoBehaviour
{
    public static UnitModelSwapManager inst;
    public GameObject[] AllyModel;
    public GameObject[] EnemyModel;

    private void Awake()
    {
        inst = this;
    }

}
