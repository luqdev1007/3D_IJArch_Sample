using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    /// <summary>
    /// Расширенный интерфейс ввода для souls-like управления.
    /// Добавьте новые члены к вашему существующему IInputService.
    /// </summary>
    public interface IInputService
    {
        bool IsEnabled { get; set;  } 

        // ── Существующие (движение) ────────────────────────────────────────────
        Vector3 MoveDirection { get; }

        // ── Новые: камера ──────────────────────────────────────────────────────

        /// <summary>
        /// Ввод правого стика / дельта мыши.
        /// X = горизонталь (yaw), Y = вертикаль (pitch).
        /// </summary>
        Vector2 LookDirection { get; }

        /// <summary>Колёсико мыши или вертикаль D-Pad для зума (−1..+1).</summary>
        float ZoomInput { get; }

        // ── Новые: lock-on ─────────────────────────────────────────────────────

        /// <summary>Нажатие кнопки захвата цели (R3 / средняя кнопка мыши).</summary>
        bool LockOnPressed { get; }

        /// <summary>Нажатие переключения цели вправо (правый стик вправо при lock-on).</summary>
        bool SwitchTargetRightPressed { get; }

        /// <summary>Нажатие переключения цели влево.</summary>
        bool SwitchTargetLeftPressed { get; }
    }
}