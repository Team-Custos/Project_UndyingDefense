using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankUpCongratulationUI : MonoBehaviour
{
    [SerializeField] private Image commanderIcon;
    [SerializeField] private TextMeshProUGUI commanderName;

    public void SetRankUpInfo(Sprite icon, string name)
    {
        commanderIcon.sprite = icon;
        commanderName.text = name;
    }

    public void OnDisable()
    {
        commanderIcon.sprite = null;
        commanderName.text = string.Empty;
    }

    public void ShowPanel()
    {
        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        gameObject.SetActive(false);
    }
}
