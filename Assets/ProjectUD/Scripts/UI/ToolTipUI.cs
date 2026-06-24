using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTipUI : MonoBehaviour
{
    [Header("패널 높이 조정 관련 변수")]
    [SerializeField] private float padding = 20f; // 패널 높이에 추가할 패딩 값

    [SerializeField] private TextMeshProUGUI skillDescTxt;
    [SerializeField] private TextMeshProUGUI skillEffectTxt;
    [SerializeField] private GameObject skillDescriptionPanel;
    [SerializeField] private GameObject skillEffectPanel;

    // 텍스트의 길이가 길어지면 패널의 크기를 조정하는 기능
    public void SetPanelHeight()
    {
        //Canvas.ForceUpdateCanvases();
        //skillDescTxt.ForceMeshUpdate();

        // skillDescriptionPanel의 높이를 skillDescTxt의 높이에 맞게 조정
        float descHeight = skillDescTxt.preferredHeight;
        RectTransform descRect = skillDescriptionPanel.GetComponent<RectTransform>();
        descRect.sizeDelta = new Vector2(descRect.sizeDelta.x, descHeight + padding); // 20은 패딩 값으로 조정 가능

        // skillEffectPanel의 높이를 skillEffectTxt의 높이에 맞게 조정
        float effectHeight = skillEffectTxt.preferredHeight;
        RectTransform effectRect = skillEffectPanel.GetComponent<RectTransform>();
        effectRect.sizeDelta = new Vector2(effectRect.sizeDelta.x, effectHeight + padding); // 20은 패딩 값으로 조정 가능
    }
}
