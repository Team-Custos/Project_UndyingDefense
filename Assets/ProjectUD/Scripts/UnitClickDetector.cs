using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitClickDetector : MonoBehaviour, IInputClick
{
    [SerializeField] private UnitHpUI unitHp;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputEventManager inputEventManager;

    private void Start()
    {
        if (inputEventManager != null)
        {
            inputEventManager.OnClickTarget = this;
        }
    }

public void OnClick(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            Debug.Log(1111);

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                Unit unit = hit.collider.GetComponent<Unit>();

                if(unit != null)
                {
                    Debug.Log("fefe");
                    unitHp.ShowHP(unit);
                }
                else
                {
                    unitHp.HideHP();
                }
            }
            else
            {
                unitHp.HideHP();
            }
            
        }
    }
}
