using UnityEngine.InputSystem;

namespace InputEventInterface
{
    public interface IInputSubmit
    {
        void OnSubmit(InputAction.CallbackContext context);
    }

    public interface IInputNavigate
    {
        void OnNavigate(InputAction.CallbackContext context);
    }

    public interface IInputClick
    {
        void OnClick(InputAction.CallbackContext context);
    }

    public interface IInputScrollWheel
    {
        void OnScrollWheel(InputAction.CallbackContext context);
    }

    public interface IInputSpeedUp
    {
        void OnSpeedUp(InputAction.CallbackContext context);
    }

    public interface IInputUnitDelete
    {
        void OnUnitDelete(InputAction.CallbackContext context);
    }
}
