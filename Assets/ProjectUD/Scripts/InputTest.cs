
using InputEventInterface;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour, IInputNavigate
{
    public PlayerInputEventManager inputEventMng;

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
    }

    private void Start()
    {
        inputEventMng.OnNavigateTarget = this;
    }
}
