using System;

namespace PostBuildProxies.Core.Definitions
{
    public class PropertyDefinition
    {
        public Type PropertyGeneratorType { get; }
        public string PropertyName { get; }

        public PropertyDefinition(Type propertyGeneratorType, string propertyName)
        {
            PropertyGeneratorType = propertyGeneratorType;
            PropertyName = propertyName;
        }
    }
}