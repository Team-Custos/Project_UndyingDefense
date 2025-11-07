using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WindVFX : VFX
{
    [SerializeField] GameObject windObject;
    [SerializeField] private float spped;

    protected override void Update()
    {
        base.Update();

        transform.Translate(direction * Time.deltaTime * spped, Space.World);
    }

    //public void Activate(float angle)
    //{
    //    transform.Translate(Vector3.forward * angle * Time.deltaTime);
    //}
}
