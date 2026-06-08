using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.PauseFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.StagesFeature;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;
using Assets._Project.Develop.Runtime.Utilities.DataManagment.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
    public class GameplayStatesFactory
    {
        private readonly DIContainer _container;

        public GameplayStatesFactory(DIContainer container)
        {
            _container = container;
        }

        public CollectLootState CreateCollectLootState()
        {
            return new CollectLootState(
                _container.Resolve<LootPullingService>(),
                _container.Resolve<MainHeroHolderService>());
        }

        public PreperationState CreatePreperationState()
        {
            return new PreperationState(_container.Resolve<PreperationTriggerService>());
        }

        public StageProcessState CreateStageProcessState()
        {
            return new StageProcessState(_container.Resolve<StageProviderService>());
        }

        public WinState CreateWinState(GameplayInputArgs inputArgs)
        {
            return new WinState(
                _container.Resolve<IInputService>(),
                _container.Resolve<LevelsProgressionService>(),
                inputArgs,
                _container.Resolve<PlayerDataProvider>(),
                _container.Resolve<ICoroutinesPerformer>(),
                _container.Resolve<GameplayPopupService>(),
                _container.Resolve<IPauseService>(),
                _container.Resolve<WalletService>(),
                _container.Resolve<MainHeroHolderService>());
        }

        public DefeatState CreateDefeatState()
        {
            return new DefeatState(
                _container.Resolve<IInputService>(),
                _container.Resolve<IPauseService>(),
                _container.Resolve<GameplayPopupService>());
        }

        public GameplayStateMachine CreateGameplayStateMachine(GameplayInputArgs gameplayInputArgs)
        {
            PreperationTriggerService preperationTriggerService = _container.Resolve<PreperationTriggerService>();
            StageProviderService stageProviderService = _container.Resolve<StageProviderService>();
            MainHeroHolderService mainHeroHolderService = _container.Resolve<MainHeroHolderService>();

            GameplayStateMachine coreLoopState = CreateCoreLoopState();

            DefeatState defeatState = CreateDefeatState();
            WinState winState = CreateWinState(gameplayInputArgs);

            ICompositeCondition coreLoopToWinStateCondition = new CompositeCondition()
                .Add(new FuncCondition(() => preperationTriggerService.HasMainHeroContact.Value))
                .Add(new FuncCondition(() => stageProviderService.CurrentStageResult.Value == StageResults.Completed))
                .Add(new FuncCondition(() => stageProviderService.HasNextStage() == false));

            ICompositeCondition coreLoopToDefeatStateCondition = new CompositeCondition()
                .Add(new FuncCondition(() =>
                {
                    if (mainHeroHolderService.MainHero != null)
                        return mainHeroHolderService.MainHero.IsDead.Value;

                    return false;
                }));

            GameplayStateMachine gameplayCycle = new GameplayStateMachine();

            gameplayCycle.AddState(coreLoopState);
            gameplayCycle.AddState(winState);
            gameplayCycle.AddState(defeatState);

            gameplayCycle.AddTransition(coreLoopState, winState, coreLoopToWinStateCondition);
            gameplayCycle.AddTransition(coreLoopState, defeatState, coreLoopToDefeatStateCondition);

            return gameplayCycle;
        }

        public GameplayStateMachine CreateCoreLoopState()
        {
            PreperationTriggerService preperationTriggerService = _container.Resolve<PreperationTriggerService>();
            StageProviderService stageProviderService = _container.Resolve<StageProviderService>();
            LootPullingService lootPullingService = _container.Resolve<LootPullingService>();

            CollectLootState collectLootState = CreateCollectLootState();
            PreperationState preperationState = CreatePreperationState();
            StageProcessState stageProcessState = CreateStageProcessState();

            ICompositeCondition preperationToStageProcessCondition = new CompositeCondition()
                .Add(new FuncCondition(() => preperationTriggerService.HasMainHeroContact.Value))
                .Add(new FuncCondition(() => stageProviderService.HasNextStage()));

            FuncCondition stageProcessToCollectStateCondition =
                new FuncCondition(() => stageProviderService.CurrentStageResult.Value == StageResults.Completed);

            FuncCondition collectStateToPreperationStateCondition =
                new FuncCondition(() => lootPullingService.AllCollected.Value);

            GameplayStateMachine coreLoopState = new GameplayStateMachine();

            coreLoopState.AddState(preperationState);
            coreLoopState.AddState(collectLootState);
            coreLoopState.AddState(stageProcessState);

            coreLoopState.AddTransition(preperationState, stageProcessState, preperationToStageProcessCondition);
            coreLoopState.AddTransition(stageProcessState, collectLootState, stageProcessToCollectStateCondition);
            coreLoopState.AddTransition(collectLootState, preperationState, collectStateToPreperationStateCondition);

            return coreLoopState;
        }
    }
}
