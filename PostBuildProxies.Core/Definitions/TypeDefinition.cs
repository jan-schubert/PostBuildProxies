using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using PostBuildProxies.Core.Generation.PropertyGenerators;

namespace PostBuildProxies.Core.Definitions
{
    public class TypeDefinition
    {
        public Type ClassType { get; private set; }
        public string ClassNamePattern { get; set; } = "Proxy_$ClassName$";
        public ReadOnlyCollection<PropertyDefinition> PropertyDefinitions { get; }

        public TypeDefinition(Type classType)
        {
            ClassType = classType;
            PropertyDefinitions = new ReadOnlyCollection<PropertyDefinition>(_propertyExpressions);
        }

        public TypeDefinition AddProperty<TPropertyGenerator>(string propertyExpression)
            where TPropertyGenerator : IPropertyGenerator
        {
            _propertyExpressions.Add(new PropertyDefinition(typeof (TPropertyGenerator), propertyExpression));
            return this;
        }

        private readonly IList<PropertyDefinition> _propertyExpressions = new List<PropertyDefinition>();
    }
}