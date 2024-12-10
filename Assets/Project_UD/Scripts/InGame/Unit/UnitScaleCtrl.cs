using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitScaleCtrl : MonoBehaviour
{
    private Vector3 originalScale;

    // Start is called before the first frame update
    void Start()
    {
        //originalScale = transform.localScale; // 원래 크기 저장
        //StartCoroutine(ScaleAnimation());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator ScaleAnimation()
    {
        // 초기 크기를 70%로 줄이기
        transform.localScale = originalScale * 0.7f;

        // 서서히 원래 크기로 돌아가기 위한 변수들
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        // 0.5초에 걸쳐 서서히 스케일 복원
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration; // 진행 정도 (0 ~ 1)

            // 선형 보간으로 스케일 변경
            transform.localScale = Vector3.Lerp(startScale, originalScale, t);

            yield return null; // 다음 프레임까지 대기
        }

        // 최종적으로 원래 크기로 맞추기(정밀도 보정)
        transform.localScale = originalScale;

        // 위치 재조정
        Vector3 pos = transform.position;
        pos.y = 0.6f;
        transform.position = pos;
    }
}
