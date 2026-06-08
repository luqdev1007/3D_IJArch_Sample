using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.PauseFeature;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class DefeatState : EndGameState, IUpdatableState
    {
        private readonly GameplayPopupService _popupService;

        public DefeatState(IInputService inputService, IPauseService pauseService, GameplayPopupService popupService) : base(inputService, pauseService)
        {
            _popupService = popupService;
        }

        public override void Enter()
        {
            base.Enter();

            Debug.Log("ПОРАЖЕНИЕ!");

            _popupService.OpenDefeatPopup();
        }

        public void Update(float deltaTime)
        {
        }
    }
}
