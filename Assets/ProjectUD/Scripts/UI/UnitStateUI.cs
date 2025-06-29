using System.Collections;
using System.Collections.Generic;
using System.Security;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitStateUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject unitStatePanel;
    [SerializeField] private TextMeshProUGUI stateNameText;
    [SerializeField] private TextMeshProUGUI stateDescriptionText;
    [SerializeField] private float yPos;

    private RectTransform unitStatePanelRectTransform;
    private RectTransform iconRectTransform;

<<<<<<< Updated upstream
    

    private Effect effect;
=======
    private DurationEffect effect;
>>>>>>> Stashed changes

    private void Start()
    {
        unitStatePanelRectTransform = unitStatePanel.GetComponent<RectTransform>();
        iconRectTransform = GetComponent<RectTransform>();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        unitStatePanel.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        unitStatePanel.SetActive(true);

        // 이미지 위로 위치 이동
        Vector3 iconWorldPos = iconRectTransform.position;
        Vector3 offset = new Vector3(0, yPos, 0); // 원하는 offset
        unitStatePanelRectTransform.position = iconWorldPos + offset;

<<<<<<< Updated upstream
        ShowUnitState(effect);

    }

    private void ShowUnitState(Effect effect)
=======
        // ShowUnitState(effect);

    }

    private void ShowUnitState(DurationEffect effect)
>>>>>>> Stashed changes
    {
        stateNameText.text = effect.Name;
        stateDescriptionText.text = effect.Description;
    }

<<<<<<< Updated upstream
    public void SetEffect(Effect effect)
=======
    public void SetEffect(DurationEffect effect)
>>>>>>> Stashed changes
    {
        this.effect = effect;
    }

    public void HideStateInfo()
    {
        unitStatePanel.SetActive(false);
    }

    private void OnDisable()
    {
        unitStatePanel.SetActive(false);
    }
}
