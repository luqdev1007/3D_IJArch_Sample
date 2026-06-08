using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System;

namespace Assets._Project.Develop.Runtime.UI.Gameplay.ResultsPopups
{
    public class WinPopupPresenter : PopupPresenterBase
    {
        private const string TitleName = "YOU WIN!";

        private readonly WinPopupView _view;
        private readonly SceneSwitcherService _sceneSwitcher;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        public WinPopupPresenter(
            ICoroutinesPerformer coroutinesPerformer,
            WinPopupView view,
            SceneSwitcherService sceneSwitcher) : base(coroutinesPerformer)
        {
            _coroutinesPerformer = coroutinesPerformer;
            _view = view;
            _sceneSwitcher = sceneSwitcher;
        }

        protected override PopupViewBase PopupView => _view;

        public override void Initialize()
        {
            base.Initialize();

            _view.SetTitle(TitleName);

            _view.ContinueClicked += OnContinueClicked;
        }

        protected override void OnPreHide()
        {
            base.OnPreHide();

            _view.ContinueClicked -= OnContinueClicked;
        }

        public override void Dispose()
        {
            base.Dispose();

            _view.ContinueClicked -= OnContinueClicked;
        }

        private void OnContinueClicked()
        {
            _coroutinesPerformer.StartPerform(_sceneSwitcher.ProcessSwitchTo(Scenes.MainMenu));
            OnCloseRequest();
        }
    }
}
