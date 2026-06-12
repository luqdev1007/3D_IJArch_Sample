using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Camera
{
    [CreateAssetMenu(fileName = "CameraConfig", menuName = "Configs/CameraConfig")]
    public class CameraConfig : ScriptableObject
    {
        [Header("Mouse Sensitivity")]
        public float MouseSensitivityX = 0.15f;
        public float MouseSensitivityY = 0.1f;

        [Header("Gamepad Sensitivity")]
        public float GamepadSensitivityX = 180f;
        public float GamepadSensitivityY = 120f;

        [Header("Limits")]
        public float MinPitchAngle = -30f;
        public float MaxPitchAngle = 50f;
        public float DefaultDistance = 5f;
    }
}