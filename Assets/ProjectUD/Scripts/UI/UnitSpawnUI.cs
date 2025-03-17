using UnityEngine;
using UnityEngine.UI;

public class UnitSpawnUI : MonoBehaviour
{
    [SerializeField] private UnitSpawnButtonUI[] spawnBtns;
    [SerializeField] private Transform selectedUI;

    public void SetSpawnButton(int index, Sprite icon, int tier, int cost)
    {
        spawnBtns[index].Set(icon, tier, cost);
    }

    public void Select(int index)
    {
        selectedUI.gameObject.SetActive(true);
        Vector2 pos = selectedUI.localPosition;
        pos.x = spawnBtns[index].transform.localPosition.x;
        selectedUI.localPosition = pos;
    }

    public void Deselect()
    {
        selectedUI.gameObject.SetActive(false);
    }
}
