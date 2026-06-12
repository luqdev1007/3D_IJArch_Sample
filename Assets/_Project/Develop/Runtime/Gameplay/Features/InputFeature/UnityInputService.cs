using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    /// <summary>
    /// Конкретная реализация IInputService через автогенерируемый C# класс PlayerInput.
    /// </summary>
    public class UnityInputService : IInputService
    {
        private readonly PlayerInput _playerInput; // Наш сгенерированный класс-обёртка

        // Кэшированные ссылки на экшены
        private readonly InputAction _moveAction;
        private readonly InputAction _lookAction;
        private readonly InputAction _zoomAction;
        private readonly InputAction _lockOnAction;
        private readonly InputAction _switchRightAction;
        private readonly InputAction _switchLeftAction;

        public UnityInputService(PlayerInput playerInput)
        {
            _playerInput = playerInput;

            // Извлекаем экшены через внутренний InputActionAsset (.asset)
            _moveAction = playerInput.asset.FindAction("Move");
            _lookAction = playerInput.asset.FindAction("Look");
            _zoomAction = playerInput.asset.FindAction("Zoom");
            _lockOnAction = playerInput.asset.FindAction("LockOn");

            // Используем безопасный поиск, так как этих экшенов пока нет в ассете
            _switchRightAction = playerInput.asset.FindAction("SwitchTargetRight");
            _switchLeftAction = playerInput.asset.FindAction("SwitchTargetLeft");

            // Обязательно активируем карту инпутов при старте!
            _playerInput.Enable();
        }

        // ── IInputService ──────────────────────────────────────────────────────

        public Vector3 MoveDirection
        {
            get
            {
                if (_moveAction == null) return Vector3.zero;
                Vector2 v = _moveAction.ReadValue<Vector2>();
                return new Vector3(v.x, 0f, v.y);
            }
        }

        public Vector2 LookDirection => _lookAction != null ? _lookAction.ReadValue<Vector2>() : Vector2.zero;

        public float ZoomInput => _zoomAction != null ? _zoomAction.ReadValue<float>() : 0f;

        public bool LockOnPressed => _lockOnAction != null && _lockOnAction.WasPressedThisFrame();

        public bool SwitchTargetRightPressed => _switchRightAction != null && _switchRightAction.WasPressedThisFrame();

        public bool SwitchTargetLeftPressed => _switchLeftAction != null && _switchLeftAction.WasPressedThisFrame();

        // Вместо свойства .enabled вызываем методы .Enable() и .Disable()
        public bool IsEnabled
        {
            get => _moveAction != null && _moveAction.enabled;
            set
            {
                if (value)
                    _playerInput.Enable();
                else
                    _playerInput.Disable();
            }
        }
    }
}