using Microsoft.Practices.Unity;
using PostBuildProxies.Core.Generation.PropertyGenerators;

namespace PostBuildProxies.Core.UnitTest.Generation.PropertyGenerators._NotifyPropertyChangedGenerator
{
    public class TestBase : PropertyGenerators.TestBase
    {
        public TestBase()
        {
            Container.RegisterType<IPropertyGenerator, NotifyPropertyChangedGenerator>();
        }
    }
}