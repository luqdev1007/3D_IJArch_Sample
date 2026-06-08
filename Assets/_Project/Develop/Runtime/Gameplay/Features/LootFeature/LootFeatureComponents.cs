using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class IsPullable : IEntityComponent
    {
    }

    public class IsPullingProcess : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class IsCollected : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class Coins : IEntityComponent
    {
        public ReactiveVariable<int> Value;
    }

    public class LootIsDropped : IEntityComponent
    {
        public ReactiveVariable<bool> Value;
    }

    public class CanDropLoot : IEntityComponent
    {
        public ICompositeCondition Value;
    }
}
