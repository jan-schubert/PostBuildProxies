using System;
using System.Reflection;
using System.Reflection.Emit;

namespace PostBuildProxies.Core.UnitTest.Generation.PropertyGenerators
{
    public class TestBase : Generation.TestBase
    {
        public TestBase()
        {
            TestRuntimeAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Test"), AssemblyBuilderAccess.Run);
            TestModuleBuilder = TestRuntimeAssembly.DefineDynamicModule("Test");
            TestTypeBuilder = TestModuleBuilder.DefineType("TestNamespace.TestClass");
        }

        protected readonly AssemblyBuilder TestRuntimeAssembly;
        protected readonly ModuleBuilder TestModuleBuilder;
        protected readonly TypeBuilder TestTypeBuilder;
    }
}