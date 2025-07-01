using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using InputEventInterface;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class HelpUI : MonoBehaviour, IInputESC
{
    [SerializeField] private PlayerInputEventManager inputEventManager;
    [SerializeField] private InGameManager inGameManager;

    [SerializeField] private Button manulBtn;
    [SerializeField] private Button attributeBtn;

    [SerializeField] private GameObject manulPanel;
    [SerializeField] private GameObject attributePanel;

    [SerializeField] private Sprite activSprite;
    [SerializeField] private Sprite deactivSprite;

    [SerializeField] private RectTransform[] panels;
    [SerializeField] private RectTransform[] enemyPanels;

    private Vector2 centerPosition = Vector2.zero;
    [SerializeField] private float panelSpacing = 1920f; 
    [SerializeField] private float slideDuration = 0.5f;

    private int currentIndex = 0;

    public void OnManuel()
    {
        SoundManager.Instance.PlayUIClickSFX();
        manulPanel.SetActive(true);
        attributePanel.SetActive(false);
        manulBtn.image.sprite = activSprite;
        attributeBtn.image.sprite = deactivSprite;
    }

    public void OnAttribute()
    {
        SoundManager.Instance.PlayUIClickSFX();
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
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    public void SlideLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdatePanelPositions();
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    public void EnemySlideRight()
    {
        if (currentIndex < enemyPanels.Length - 1)
        {
            currentIndex++;
            UpdateEnemyPanelPositions();
            SoundManager.Instance.PlayUIClickSFX();
        }
    }

    public void EnemySlideLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateEnemyPanelPositions();
            SoundManager.Instance.PlayUIClickSFX();
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

    private void UpdateEnemyPanelPositions()
    {
        for (int i = 0; i < enemyPanels.Length; i++)
        {
            float targetX = (i - currentIndex) * panelSpacing;
            Vector2 targetPos = new Vector2(centerPosition.x + targetX, centerPosition.y);
            enemyPanels[i].DOAnchorPos(targetPos, slideDuration)
                     .SetEase(Ease.OutCubic)
                     .SetUpdate(true);
        }
    }

    public void OpenHelp()
    {
        SoundManager.Instance.PlayUIClickSFX();
        currentIndex = 0;
        ResetToFirstPanel();
        gameObject.SetActive(true);
        inputEventManager.OnESCTarget = this;
    }

    public void CloseHelp()
    {
        //SoundManager.Instance.playCancleSFX();
        gameObject.SetActive(false);

        if(inGameManager != null)
            inputEventManager.OnESCTarget = inGameManager;
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

    public void OnESC(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            CloseHelp();
        }
    }
}
