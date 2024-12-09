using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScaleCtrl : MonoBehaviour
{
    private Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = transform.localScale; // 원래 크기 저장
        StartCoroutine(ScaleAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator ScaleAnimation()
    {
        // 초기 크기를 70%로 줄이기
        transform.localScale = originalScale * 0.7f;

        yield return new WaitForSeconds(0.5f);

        // 원래 크기로 복원
        transform.localScale = originalScale;

        Vector3 pos = transform.position;
        pos.y = 0.6f;  // y축 높이 조정
        transform.position = pos;
    }
}
