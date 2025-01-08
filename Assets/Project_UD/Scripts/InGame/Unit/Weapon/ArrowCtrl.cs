using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이 스크립트는 화살 오브젝트를 관리하기 위한 스크립트입니다.
public class ArrowCtrl : MonoBehaviour
{
    public int Atk = 1;
    public float speed = 1f;
    public bool isEnemyAttack = false;

    public float alphaColor = 1f;
    MaterialPropertyBlock block;
    public Color baseColor;

    MeshRenderer meshRenderer;


    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        block = new MaterialPropertyBlock();
        //GetComponent<Renderer>().GetPropertyBlock(block);
        Destroy(gameObject,3f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * 0.5f);

        meshRenderer.GetPropertyBlock(block);
        block.SetColor("_BASE_COLOR", new Color(1, 0, 0, alphaColor));
        meshRenderer.SetPropertyBlock(block);

        Material mat = meshRenderer.sharedMaterial;
        if (mat != null)
        {
            Debug.Log($"Shader: {mat.shader.name}");
            for (int i = 0; i < mat.shader.GetPropertyCount(); i++)
            {
                Debug.Log($"Property {i}: {mat.shader.GetPropertyName(i)} - {mat.shader.GetPropertyType(i)}");
            }
        }

    }

    //public void StickToTarget(Transform target)
    //{
    //    transform.SetParent(target);
    //    transform.localPosition = Vector3.zero + Vector3.up * transform.localPosition.y;

    //    speed = 0;

    //    StartCoroutine(FadeDestroy());

    //    IEnumerator FadeDestroy()
    //    {
    //        yield return new WaitForSeconds(1f);
    //        for (float i = 1; i >= 0; i -= 0.1f)
    //        {
    //            GetComponent<Renderer>().GetPropertyBlock(block).GetColor("BaseColor");
    //            color.a = i;
    //            GetComponent<Renderer>().material.color = color;
    //            yield return new WaitForSeconds(0.1f);
    //        }
    //        Destroy(gameObject);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(CONSTANT.TAG_ENEMY))
        {
            Ingame_UnitCtrl unitCtrl = other.GetComponent<Ingame_UnitCtrl>();

            transform.SetParent(other.transform);
            transform.localPosition = Vector3.zero + Vector3.up * transform.localPosition.y;

            speed = 0;

            //StickToTarget(unitCtrl.VisualModel.transform);
        }
    }

}
