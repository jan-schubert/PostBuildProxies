using System.Reflection;
using System.Reflection.Emit;
using PostBuildProxies.Core.Builders;
using PostBuildProxies.Core.Definitions;

namespace PostBuildProxies.Core.Generation
{
    public class TypeGenerator : ITypeGenerator
    {
        public TypeGenerator(IPropertyGeneratorFactory propertyGeneratorFactory)
        {
            _propertyGeneratorFactory = propertyGeneratorFactory;
        }

        public void Generate(ModuleBuilder moduleBuilder)
        {
            var typeBuilder = moduleBuilder.DefineType($"{_typeDefinition.ClassType.Namespace}." + GetProxyClassName(), TypeAttributes.Public | TypeAttributes.Class, _typeDefinition.ClassType);

            foreach (var propertyDefinition in _typeDefinition.PropertyDefinitions)
            {
                _propertyGeneratorFactory.Create(propertyDefinition.PropertyGeneratorType).Generate(typeBuilder, propertyDefinition.PropertyName);
            }

            typeBuilder.CreateType();
        }

        private string GetProxyClassName()
        {
            return _typeDefinition.ClassNamePattern.Replace("$ClassName$", _typeDefinition.ClassType.Name);
        }

        public void Initialize(TypeDefinition typeDefinition)
        {
            _typeDefinition = typeDefinition;
        }

        private readonly IPropertyGeneratorFactory _propertyGeneratorFactory;
        private TypeDefinition _typeDefinition;
    }
}