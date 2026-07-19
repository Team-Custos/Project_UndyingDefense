using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPageController : MonoBehaviour
{
    [SerializeField] private RectTransform viewport;       // RectMask2D가 붙은 보이는 영역
    [SerializeField] private RectTransform pageContainer;  // 페이지들을 담는 부모
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private float slideDuration = 0.35f;
    [SerializeField] private Ease slideEase = Ease.OutCubic;

    private int pageCount;
    private int currentIndex = 0;
    private float pageWidth;
    private bool isSliding = false;

    private void Awake()
    {
        pageCount = pageContainer.childCount;
        pageWidth = viewport.rect.width;

        for (int i = 0; i < pageCount; i++)
        {
            RectTransform page = pageContainer.GetChild(i) as RectTransform;
            page.anchoredPosition = new Vector2(pageWidth * i, 0);
            page.sizeDelta = new Vector2(pageWidth, page.sizeDelta.y);
        }

        UpdateArrowState();
    }

    // 메인 로비 페이지 버튼 클릭 시 호출되는 메서드
    public void OnClickNext() => MoveToPage(currentIndex + 1);
    public void OnClickPrev() => MoveToPage(currentIndex - 1);

    private void MoveToPage(int targetIndex)
    {
        if (isSliding || targetIndex < 0 || targetIndex >= pageCount) return;

        isSliding = true;
        currentIndex = targetIndex;

        pageContainer.DOAnchorPosX(-pageWidth * currentIndex, slideDuration)
            .SetEase(slideEase)
            .OnComplete(() =>
            {
                isSliding = false;
                UpdateArrowState();
            });

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    private void UpdateArrowState()
    {
        prevButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < pageCount - 1;
    }
}
