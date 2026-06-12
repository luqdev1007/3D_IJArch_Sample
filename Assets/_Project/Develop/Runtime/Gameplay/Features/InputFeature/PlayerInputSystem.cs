using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.Camera;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    /// <summary>
    /// Обрабатывает ввод игрока с учётом режима камеры.
    ///
    /// В обычном режиме:
    ///   - moveDirection = направление относительно камеры
    ///   - rotationDirection = то же самое (персонаж поворачивается к движению)
    ///
    /// В режиме lock-on:
    ///   - moveDirection = стрейф относительно оси игрок→цель
    ///   - rotationDirection = направление НА цель (персонаж всегда смотрит на врага)
    /// </summary>
    public class PlayerInputSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _input;
        private readonly CameraService _cameraService;

        private ReactiveVariable<Vector3> _moveDirection;
        private ReactiveVariable<Vector3> _rotationDirection;

        public PlayerInputSystem(IInputService input, CameraService cameraService)
        {
            _input = input;
            _cameraService = cameraService;
        }

        public void OnInit(Entity entity)
        {
            _moveDirection = entity.MoveDirection;
            _rotationDirection = entity.RotationDirection;
        }

        public void OnUpdate(float deltaTime)
        {
            // Обрабатываем переключение lock-on
            HandleLockOnInput();

            // Вычисляем направление движения
            Vector3 rawInput = _input.MoveDirection;
            Vector2 moveInput = new Vector2(rawInput.x, rawInput.z);

            if (_cameraService.IsLockedOn)
                UpdateLockOnMovement(moveInput);
            else
                UpdateOrbitMovement(moveInput);

            // Обновляем камеру (look input + zoom)
            Vector2 lookInput = _input.LookDirection; // правый стик / мышь
            float zoom = _input.ZoomInput;     // колёсико / триггер
            _cameraService.Tick(deltaTime, lookInput, zoom);
        }

        // ── Orbit mode ─────────────────────────────────────────────────────────
        private void UpdateOrbitMovement(Vector2 moveInput)
        {
            Vector3 direction = CalculateCameraRelativeDirection(moveInput);
            _moveDirection.Value = direction;
            _rotationDirection.Value = direction; // персонаж поворачивается к движению
        }

        // ── Lock-on mode ───────────────────────────────────────────────────────
        private void UpdateLockOnMovement(Vector2 moveInput)
        {
            Transform cam = _cameraService.CameraTransform;
            if (cam == null) { UpdateOrbitMovement(moveInput); return; }

            // Вектор вперёд = от игрока к цели (плоский)
            Vector3 camForward = cam.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cam.right;
            camRight.y = 0f;
            camRight.Normalize();

            // Движение: вперёд/назад + стрейф влево/вправо
            Vector3 moveDir = camForward * moveInput.y + camRight * moveInput.x;
            _moveDirection.Value = moveDir;

            // Поворот персонажа — всегда к цели, независимо от стика
            _rotationDirection.Value = camForward;
        }

        // ── Lock-on input handling ─────────────────────────────────────────────
        private void HandleLockOnInput()
        {
            if (_input.LockOnPressed)
                _cameraService.ToggleLockOn();

            if (_input.SwitchTargetRightPressed)
                _cameraService.SwitchLockOnTarget(true);
            else if (_input.SwitchTargetLeftPressed)
                _cameraService.SwitchLockOnTarget(false);
        }

        // ── Camera-relative direction (orbit) ──────────────────────────────────
        private Vector3 CalculateCameraRelativeDirection(Vector2 input)
        {
            Transform cameraTransform = _cameraService.CameraTransform;
            if (cameraTransform == null)
                return new Vector3(input.x, 0f, input.y);

            Vector3 forward = cameraTransform.forward; forward.y = 0f; forward.Normalize();
            Vector3 right = cameraTransform.right; right.y = 0f; right.Normalize();

            return forward * input.y + right * input.x;
        }
    }
}