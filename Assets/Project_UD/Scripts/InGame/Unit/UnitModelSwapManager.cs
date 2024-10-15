using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;



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
