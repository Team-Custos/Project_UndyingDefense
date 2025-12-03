using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawnUI : MonoBehaviour
{
    [SerializeField] private UnitSpawnButtonUI[] spawnBtns;
    [SerializeField] private Transform selectedUI;
    [SerializeField] private TextMeshProUGUI[] spawnBtnPriceText;
    [SerializeField] private Image[] frameImage;
    [SerializeField] private Sprite selectIcon;
    [SerializeField] private Sprite frameIcon;
    private int selectedIndex = -1;

    public void SetSpawnButton(int index, Sprite icon, int tier, int cost)
    {
        spawnBtns[index].Set(icon, tier, cost);
    }

    public void Select(int index)
    {
        if (selectedIndex != -1 && selectedIndex != index)
        {
            frameImage[selectedIndex].sprite = frameIcon;
        }

        frameImage[index].sprite = selectIcon;

        selectedIndex = index;

        //selectedUI.gameObject.SetActive(true);
        //Vector2 pos = selectedUI.localPosition;
        //pos.x = spawnBtns[index].transform.localPosition.x;
        //selectedUI.localPosition = pos;
    }

    public void Deselect()
    {
        if (selectedIndex != -1)
            frameImage[selectedIndex].sprite = frameIcon;

        selectedIndex = -1;

        //selectedUI.gameObject.SetActive(false);
    }

    
}
