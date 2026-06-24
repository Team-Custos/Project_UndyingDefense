using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShortCutKeyManager : MonoBehaviour, IInputSpeedUp
{
    [SerializeField] private InGameManager inGameManager;
    [SerializeField] private PlayerInputEventManager inputEventManager;

    private void Start()
    {
        inputEventManager.OnSpeedUpTarget = this;
    }

    public void OnSpeedUp(InputAction.CallbackContext context)
    {
        if (inGameManager.IsGamgePause)
            return;

        if (context.action.name == "SpeedUp" && context.performed)
        {
            Time.timeScale = 3.0f;
        }
        else if (context.canceled)
        {
            Time.timeScale = 1.0f;
        }
    }

}
