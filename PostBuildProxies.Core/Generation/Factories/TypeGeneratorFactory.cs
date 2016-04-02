using System;
using Microsoft.Practices.Unity;

namespace PostBuildProxies.Core.Generation
{
    public class TypeGeneratorFactory : ITypeGeneratorFactory
    {
        public TypeGeneratorFactory(IUnityContainer container)
        {
            _container = container;
        }

        public ITypeGenerator Create(Type classType)
        {
            return _container.Resolve<ITypeGenerator>();
        }

        private readonly IUnityContainer _container;
    }
}