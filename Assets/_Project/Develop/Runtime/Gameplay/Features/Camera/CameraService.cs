using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using System;
using UnityEngine;
using Unity.Cinemachine;
using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Camera
{
    /// <summary>
    /// Souls-like камера.
    /// 
    /// Режим ORBIT (обычный):
    ///   - Правый стик / мышь вращают камеру по орбите вокруг игрока
    ///   - Скролл мыши — приближение/удаление
    /// 
    /// Режим LOCK-ON (захват цели):
    ///   - Камера плавно смотрит на врага через Slerp
    ///   - Игрок стрейфится (боком), а не разворачивается к врагу
    ///   - Переключение целей влево/вправо
    /// </summary>
    public class CameraService : IInitializable, IDisposable
    {
        // ── Настройки орбиты (Изменяемые поля вместо const) ───────────────────
        public float OrbitSensitivityX = 0.15f; // Настройки под Mouse Delta
        public float OrbitSensitivityY = 0.10f;
        public float MinPitchAngle = -30f;      // Максимум вниз
        public float MaxPitchAngle = 50f;       // Максимум вверх
        public float DefaultDistance = 5f;
        public float MinDistance = 2f;
        public float MaxDistance = 10f;
        public float ZoomSpeed = 2f;

        // ── Настройки lock-on ─────────────────────────────────────────────────
        public float LockOnLookSpeed = 8f;      // Скорость Slerp к цели
        public float LockOnPitch = -10f;        // Фиксированный угол вниз при lock-on
        public float LockOnMinDistance = 3f;
        public float LockOnMaxDistance = 8f;

        // ── Ссылки на ресурсы ──────────────────────────────────────────────────
        private const string MainHeroCameraPath = "Camera/MainHeroCamera";

        private readonly MainHeroHolderService _mainHeroHolderService;
        private readonly ResourcesAssetsLoader _resources;
        private readonly LockOnService _lockOnService;

        private CinemachineCamera _mainHeroCamera;
        private CinemachineOrbitalFollow _orbitalFollow;
        private IDisposable _heroRegistredDisposable;

        // ── Orbit state ────────────────────────────────────────────────────────
        private float _yaw;          // Текущий угол вокруг Y
        private float _pitch;        // Текущий угол вверх/вниз
        private float _currentDist;  // Текущее расстояние от цели

        // ── References ─────────────────────────────────────────────────────────
        private Transform _heroTransform;

        public Transform CameraTransform => UnityEngine.Camera.main != null
            ? UnityEngine.Camera.main.transform
            : null;

        public bool IsLockedOn => _lockOnService.IsLockedOn.Value;

        // ── Constructor ────────────────────────────────────────────────────────
        public CameraService(
            MainHeroHolderService mainHeroHolderService,
            ResourcesAssetsLoader resources,
            LockOnService lockOnService)
        {
            _mainHeroHolderService = mainHeroHolderService;
            _resources = resources;
            _lockOnService = lockOnService;
        }

        // ── IInitializable ─────────────────────────────────────────────────────
        public void Initialize()
        {
            _heroRegistredDisposable =
                _mainHeroHolderService.HeroRegistred.Subscribe(OnMainHeroRegistred);

            _mainHeroCamera =
                Object.Instantiate(_resources.Load<CinemachineCamera>(MainHeroCameraPath));

            _orbitalFollow = _mainHeroCamera.GetComponent<CinemachineOrbitalFollow>();
            _currentDist = DefaultDistance;

            // Начальные углы берём из текущего положения камеры
            Transform cam = CameraTransform;
            if (cam != null)
            {
                _yaw = cam.eulerAngles.y;
                _pitch = cam.eulerAngles.x;
            }
        }

        // ── IDisposable ────────────────────────────────────────────────────────
        public void Dispose() => _heroRegistredDisposable?.Dispose();

        // ── Обновление из GameLoop (в LateUpdate) ─────────────────────────────
        public void Tick(float deltaTime, Vector2 lookInput, float zoomInput)
        {
            _lockOnService.ValidateCurrentTarget(CameraTransform);

            if (_lockOnService.IsLockedOn.Value)
                UpdateLockOnCamera(deltaTime);
            else
                UpdateOrbitCamera(deltaTime, lookInput, zoomInput);
        }

        // ── Lock-On toggle ─────────────────────────────────────────────────────
        public void ToggleLockOn()
        {
            if (_heroTransform == null)
                return;

            _lockOnService.ToggleLockOn(_heroTransform, CameraTransform);
        }

        /// <summary>Переключить цель вправо (true) или влево (false).</summary>
        public void SwitchLockOnTarget(bool toRight)
        {
            if (_heroTransform == null) return;
            _lockOnService.SwitchTarget(_heroTransform, CameraTransform, toRight);
        }

        // ── Private: orbit ──────────────────────────────────────────────────────
        private void UpdateOrbitCamera(float deltaTime, Vector2 lookInput, float zoomInput)
        {
            // Вращение мыши (БЕЗ deltaTime, так как это дельта пикселей за кадр)
            _yaw += lookInput.x * OrbitSensitivityX;
            _pitch -= lookInput.y * OrbitSensitivityY;
            _pitch = Mathf.Clamp(_pitch, MinPitchAngle, MaxPitchAngle);

            // Зум (zoomInput от колесика мыши идет сотнями, сбиваем масштаб через 0.01f)
            _currentDist -= zoomInput * ZoomSpeed * 0.01f;
            _currentDist = Mathf.Clamp(_currentDist, MinDistance, MaxDistance);

            ApplyOrbitToCinemachine();
        }

        private void ApplyOrbitToCinemachine()
        {
            if (_orbitalFollow == null) return;

            _orbitalFollow.TargetOffset = Vector3.up * 1.5f; // Смотрим чуть выше пивота персонажа
            _orbitalFollow.Radius = _currentDist;

            // Передаем углы в оси Cinemachine Orbital Follow
            _orbitalFollow.HorizontalAxis.Value = _yaw;
            _orbitalFollow.VerticalAxis.Value = _pitch;
        }

        // ── Private: lock-on ────────────────────────────────────────────────────
        private void UpdateLockOnCamera(float deltaTime)
        {
            Transform target = _lockOnService.CurrentTarget.Value;
            if (target == null || _heroTransform == null) return;

            // 1. Находим вектор от игрока к врагу
            Vector3 toTarget = (target.position - _heroTransform.position).normalized;

            // 2. Вычисляем целевой Yaw (горизонтальный угол), чтобы смотреть на врага
            float targetYaw = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;

            // Плавный поворот углов самой орбиты к целевым углам (без дерготни LookAt)
            _yaw = Mathf.LerpAngle(_yaw, targetYaw, LockOnLookSpeed * deltaTime);
            _pitch = Mathf.LerpAngle(_pitch, LockOnPitch, LockOnLookSpeed * deltaTime);

            // 3. Динамическая дистанция в зависимости от расстояния до врага
            float dist = Vector3.Distance(_heroTransform.position, target.position);
            _currentDist = Mathf.Lerp(_currentDist,
                Mathf.Clamp(dist * 0.8f, LockOnMinDistance, LockOnMaxDistance),
                5f * deltaTime);

            // Применяем высчитанные плавные углы в Cinemachine
            ApplyOrbitToCinemachine();
        }


        // ── Hero registered ────────────────────────────────────────────────────
        private void OnMainHeroRegistred(Entity entity)
        {
            _heroTransform = entity.Transform;
            _mainHeroCamera.Target.TrackingTarget = _heroTransform;
        }
    }
}