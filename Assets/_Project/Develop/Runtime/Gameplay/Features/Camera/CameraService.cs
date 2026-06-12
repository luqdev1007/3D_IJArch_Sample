using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagment;
using System;
using Unity.Cinemachine;

using Object = UnityEngine.Object;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Camera
{
    public class CameraService : IInitializable, IDisposable
    {
        private const string MainHeroCameraPath = "Camera/MainHeroCamera";

        private readonly MainHeroHolderService _mainHeroHolderService;
        private readonly ResourcesAssetsLoader _resources;

        private CinemachineCamera _mainHeroCamera;

        private IDisposable _heroRegistredDisposable;

        public CameraService(MainHeroHolderService mainHeroHolderService, ResourcesAssetsLoader resources)
        {
            _mainHeroHolderService = mainHeroHolderService;
            _resources = resources;
        }

        public void Initialize()
        {
            _heroRegistredDisposable = _mainHeroHolderService.HeroRegistred.Subscribe(OnMainHeroRegistred);

            _mainHeroCamera = Object.Instantiate(_resources.Load<CinemachineCamera>(MainHeroCameraPath));
        }
    
        public void Dispose()
        {
            _heroRegistredDisposable.Dispose();
        }

        private void OnMainHeroRegistred(Entity entity)
        {
            _mainHeroCamera.Target.TrackingTarget = entity.Transform;
        }
    }
}
