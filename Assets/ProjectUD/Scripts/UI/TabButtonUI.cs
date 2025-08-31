using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tabText;
    [SerializeField] private CharacterArchiveUI characterArchiveUI;

    public void OnTabButtonClick(string name)
    {
        string kFactionName = tabText.text;
        characterArchiveUI.SetFactionName(kFactionName);
        characterArchiveUI.OnTabBtnClick(name);

    }

}
