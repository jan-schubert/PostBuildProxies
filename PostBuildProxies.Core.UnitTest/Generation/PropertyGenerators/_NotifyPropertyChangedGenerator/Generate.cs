using System.ComponentModel;
using FluentAssertions;
using Microsoft.Practices.Unity;
using PostBuildProxies.Core.Generation.PropertyGenerators;
using PostBuildProxies.Core.UnitTest.Generation.TestClasses;
using Xunit;

namespace PostBuildProxies.Core.UnitTest.Generation.PropertyGenerators._NotifyPropertyChangedGenerator
{
    public class Generate : TestBase
    {
        [Fact]
        public void ShouldImplementFromINotifyPropertyChanged()
        {
            var notifyPropertyChangedGenerator = Container.Resolve<IPropertyGenerator>();

            notifyPropertyChangedGenerator.Generate(TestTypeBuilder, nameof(PublicAbstractClass.MyString));
            TestTypeBuilder.CreateType();

            TestRuntimeAssembly.GetType("TestNamespace.TestClass").GetInterface(typeof (INotifyPropertyChanged).FullName).Should().NotBeNull("class doesn't inherit from INotifyPropertyChanged");
        }
    }
}