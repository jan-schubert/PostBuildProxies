namespace PostBuildProxies.Core.Generation
{
    public interface IAssemblyGeneratorFactory
    {
        IAssemblyGenerator Create();
    }
}