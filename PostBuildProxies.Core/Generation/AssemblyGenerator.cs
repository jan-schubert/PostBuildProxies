using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using PostBuildProxies.Core.Definitions;

namespace PostBuildProxies.Core.Generation
{
    public class AssemblyGenerator : IAssemblyGenerator
    {
        private readonly ITypeGeneratorFactory _typeGeneratorFactory;

        public AssemblyGenerator(ITypeGeneratorFactory typeGeneratorFactory)
        {
            _typeGeneratorFactory = typeGeneratorFactory;
        }

        public void Create()
        {
            var assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(_definition.Name) { Version = _definition.Version }, AssemblyBuilderAccess.Save);
            var moduleBuilder = assembly.DefineDynamicModule(_definition.Name, _tempAssemblyPath);

            foreach(var typeGenerator in _typeGenerators)
            {
                typeGenerator.Generate(moduleBuilder);
            }

            SaveAssembly(assembly);
        }

        private void SaveAssembly(AssemblyBuilder assembly)
        {
            if(File.Exists(_tempAssemblyPath))
                File.Delete(_tempAssemblyPath);

            if(File.Exists(_destinationAssemblyPath))
                File.Delete(_destinationAssemblyPath);

            assembly.Save(_tempAssemblyPath);
            File.Move(_tempAssemblyPath, _destinationAssemblyPath);
        }

        public void Initialize(AssemblyDefinition definition)
        {
            _definition = definition;

            foreach(var typeDefinition in definition.TypeDefinitions)
            {
                var generator = _typeGeneratorFactory.Create(typeDefinition.ClassType);
                generator.Initialize(typeDefinition);
                _typeGenerators.Add(generator);
            }

            _tempAssemblyPath = Path.Combine(_definition.FileName);
            _destinationAssemblyPath = Path.Combine(_definition.Directory, _definition.FileName);
        }

        private AssemblyDefinition _definition;
        private string _tempAssemblyPath;
        private string _destinationAssemblyPath;
        private readonly IList<ITypeGenerator> _typeGenerators = new List<ITypeGenerator>();
    }
}