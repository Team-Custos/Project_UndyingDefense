using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltEvents;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class SystemConfirmUI : MonoBehaviour
{
    [SerializeField] SettingUI settingUI;
    [SerializeField] PlayerInputEventManager inputEventManager;

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

        if(settingUI != null && inputEventManager != null && settingUI.gameObject.activeSelf)
        {
            inputEventManager.OnESCTarget = settingUI;
        }
    }

    public void CancelEventInvoke()
    {
        gameObject.SetActive(false);
        inputEventManager.OnESCTarget = settingUI;
    }
}
