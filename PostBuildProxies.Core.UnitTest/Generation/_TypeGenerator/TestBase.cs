using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Practices.Unity;
using Moq;
using PostBuildProxies.Core.Builders;
using PostBuildProxies.Core.Generation.PropertyGenerators;

namespace PostBuildProxies.Core.UnitTest.Generation._TypeGenerator
{
    public class TestBase
        : Generation.TestBase
    {
        public TestBase()
        {
            PropertyGeneratorFactoryMock = new Mock<IPropertyGeneratorFactory>();
            PropertyGeneratorFactoryMock.Setup(item => item.Create(It.IsAny<Type>())).Returns(() =>
            {
                var mock = new Mock<IPropertyGenerator>();
                mock.Setup(item => item.Generate(It.IsAny<TypeBuilder>(), It.IsAny<string>()));
                PropertyGeneratorMocks.Add(mock);
                return mock.Object;
            });

            Container.RegisterInstance(PropertyGeneratorFactoryMock.Object);

            TestRuntimeAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Test"), AssemblyBuilderAccess.Run);
            TestModuleBuilder = TestRuntimeAssembly.DefineDynamicModule("Test");
        }

        protected readonly AssemblyBuilder TestRuntimeAssembly;
        protected readonly ModuleBuilder TestModuleBuilder;
        protected readonly Mock<IPropertyGeneratorFactory> PropertyGeneratorFactoryMock;
        protected readonly List<Mock<IPropertyGenerator>> PropertyGeneratorMocks = new List<Mock<IPropertyGenerator>>();
    }
}