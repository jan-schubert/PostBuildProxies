using System;
using PostBuildProxies.Core.Generation.PropertyGenerators;

namespace PostBuildProxies.Core.Builders
{
    public interface IPropertyGeneratorFactory
    {
        IPropertyGenerator Create(Type propertyGeneratorType);
    }
}