using Assets._Project.Develop.Runtime.Configs.Meta.Stats;
using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets.CourseGame.Develop.Gameplay.Features.StatsFeature;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.StatsUpgradePopup
{
    public class UpgradableStatPresenter : IPresenter
    {
        private UpgradableStatView _view;
        private StatsViewConfig _statsViewConfig;
        private StatsUpgradeService _upgradeStatsService;
        private WalletService _walletService;
        private StatTypes _statType;
        private CurrencyIconsConfig _currencyIconsConfig;

        private List<IDisposable> _disposables = new();

        public UpgradableStatPresenter(
            UpgradableStatView view,
            StatTypes type,
            StatsViewConfig statsShowConfig,
            StatsUpgradeService upgradeStatsService,
            WalletService walletService,
            CurrencyIconsConfig currencyIconsConfig)
        {
            _view = view;
            _statsViewConfig = statsShowConfig;
            _upgradeStatsService = upgradeStatsService;
            _walletService = walletService;
            _statType = type;
            _currencyIconsConfig = currencyIconsConfig;
        }

        public UpgradableStatView View => _view;

        public void Initialize()
        {
            StatViewConfig statShowData = _statsViewConfig.GetStatViewData(_statType);

            _view.Initialize(statShowData.Name, statShowData.Sprite, GetStatValueText());

            UpdateBuyButtonState();

            _view.BuyButtonView.Click += OnBuyButtonClicked;

            IReadOnlyVariable<int> statLevel = _upgradeStatsService.GetStatLevelFor(_statType);
            _disposables.Add(statLevel.Subscribe(OnStatUpgradeLevelChanged));

            IReadOnlyVariable<int> currency = _walletService.GetCurrency(_upgradeStatsService.GetUpgradeCostTypeFor(_statType));
            _disposables.Add(currency.Subscribe(OnWalletChanged));
        }

        public void Dispose()
        {
            _view.BuyButtonView.Click -= OnBuyButtonClicked;

            foreach (IDisposable disposable in _disposables)
                disposable.Dispose();
        }

        private void OnStatUpgradeLevelChanged(int arg1, int arg2) => _view.SetStatValueText(GetStatValueText());


        private void OnWalletChanged(int arg1, int arg2) => UpdateBuyButtonState();


        private void OnBuyButtonClicked()
        {
            if (_upgradeStatsService.TryGetUpgradeCostFor(_statType, out CurrencyTypes currencyType, out int cost))
            {
                if(_walletService.Enough(currencyType, cost))
                {
                    if (_upgradeStatsService.TryUpgradeStat(_statType) == false)
                        throw new Exception();

                    _walletService.Spend(currencyType, cost);
                }
                else
                {
                    Debug.Log("Not enought currency");
                }
            }
            else
            {
                Debug.Log("Already max");
            }
        }

        private void UpdateBuyButtonState()
        {
            if(_upgradeStatsService.TryGetUpgradeCostFor(_statType, out CurrencyTypes currencyType, out int cost))
            {
                _view.BuyButtonView.SetPriceText(cost.ToString());
                _view.BuyButtonView.ShowIcon();
                _view.BuyButtonView.SetIcon(_currencyIconsConfig.GetSpriteFor(currencyType));

                if (_walletService.Enough(currencyType, cost))
                    _view.BuyButtonView.Unlock();
                else
                    _view.BuyButtonView.Lock();
            }
            else
            {
                _view.BuyButtonView.HideIcon();
                _view.BuyButtonView.Lock();
                _view.BuyButtonView.SetPriceText("MAX");
            }
        }

        private string GetStatValueText()
        {
            float statValue = _upgradeStatsService.GetCurrentStatValueFor(_statType);
            string result = statValue.ToString();

            if(_upgradeStatsService.TryGetStatValueForNextLevel(_statType, out float nextStatValue))
            {
                result += $"<color=green>>{nextStatValue}</color>";
            }

            return result;
        }
    }
}
