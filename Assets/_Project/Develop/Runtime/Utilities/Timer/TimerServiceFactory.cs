using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagment;

namespace Assets._Project.Develop.Runtime.Utilities.Timer
{
    public class TimerServiceFactory
    {
        private readonly DIContainer _container;

        public TimerServiceFactory(DIContainer container)
        {
            _container = container;
        }

        public TimerService Create(float cooldown)
            => new TimerService(cooldown, _container.Resolve<ICoroutinesPerformer>());
    }
}
