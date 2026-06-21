using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SystemConfirmUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI confirmText;
    private UltEvent confirmEvent;

    public void SetConfirmUI(LocalizedString message, UltEvent confirmEvent)
    {
        confirmText.text = message.GetLocalizedString();
        this.confirmEvent = confirmEvent;
        gameObject.SetActive(true);
    }

    public void ConfirmEventInvoke()
    {
        confirmEvent?.Invoke();
        gameObject.SetActive(false);
    }

    public void CancelEventInvoke()
    {
        gameObject.SetActive(false);
    }
}
