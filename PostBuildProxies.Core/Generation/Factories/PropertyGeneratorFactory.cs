using System;
using Microsoft.Practices.Unity;
using PostBuildProxies.Core.Generation.PropertyGenerators;

namespace PostBuildProxies.Core.Builders
{
    public class PropertyGeneratorFactory
        : IPropertyGeneratorFactory
    {
        public PropertyGeneratorFactory(IUnityContainer container)
        {
            _container = container;
        }

        public IPropertyGenerator Create(Type propertyGeneratorType)
        {
            if(propertyGeneratorType == null)
                throw new ArgumentNullException(nameof(propertyGeneratorType));

            try
            {
                return _container.Resolve(propertyGeneratorType) as IPropertyGenerator;
            }
            catch(Exception)
            {
                throw new ArgumentException($"No property generator registered for type {propertyGeneratorType.Name}.");
            }
        }

        private readonly IUnityContainer _container;
    }
}