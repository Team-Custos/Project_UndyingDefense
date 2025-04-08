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

    public interface IInputClick    // 마우스 좌클릭
    {
        void OnClick(InputAction.CallbackContext context);
    }
    public interface IInputRightClick
    {
        void OnRightClick(InputAction.CallbackContext context);
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

    public interface IInputUnitSpawn
    {
        void OnUnitSpawn(InputAction.CallbackContext context);
    }

    public interface IInputUnitUpgrade
    {
        void OnUnitUpgrade(InputAction.CallbackContext context);
    }

    public interface IInputUnitModeChange
    {
        void OnUnitModeChange(InputAction.CallbackContext context);
    }
}
