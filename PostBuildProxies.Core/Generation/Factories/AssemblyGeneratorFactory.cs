using Microsoft.Practices.Unity;

namespace PostBuildProxies.Core.Generation
{
    public class AssemblyGeneratorFactory : IAssemblyGeneratorFactory
    {
        public AssemblyGeneratorFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IAssemblyGenerator Create()
        {
            return _container.Resolve<IAssemblyGenerator>();
        }

        private readonly IUnityContainer _container;
    }
}