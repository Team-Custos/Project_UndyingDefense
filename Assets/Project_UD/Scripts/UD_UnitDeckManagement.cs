using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UD_UnitDeckManagement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image[] unitSlotImage = null;
    public Image[] unitListImage = null;

    public Button[] unitSelectButton = null;
    public Button[] unitSlotClearButton = null;

    private Sprite emptySlotImageSprite;
    private Color emptySlotImageColor;

    private bool isSlot1Empty = true;
    private bool isSlot2Empty = true;

    private bool isUnit1Select = false;
    private bool isUnit2Select = false;
    private bool isUnit3Select = false;

    public GameObject unitInfoPanel;
    private GameObject unitInfoPanelInstance;
    public Vector3 unitInfoPanelPos;


    // Start is called before the first frame update
    void Start()
    {
        emptySlotImageSprite = unitSlotImage[0].sprite;
        emptySlotImageColor = unitSlotImage[0].color;

        if (unitSelectButton[0] != null)
        {
            unitSelectButton[0].onClick.AddListener(() => AddUnitToSlot(0));
        }
        if (unitSelectButton[1] != null)
        {
            unitSelectButton[1].onClick.AddListener(() => AddUnitToSlot(1));
        }
        if (unitSelectButton[2] != null)
        {
            unitSelectButton[2].onClick.AddListener(() => AddUnitToSlot(2));
        }

        if (unitSlotClearButton[0] != null)
        {
            unitSlotClearButton[0].onClick.AddListener(() => ClearUnitSlot(0));
        }
        if (unitSlotClearButton[1] != null)
        {
            unitSlotClearButton[1].onClick.AddListener(() => ClearUnitSlot(1));
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddUnitToSlot(int unitIndex)
    {
        if (unitIndex < unitListImage.Length && unitSlotImage[0] != null && unitSlotImage[1] != null)
        {
            bool isUnitSelected = false;

            if (unitIndex == 0)
                isUnitSelected = isUnit1Select;
            else if (unitIndex == 1)
                isUnitSelected = isUnit2Select;
            else if (unitIndex == 2)
                isUnitSelected = isUnit3Select;

            if (!isUnitSelected)
            {
                if (isSlot1Empty)
                {
                    unitSlotImage[0].sprite = unitListImage[unitIndex].sprite;
                    unitSlotImage[0].color = unitListImage[unitIndex].color;
                    isSlot1Empty = false;
                }
                else if (isSlot2Empty)
                {
                    unitSlotImage[1].sprite = unitListImage[unitIndex].sprite;
                    unitSlotImage[1].color = unitListImage[unitIndex].color;
                    isSlot2Empty = false;
                }
                else
                {
                    return;
                }

                if (unitIndex == 0)
                {
                    isUnit1Select = true;
                }
                else if (unitIndex == 1)
                {
                    isUnit2Select = true;
                }
                else if (unitIndex == 2)
                {
                    isUnit3Select = true;
                }
            }
        }
    }

    void ClearUnitSlot(int slotIndex)
    {
        if (slotIndex < unitSlotImage.Length && unitSlotImage[slotIndex] != null)
        {
            unitSlotImage[slotIndex].sprite = emptySlotImageSprite;
            unitSlotImage[slotIndex].color = emptySlotImageColor;

            if (slotIndex == 0)
            {
                isSlot1Empty = true;
                if (unitSlotImage[0].sprite == unitListImage[0].sprite)
                {
                    isUnit1Select = false;
                }
                else if (unitSlotImage[0].sprite == unitListImage[1].sprite)
                {
                    isUnit2Select = false;
                }
                else if (unitSlotImage[0].sprite == unitListImage[2].sprite)
                {
                    isUnit3Select = false;
                }
            }
            else if (slotIndex == 1)
            {
                isSlot2Empty = true;
                if (unitSlotImage[1].sprite == unitListImage[0].sprite)
                {
                    isUnit1Select = false;
                }
                else if (unitSlotImage[1].sprite == unitListImage[1].sprite)
                {
                    isUnit2Select = false;
                }
                else if (unitSlotImage[1].sprite == unitListImage[2].sprite)
                {
                    isUnit3Select = false;
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (unitInfoPanelInstance == null)
        {
            unitInfoPanelInstance = Instantiate(unitInfoPanelInstance, transform);
            unitInfoPanelInstance.transform.position = transform.position + unitInfoPanelPos;

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (unitInfoPanelInstance != null)
        {
            Destroy(unitInfoPanelInstance);
        }
    }
}
