using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MaterialPropertyController : MonoBehaviour
{
    [Header("■ Components")]
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("■ Options")]
    public float transparency;
    public float reflectionsWeight;

    private MaterialPropertyBlock block;

    private void Start()
    {
        block = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(block);
    }

    private void Update()
    {
        meshRenderer.GetPropertyBlock(block);
        block.SetFloat("_TRANSPARENCY", transparency);
        block.SetFloat("_REFLECTIONS_WEIGHT", reflectionsWeight);
        meshRenderer.SetPropertyBlock(block);
    }
}
