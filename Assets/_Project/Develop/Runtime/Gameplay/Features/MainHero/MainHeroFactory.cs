using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AbilitiesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.LevelUPFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagment;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using System;
using UnityEngine;
using Assets.CourseGame.Develop.Gameplay.Features.StatsFeature;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
    public class MainHeroFactory
    {
        private readonly DIContainer _container;

        private readonly EntitiesFactory _entitiesFactory;
        private readonly BrainsFactory _brainsFactory;
        private readonly ConfigsProviderService _configsProviderService;
        private readonly EntitiesLifeContext _entitiesLifeContext;
        private readonly StatsUpgradeService _statsUpgradeService;

        public MainHeroFactory(DIContainer container)
        {
            _container = container;
            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();
            _configsProviderService = _container.Resolve<ConfigsProviderService>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
            _statsUpgradeService = _container.Resolve<StatsUpgradeService>();
        }

        public Entity Create(Vector3 position)
        {
            MainHeroConfig config = _configsProviderService.GetConfig<MainHeroConfig>();

            Entity entity = _entitiesFactory.CreateMainHero(position, config, GetStats());

            entity
                .AddIsMainHero()
                .AddTeam(new ReactiveVariable<Teams>(Teams.MainHero))

                .AddAbilities()
                .AddSystem(new AbilityOnAddActivatorSystem())

                .AddCoins()

                .AddLevel(new ReactiveVariable<int>(1))
                .AddExperience()
                .AddSystem(new LevelUpSystem(_configsProviderService.GetConfig<ExperienceForUpgradeLevelConfig>()))

                .AddCurrentTarget();

            _brainsFactory.CreateMainHeroBrain(entity, new NearestDamageableTargetSelector(entity));

            _entitiesLifeContext.Add(entity);

            return entity;
        }


        private Dictionary<StatTypes, float> GetStats()
        {
            Dictionary<StatTypes, float> stats = new();

            foreach (StatTypes statType in Enum.GetValues(typeof(StatTypes)))
                stats.Add(statType, _statsUpgradeService.GetCurrentStatValueFor(statType));

            return stats;
        }
    }
}
