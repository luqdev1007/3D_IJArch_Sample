using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Stages
{
    [CreateAssetMenu(menuName = "Configs/Gameplay/Stages/NewClearAllEnemiesStage", fileName = "ClearAllEnemiesStage")]
    public class ClearAllEnemiesStageConfig : StageConfig
    {
        [SerializeField] private List<EnemyItemConfig> _enemyItems;

        public IReadOnlyList<EnemyItemConfig> EnemyItems => _enemyItems;
    }
}
