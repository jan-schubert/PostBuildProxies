using System;
using FluentAssertions;
using Microsoft.Practices.Unity;
using PostBuildProxies.Core.Builders;
using PostBuildProxies.Core.Generation.PropertyGenerators;
using Xunit;

namespace PostBuildProxies.Core.UnitTest.Generation.Factories._PropertyGeneratorFactory
{
    public class Create : PropertyGenerators.TestBase
    {
        [Fact]
        public void ShouldCreateNotifyPropertyChangedGenerator()
        {
            Container.RegisterType<INotifyPropertyChangedGenerator, NotifyPropertyChangedGenerator>();

            var factory = Container.Resolve<PropertyGeneratorFactory>();

            var propertyGenerator = factory.Create(typeof(INotifyPropertyChangedGenerator));

            propertyGenerator.Should().NotBeNull();
        }

        [Fact]
        public void ShouldThrowExceptionIfGeneratorTypeIsUnknown()
        {
            var factory = Container.Resolve<PropertyGeneratorFactory>();

            var action = new Action(() => factory.Create(typeof(INotifyPropertyChangedGenerator)));

            action.ShouldThrow<ArgumentException>();
        }
    }
}