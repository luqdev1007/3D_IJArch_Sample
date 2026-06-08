using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEditor.Build;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LevelUPFeature
{
    public class Experience : IEntityComponent
    {
        public ReactiveVariable<float> Value;
    }

    public class Level : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }
}
