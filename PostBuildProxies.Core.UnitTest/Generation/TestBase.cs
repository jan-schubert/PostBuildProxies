using Microsoft.Practices.Unity;

namespace PostBuildProxies.Core.UnitTest.Generation
{
    public class TestBase
    {
        public TestBase()
        {
            Container = new UnityContainer();
        }

        protected UnityContainer Container;
    }
}