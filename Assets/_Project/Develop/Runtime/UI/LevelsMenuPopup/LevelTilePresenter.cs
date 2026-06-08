using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.LevelsMenuPopup
{
    public class LevelTilePresenter : ISubscribedPresenter
    {
        private readonly LevelsProgressionService _levelsService;
        private readonly SceneSwitcherService _sceneSwitcherService;
        private readonly ICoroutinesPerformer _coroutinesPerformer;

        private readonly int _levelNumber;

        private readonly LevelTileView _view;

        public LevelTilePresenter(
            LevelsProgressionService levelsService,
            SceneSwitcherService sceneSwitcherService, 
            ICoroutinesPerformer coroutinesPerformer, 
            int levelNumber, 
            LevelTileView view)
        {
            _levelsService = levelsService;
            _sceneSwitcherService = sceneSwitcherService;
            _coroutinesPerformer = coroutinesPerformer;
            _levelNumber = levelNumber;
            _view = view;
        }

        public LevelTileView View => _view;

        public void Initialize()
        {
            _view.SetLevel(_levelNumber.ToString());

            if (_levelsService.CanPlay(_levelNumber))
            {
                if (_levelsService.IsLevelCompleted(_levelNumber))
                    _view.SetComplete();
                else
                    _view.SetActive();
            }
            else
            {
                _view.SetBlock();
            }
        }

        public void Dispose()
        {
            _view.Clicked -= OnViewClicked;
        }

        public void Subscribe()
        {
            _view.Clicked += OnViewClicked;
        }

        public void Unsubscribe()
        {
            _view.Clicked -= OnViewClicked;
        }

        private void OnViewClicked()
        {
            if(_levelsService.CanPlay(_levelNumber) == false)
            {
                Debug.Log("Уровень заблокирован, пройдите предыдущий");
                return;
            }

            _coroutinesPerformer
                .StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, new GameplayInputArgs(_levelNumber)));
        }
    }
}
