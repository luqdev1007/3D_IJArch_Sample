using Assets._Project.Develop.Runtime.UI.CommonViews;
using Assets._Project.Develop.Runtime.UI.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
    public class MainMenuScreenView : MonoBehaviour, IView
    {
        public event Action OpenLevelsMenuButtonClicked;
        public event Action OpenStatsUpgradeButtonClicked;

        [field: SerializeField] public IconTextListView WalletView { get; private set; }

        [SerializeField] private Button _openLevelsMenuButton;
        [SerializeField] private Button _openStatsUpgradeButton;

        private void OnEnable()
        {
            _openLevelsMenuButton.onClick.AddListener(OnOpenLevelsMenuButtonClicked);
            _openStatsUpgradeButton.onClick.AddListener(OnOpenStatsUpgradeButtonClicked);
        }

        private void OnDisable()
        {
            _openLevelsMenuButton.onClick.RemoveListener(OnOpenLevelsMenuButtonClicked);
            _openStatsUpgradeButton.onClick.RemoveListener(OnOpenStatsUpgradeButtonClicked);
        }

        private void OnOpenLevelsMenuButtonClicked() => OpenLevelsMenuButtonClicked?.Invoke();
        private void OnOpenStatsUpgradeButtonClicked() => OpenStatsUpgradeButtonClicked?.Invoke();
    }
}
