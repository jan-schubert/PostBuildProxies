using System;

namespace PostBuildProxies.Core.Generation
{
    public interface ITypeGeneratorFactory
    {
        ITypeGenerator Create(Type classType);
    }
}