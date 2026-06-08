using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay.ResultsPopups;
using Assets.CourseGame.Develop.Gameplay.Features.AbilitiesFeature.View;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
    public class GameplayPopupService : PopupService
    {
        private readonly GameplayUIRoot _uiRoot;
        private readonly GameplayPresentersFactory _gameplayPresentersFactory;

        public GameplayPopupService(
            ViewsFactory viewsFactory,
            ProjectPresentersFactory presentersFactory,
            GameplayUIRoot uiRoot,
            GameplayPresentersFactory gameplayPresentersFactory)
            : base(viewsFactory, presentersFactory)
        {
            _uiRoot = uiRoot;
            _gameplayPresentersFactory = gameplayPresentersFactory;
        }

        protected override Transform PopupLayer => _uiRoot.PopupsLayer;

        public WinPopupPresenter OpenWinPopup(Action closedCallback = null)
        {
            WinPopupView view = ViewsFactory.Create<WinPopupView>(ViewIDs.WinPopup, PopupLayer);

            WinPopupPresenter popup = _gameplayPresentersFactory.CreateWinPopupPresenter(view);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public DefeatPopupPresenter OpenDefeatPopup(Action closedCallback = null)
        {
            DefeatPopupView view = ViewsFactory.Create<DefeatPopupView>(ViewIDs.DefeatPopup, PopupLayer);

            DefeatPopupPresenter popup = _gameplayPresentersFactory.CreateDefeatPopupPresenter(view);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }

        public AbilitySelectPopupPresenter OpenAbilitySelectPopup(
            Entity entity, 
            int level, 
            Action closedCallback = null)
        {
            AbilitySelectPopupView view = ViewsFactory.Create<AbilitySelectPopupView>(ViewIDs.AbilitySelectPopup, PopupLayer);

            AbilitySelectPopupPresenter popup = _gameplayPresentersFactory.CreateAbilitySelectPopupPresenter(view, entity, level);

            OnPopupCreated(popup, view, closedCallback);

            return popup;
        }
    }
}
