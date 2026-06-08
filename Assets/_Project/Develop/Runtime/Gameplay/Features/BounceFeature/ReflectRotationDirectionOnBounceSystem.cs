using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BounceFeature
{
    public class ReflectRotationDirectionOnBounceSystem : IInitializableSystem, IDisposableSystem
    {
        private ReactiveVariable<Vector3> _rotationDirection;
        private ReactiveEvent<RaycastHit> _bounceEvent;

        private IDisposable _bounceDisposable;

        public void OnInit(Entity entity)
        {
            _rotationDirection = entity.RotationDirection;
            _bounceEvent = entity.BounceEvent;
            _bounceDisposable = _bounceEvent.Subscribe(OnBounce);
        }

        private void OnBounce(RaycastHit hit)
        {
            _rotationDirection.Value = Vector3.Reflect(_rotationDirection.Value, hit.normal);
        }

        public void OnDispose()
        {
            _bounceDisposable.Dispose();
        }
    }
}
