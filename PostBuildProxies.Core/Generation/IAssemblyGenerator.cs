using PostBuildProxies.Core.Definitions;

namespace PostBuildProxies.Core.Generation
{
    public interface IAssemblyGenerator
    {
        void Create();
        void Initialize(AssemblyDefinition definition);
    }
}