using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HelpUI : MonoBehaviour
{
    [SerializeField] private Button manulBtn;
    [SerializeField] private Button attributeBtn;

    [SerializeField] private GameObject manulPanel;
    [SerializeField] private GameObject attributePanel;

    [SerializeField] private Sprite activSprite;
    [SerializeField] private Sprite deactivSprite;

    [SerializeField] private RectTransform[] panels;
    private Vector2 centerPosition = Vector2.zero;
    [SerializeField] private float panelSpacing = 1920f; 
    [SerializeField] private float slideDuration = 0.5f;

    private int currentIndex = 0;

    public void OnManuel()
    {
        manulPanel.SetActive(true);
        attributePanel.SetActive(false);
        manulBtn.image.sprite = activSprite;
        attributeBtn.image.sprite = deactivSprite;
    }

    public void OnAttribute()
    {
        manulPanel.SetActive(false);
        attributePanel.SetActive(true);
        manulBtn.image.sprite = deactivSprite;
        attributeBtn.image.sprite = activSprite;
    }

    public void SlideRight()
    {
        if (currentIndex < panels.Length - 1)
        {
            currentIndex++;
            UpdatePanelPositions();
        }
    }

    public void SlideLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdatePanelPositions();
        }
    }

    private void UpdatePanelPositions()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            float targetX = (i - currentIndex) * panelSpacing;
            Vector2 targetPos = new Vector2(centerPosition.x + targetX, centerPosition.y);
            panels[i].DOAnchorPos(targetPos, slideDuration)
                     .SetEase(Ease.OutCubic)
                     .SetUpdate(true);
        }
    }

    public void OpenHelp()
    {
        if(Time.timeScale != 0.0f)
            Time.timeScale = 0.0f;

        currentIndex = 0;
        ResetToFirstPanel();
        gameObject.SetActive(true);
    }

    public void CloseHelp()
    {
        Time.timeScale = 1.0f;

        gameObject.SetActive(false);
    }

    private void ResetToFirstPanel()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            float targetX = (i - currentIndex) * panelSpacing;
            Vector2 targetPos = new Vector2(centerPosition.x + targetX, centerPosition.y);
            panels[i].anchoredPosition = targetPos;
        }
    }
}
