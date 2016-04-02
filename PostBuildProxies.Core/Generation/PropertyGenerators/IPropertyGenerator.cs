using System.Reflection.Emit;

namespace PostBuildProxies.Core.Generation.PropertyGenerators
{
    public interface IPropertyGenerator
    {
        void Generate(TypeBuilder typeBuilder, string propertyName);
    }
}