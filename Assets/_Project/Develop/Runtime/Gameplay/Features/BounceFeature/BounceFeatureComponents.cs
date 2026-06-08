using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BounceFeature
{
    public class LayerToBounceReaction : IEntityComponent
    {
        public LayerMask Value;
    }

    public class BounceEvent : IEntityComponent
    {
        public ReactiveEvent<RaycastHit> Value;
    }

    public class BounceCount : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }
}
