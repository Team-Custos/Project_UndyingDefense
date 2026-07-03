using System.Collections;
using System.Collections.Generic;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Settings;

public class StatusUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ToolTipUI toolTipUI;
    [SerializeField] private GameObject statusPanel;
    [SerializeField] private TextMeshProUGUI statusNameText;
    [SerializeField] private TextMeshProUGUI statusTypeText;
    [SerializeField] private TextMeshProUGUI statusDescText;
    [SerializeField] private TextMeshProUGUI statusEffectText;
    [SerializeField] private float yPos;
    [SerializeField] private float xPos;

    [SerializeField] private RectTransform unitStatePanelRectTransform;
    [SerializeField] private RectTransform iconRectTransform;

    private DurationEffect effect;

    //private void Start()
    //{
    //    unitStatePanelRectTransform = statusPanel.GetComponent<RectTransform>();
    //    iconRectTransform = GetComponent<RectTransform>();

    //}

    public void OnPointerExit(PointerEventData eventData)
    {
        statusPanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 iconPos = iconRectTransform.anchoredPosition;
        Vector2 panelPos = iconPos + new Vector2(xPos, yPos);


        unitStatePanelRectTransform.anchoredPosition = panelPos;

        statusPanel.SetActive(true);

        SetStatusText(effect);
        toolTipUI.SetPanelHeight();

    }

    private void SetStatusText(DurationEffect effect)
    {
        statusNameText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("Status", $"{effect.Id}_name", LocalizationSettings.SelectedLocale);

        string key = $"stts_type_{effect.Type}";

        statusTypeText.text = LocalizationSettings.StringDatabase
            .GetLocalizedString("Status", key, LocalizationSettings.SelectedLocale);

        statusDescText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("Status", $"{effect.Id}_desc", LocalizationSettings.SelectedLocale);

        statusEffectText.text = LocalizationSettings.StringDatabase.
            GetLocalizedString("Status", $"{effect.Id}_effect", LocalizationSettings.SelectedLocale);
    }

    public void SetEffect(DurationEffect effect)
    {
        this.effect = effect;
    }

    public void HideStatusInfo()
    {
        statusPanel.SetActive(false);
    }

    private void OnDisable()
    {
        statusPanel.SetActive(false);
    }
}
