using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Camera
{
    /// <summary>
    /// Управляет системой lock-on (захват цели) как в Dark Souls / Elden Ring.
    /// Ищет ближайшего врага в конусе перед игроком и в радиусе.
    /// </summary>
    public class LockOnService
    {
        // ── Настройки поиска цели ────────────────────────────────────────────
        private const float SearchRadius = 20f;   // радиус поиска, Unity units
        private const float MaxVerticalAngle = 60f;   // макс. угол по вертикали (°)
        private const float MaxHorizontalAngle = 90f;   // конус перед камерой (°)
        private const string EnemyTag = "Enemy";

        // ── Состояние ────────────────────────────────────────────────────────
        public ReactiveVariable<bool> IsLockedOn { get; } = new(false);
        public ReactiveVariable<Transform> CurrentTarget { get; } = new(null);

        private readonly List<Transform> _potentialTargets = new();

        // ── Публичный API ────────────────────────────────────────────────────

        /// <summary>Переключить lock-on: найти цель или снять захват.</summary>
        public void ToggleLockOn(Transform playerTransform, Transform cameraTransform)
        {
            if (IsLockedOn.Value)
            {
                Release();
                return;
            }

            Transform target = FindBestTarget(playerTransform, cameraTransform);
            if (target != null)
                Acquire(target);
        }

        /// <summary>Переключиться на следующую цель (как в Elden Ring — правый стик).</summary>
        public void SwitchTarget(Transform playerTransform, Transform cameraTransform, bool switchRight)
        {
            if (!IsLockedOn.Value || CurrentTarget.Value == null) return;

            RefreshPotentialTargets(playerTransform, cameraTransform);
            if (_potentialTargets.Count < 2) return;

            int currentIndex = _potentialTargets.IndexOf(CurrentTarget.Value);
            if (currentIndex < 0) { Acquire(_potentialTargets[0]); return; }

            // Сортируем по горизонтальному углу относительно текущей цели
            _potentialTargets.Sort((a, b) =>
            {
                float angleA = GetHorizontalAngleFrom(cameraTransform, a);
                float angleB = GetHorizontalAngleFrom(cameraTransform, b);
                return angleA.CompareTo(angleB);
            });

            int newIndex = (_potentialTargets.IndexOf(CurrentTarget.Value) + (switchRight ? 1 : -1))
                           % _potentialTargets.Count;
            if (newIndex < 0) newIndex = _potentialTargets.Count - 1;

            Acquire(_potentialTargets[newIndex]);
        }

        /// <summary>Проверить, не умерла ли / не скрылась ли цель (вызывать в Update).</summary>
        public void ValidateCurrentTarget(Transform cameraTransform)
        {
            if (!IsLockedOn.Value) return;
            if (CurrentTarget.Value == null || !CurrentTarget.Value.gameObject.activeInHierarchy)
            {
                Release();
                return;
            }

            // Если цель ушла слишком далеко — снять захват
            if (Vector3.Distance(cameraTransform.position, CurrentTarget.Value.position) > SearchRadius * 1.5f)
                Release();
        }

        // ── Внутренние методы ─────────────────────────────────────────────────

        private void Acquire(Transform target)
        {
            CurrentTarget.Value = target;
            IsLockedOn.Value = true;
        }

        private void Release()
        {
            CurrentTarget.Value = null;
            IsLockedOn.Value = false;
        }

        private Transform FindBestTarget(Transform player, Transform camera)
        {
            RefreshPotentialTargets(player, camera);
            if (_potentialTargets.Count == 0) return null;

            // Приоритет: минимальный угол от центра экрана, при равном — ближайший
            Transform best = null;
            float bestScore = float.MaxValue;

            foreach (Transform t in _potentialTargets)
            {
                float dist = Vector3.Distance(player.position, t.position);
                float angle = GetAngleFromCameraCenter(camera, t);
                float score = angle * 0.7f + dist * 0.3f;   // взвешенная оценка
                if (score < bestScore) { bestScore = score; best = t; }
            }

            return best;
        }

        private void RefreshPotentialTargets(Transform player, Transform camera)
        {
            _potentialTargets.Clear();
            Collider[] hits = Physics.OverlapSphere(player.position, SearchRadius);

            foreach (Collider c in hits)
            {
                if (!c.CompareTag(EnemyTag)) continue;
                Transform t = c.transform;

                // Угловой фильтр
                Vector3 dir = (t.position - camera.position).normalized;
                float hAngle = Vector3.Angle(FlatForward(camera), FlatDir(camera, t));
                float vAngle = Mathf.Abs(Vector3.SignedAngle(dir, FlatDir(camera, t), camera.right));

                if (hAngle > MaxHorizontalAngle || vAngle > MaxVerticalAngle) continue;

                // Проверка прямой видимости
                if (Physics.Linecast(camera.position, t.position + Vector3.up, 0, QueryTriggerInteraction.Ignore)) 
                    continue;

                _potentialTargets.Add(t);
            }
        }

        private static float GetAngleFromCameraCenter(Transform camera, Transform target)
        {
            Vector3 toTarget = (target.position - camera.position).normalized;
            return Vector3.Angle(camera.forward, toTarget);
        }

        private static float GetHorizontalAngleFrom(Transform camera, Transform target)
        {
            Vector3 flat = FlatDir(camera, target);
            return Vector3.SignedAngle(FlatForward(camera), flat, Vector3.up);
        }

        private static Vector3 FlatForward(Transform t) =>
            new Vector3(t.forward.x, 0f, t.forward.z).normalized;

        private static Vector3 FlatDir(Transform from, Transform to) =>
            new Vector3(to.position.x - from.position.x, 0f, to.position.z - from.position.z).normalized;
    }
}