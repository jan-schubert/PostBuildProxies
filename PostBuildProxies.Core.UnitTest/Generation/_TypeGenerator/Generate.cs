using System;
using System.Reflection.Emit;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Moq;
using PostBuildProxies.Core.Definitions;
using PostBuildProxies.Core.Generation;
using PostBuildProxies.Core.Generation.PropertyGenerators;
using PostBuildProxies.Core.UnitTest.Generation.TestClasses;
using Xunit;

namespace PostBuildProxies.Core.UnitTest.Generation._TypeGenerator
{
    public class Generate : TestBase
    {
        [Fact]
        public void ShouldGenerateClassWithDefaultName()
        {
            var typeDefinition = new TypeDefinition(typeof(PublicAbstractClass));

            var typeGenerator = Container.Resolve<TypeGenerator>();
            typeGenerator.Initialize(typeDefinition);
            typeGenerator.Generate(TestModuleBuilder);

            TestRuntimeAssembly.GetType(GetProxyClassFullName<PublicAbstractClass>(typeDefinition)).Should().NotBeNull();
            TestRuntimeAssembly.GetType(GetProxyClassFullName<PublicAbstractClass>(typeDefinition)).Name.Should().Be("Proxy_PublicAbstractClass");
            TestRuntimeAssembly.GetType(GetProxyClassFullName<PublicAbstractClass>(typeDefinition)).Namespace.Should().Be("PostBuildProxies.Core.UnitTest.Generation.TestClasses");
        }

        [Theory]
        [InlineData("$ClassName$Proxy")]
        [InlineData("$ClassName$_Proxy")]
        public void ShouldGenerateClassByName(string namePattern)
        {
            var typeDefinition = new TypeDefinition(typeof(PublicAbstractClass))
            {
                ClassNamePattern = namePattern
            };

            var typeGenerator = Container.Resolve<TypeGenerator>();
            typeGenerator.Initialize(typeDefinition);
            typeGenerator.Generate(TestModuleBuilder);

            TestRuntimeAssembly.GetType(GetProxyClassFullName<PublicAbstractClass>(typeDefinition)).Should().NotBeNull();
            TestRuntimeAssembly.GetType(GetProxyClassFullName<PublicAbstractClass>(typeDefinition)).Name.Should().Be(typeDefinition.ClassNamePattern.Replace("$ClassName$", nameof(PublicAbstractClass)));
            TestRuntimeAssembly.GetType(GetProxyClassFullName<PublicAbstractClass>(typeDefinition)).Namespace.Should().Be("PostBuildProxies.Core.UnitTest.Generation.TestClasses");
        }

        [Fact]
        public void ShouldInheritFromProxiedClass()
        {
            var typeDefinition = new TypeDefinition(typeof(PublicAbstractClass));

            var typeGenerator = Container.Resolve<TypeGenerator>();
            typeGenerator.Initialize(typeDefinition);
            typeGenerator.Generate(TestModuleBuilder);

            TestRuntimeAssembly.GetType(GetProxyClassFullName<PublicAbstractClass>(typeDefinition)).BaseType.FullName.Should().Be(typeof (PublicAbstractClass).FullName);
        }

        [Fact]
        public void ShouldGeneratePropertyOverrideForAllConfiguredProperties()
        {
            var typeDefinition = new TypeDefinition(typeof(PublicAbstractClass));
            typeDefinition.AddProperty<IPropertyGenerator>(nameof(PublicAbstractClass.MyString)).AddProperty<IPropertyGenerator>(nameof(PublicAbstractClass.MyInt));
            var typeGenerator = Container.Resolve<TypeGenerator>();
            typeGenerator.Initialize(typeDefinition);

            typeGenerator.Generate(TestModuleBuilder);

            PropertyGeneratorFactoryMock.Verify(item => item.Create(It.IsAny<Type>()), Times.Exactly(2));

            foreach (var propertyGeneratorMock in PropertyGeneratorMocks)
            {
                propertyGeneratorMock.Verify(item => item.Generate(It.IsAny<TypeBuilder>(), It.IsAny<string>()), Times.Once);
            }
        }

        private static string GetProxyClassFullName<TClass>(TypeDefinition typeDefinition)
        {
            return $"{typeDefinition.ClassType.Namespace}.{typeDefinition.ClassNamePattern.Replace("$ClassName$", typeof(TClass).Name)}";
        }
    }
}