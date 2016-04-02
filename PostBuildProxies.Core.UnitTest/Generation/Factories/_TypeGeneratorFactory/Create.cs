using FluentAssertions;
using Microsoft.Practices.Unity;
using Moq;
using PostBuildProxies.Core.Builders;
using PostBuildProxies.Core.Generation;
using PostBuildProxies.Core.UnitTest.Generation.TestClasses;
using Xunit;

namespace PostBuildProxies.Core.UnitTest.Generation.Factories._TypeGeneratorFactory
{
    public class Create : TestBase
    {
        [Fact]
        public void ShouldCreateTypeGenerator()
        {
            Container.RegisterInstance(new Mock<IPropertyGeneratorFactory>().Object);
            Container.RegisterType<ITypeGenerator, TypeGenerator>();
            var factory = Container.Resolve<TypeGeneratorFactory>();

            var generator = factory.Create(typeof(PublicAbstractClass));

            generator.Should().NotBeNull();
        }
    }
}