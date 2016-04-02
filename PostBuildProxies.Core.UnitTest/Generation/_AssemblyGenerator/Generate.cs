using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Moq;
using PostBuildProxies.Core.Definitions;
using PostBuildProxies.Core.Generation;
using PostBuildProxies.Core.UnitTest.Generation.TestClasses;
using Xunit;

namespace PostBuildProxies.Core.UnitTest.Generation._AssemblyGenerator
{
    public class Generate : TestBase
    {
        [Fact]
        public void ShouldGenerateAssembly()
        {
            Container.RegisterType<IAssemblyGeneratorFactory, AssemblyGeneratorFactory>();
            Container.RegisterType<IAssemblyGenerator, AssemblyGenerator>();
            Container.RegisterType<ITypeGeneratorFactory, TypeGeneratorFactory>();
            var typeGeneratorMock = new Mock<ITypeGenerator>();
            Container.RegisterInstance(typeGeneratorMock.Object);
            var configuration = new AssemblyDefinition
            {
                Name = "MyProxyAssembly",
                Version = new Version(1, 2, 3, 4),
                FileName = "MyAssemblyFileName.dll",
                Directory = "../",
                TypeDefinitions = new List<TypeDefinition>
                {
                    new TypeDefinition(typeof(PublicAbstractClass))
                }
            };

            var generator = Container.Resolve<IAssemblyGeneratorFactory>().Create();
            generator.Initialize(configuration);

            generator.Create();

            var createdAssembly = Assembly.LoadFile(Path.GetFullPath(Path.Combine(configuration.Directory, configuration.FileName)));

            createdAssembly.GetName().Name.Should().Be(configuration.Name);
            createdAssembly.GetName().Version.Should().Be(configuration.Version);
            typeGeneratorMock.Verify(item => item.Initialize(It.IsAny<TypeDefinition>()), Times.Once);
            typeGeneratorMock.Verify(item => item.Generate(It.IsAny<ModuleBuilder>()), Times.Once);
        }
    }
}