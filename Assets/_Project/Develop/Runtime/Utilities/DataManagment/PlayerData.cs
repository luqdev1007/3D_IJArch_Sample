using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilities.DataManagment
{
    public class PlayerData : ISaveData
    {
        public Dictionary<CurrencyTypes, int> WalletData;
        public List<int> CompletedLevels;
        public Dictionary<StatTypes, int> StatsUpgradeLevel;
    }
}
