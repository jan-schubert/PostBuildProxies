using System.Reflection.Emit;
using PostBuildProxies.Core.Definitions;

namespace PostBuildProxies.Core.Generation
{
    public interface ITypeGenerator
    {
        void Generate(ModuleBuilder moduleBuilder);
        void Initialize(TypeDefinition typeDefinition);
    }
}