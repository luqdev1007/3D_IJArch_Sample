using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StagesFeature
{
    public class ClearAllEnemiesStage : IStage
    {
        private ClearAllEnemiesStageConfig _config;

        private ReactiveEvent _completed = new();

        private EnemiesFactory _enemiesFactory;
        private EntitiesLifeContext _entitiesLifeContext;

        private bool _inProcess;

        private Dictionary<Entity, IDisposable> _spawnedEnemiesToRemoveReason = new();

        public ClearAllEnemiesStage(
            ClearAllEnemiesStageConfig config,
            EnemiesFactory enemiesFactory,
            EntitiesLifeContext entitiesLifeContext)
        {
            _config = config;
            _enemiesFactory = enemiesFactory;
            _entitiesLifeContext = entitiesLifeContext;
        }

        public IReadOnlyEvent Completed => _completed;

        public void Cleanup()
        {
            foreach (KeyValuePair<Entity, IDisposable> item in _spawnedEnemiesToRemoveReason)
            {
                item.Value.Dispose();
                _entitiesLifeContext.Release(item.Key);
            }

            _spawnedEnemiesToRemoveReason.Clear();

            _inProcess = false;
        }

        public void Dispose()
        {
            foreach (KeyValuePair<Entity, IDisposable> item in _spawnedEnemiesToRemoveReason)
            {
                item.Value.Dispose();
            }

            _spawnedEnemiesToRemoveReason.Clear();

            _inProcess = false;
        }

        public void Start()
        {
            if (_inProcess)
                throw new InvalidOperationException("Game mode alread started");

            SpawnEnemies();

            _inProcess = true;
        }

        public void Update(float deltaTime)
        {
            if (_inProcess == false)
                return;

            if (_spawnedEnemiesToRemoveReason.Count == 0)
                ProcessEnd();
        }

        private void ProcessEnd()
        {
            _inProcess = false;
            _completed.Invoke();
        }

        private void SpawnEnemies()
        {
            foreach (EnemyItemConfig enemyItemConfig in _config.EnemyItems)
                SpawnEnemy(enemyItemConfig);
        }

        private void SpawnEnemy(EnemyItemConfig enemyItemConfig)
        {
            Entity spawnedEnemy = _enemiesFactory.Create(enemyItemConfig.SpawnPosition, enemyItemConfig.EnemyConfig);

            IDisposable removeReason = spawnedEnemy.IsDead.Subscribe((oldValue, isDead) =>
            {
                if (isDead)
                {
                    IDisposable disposable = _spawnedEnemiesToRemoveReason[spawnedEnemy];
                    disposable.Dispose();
                    _spawnedEnemiesToRemoveReason.Remove(spawnedEnemy);
                }
            });

            _spawnedEnemiesToRemoveReason.Add(spawnedEnemy, removeReason);
        }
    }
}
