using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
    public class PlayerInputSystem : IInitializableSystem, IUpdatableSystem
    {
        private readonly IInputService _input;

        private ReactiveVariable<Vector3> _moveDirection;
        private ReactiveVariable<Vector3> _rotationDirection;

        public PlayerInputSystem(IInputService input)
        {
            _input = input;
        }

        public void OnInit(Entity entity)
        {
            _moveDirection = entity.MoveDirection;
            _rotationDirection = entity.RotationDirection;
        }

        public void OnUpdate(float deltaTime)
        {
            _moveDirection.Value = _input.MoveDirection;
            _rotationDirection.Value = _input.MoveDirection;
        }
    }
}
