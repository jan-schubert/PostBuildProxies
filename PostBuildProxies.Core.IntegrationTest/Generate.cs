using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using FluentAssertions;
using Microsoft.Practices.Unity;
using Moq;
using PostBuildProxies.Core.Builders;
using PostBuildProxies.Core.Definitions;
using PostBuildProxies.Core.Generation;
using PostBuildProxies.Core.Generation.PropertyGenerators;
using PostBuildProxies.Core.UnitTest.Generation;
using PostBuildProxies.Core.UnitTest.Generation.TestClasses;
using Xunit;

namespace PostBuildProxies.Core.IntegrationTest
{
    public class Generate : TestBase
    {
        [Fact]
        public void ShouldGenerateAssemblyWithNotifyPropertyChangedClasses()
        {
            Container.RegisterType<IAssemblyGeneratorFactory, AssemblyGeneratorFactory>();
            Container.RegisterType<IAssemblyGenerator, AssemblyGenerator>();
            Container.RegisterType<ITypeGeneratorFactory, TypeGeneratorFactory>();
            Container.RegisterType<ITypeGenerator, TypeGenerator>();
            Container.RegisterType<IPropertyGeneratorFactory, PropertyGeneratorFactory>();
            Container.RegisterType<INotifyPropertyChangedGenerator, NotifyPropertyChangedGenerator>();
            Container.RegisterInstance<IUnityContainer>(Container);

            var typeDefinition = new TypeDefinition(typeof(PublicAbstractClass));
            typeDefinition.AddProperty<INotifyPropertyChangedGenerator>(nameof(PublicAbstractClass.MyInt));
            var configuration = new AssemblyDefinition
            {
                Name = "MyProxyAssembly",
                Version = new Version(1, 2, 3, 4),
                FileName = "MyAssemblyFileName.dll",
                Directory = "../",
                TypeDefinitions = new List<TypeDefinition>
                {
                    typeDefinition
                }
            };

            var generator = Container.Resolve<IAssemblyGeneratorFactory>().Create();
            generator.Initialize(configuration);

            generator.Create();

            var createdAssembly = Assembly.LoadFile(Path.GetFullPath(Path.Combine(configuration.Directory, configuration.FileName)));

            createdAssembly.GetName().Name.Should().Be(configuration.Name);
            createdAssembly.GetName().Version.Should().Be(configuration.Version);
        }
    }
}