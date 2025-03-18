using InputEventInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitClickDetector : MonoBehaviour, IInputClick
{
    [SerializeField] private UnitSelectUI unitSelectUI;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputEventManager inputEventManager;


    private void Start()
    {
        inputEventManager.OnClickTarget = this;
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
                if(hit.collider.CompareTag("Unit"))
                {
                    Debug.Log(22222);
                    Unit unit = hit.collider.GetComponent<Unit>();
                    
                    unitSelectUI.ShowHP(unit);
                }
                else
                {
                    Debug.Log(33333);
                    unitSelectUI.HideHP();
                }

            }
        }
    }
}
