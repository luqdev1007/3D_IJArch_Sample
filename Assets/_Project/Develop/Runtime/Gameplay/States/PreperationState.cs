using Assets._Project.Develop.Runtime.Gameplay.Features.StagesFeature;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class PreperationState : State, IUpdatableState
    {
        private readonly PreperationTriggerService _preperationTriggerService;

        public PreperationState(PreperationTriggerService preperationTriggerService)
        {
            _preperationTriggerService = preperationTriggerService;
        }

        public override void Enter()
        {
            base.Enter();

            Vector3 nextStageTriggerPosition = Vector3.zero + Vector3.forward * 4;
            _preperationTriggerService.Create(nextStageTriggerPosition);
        }

        public void Update(float deltaTime)
        {
            _preperationTriggerService.Update(deltaTime);
        }

        public override void Exit()
        {
            base.Exit();

            _preperationTriggerService.Cleanup();
        }
    }
}
