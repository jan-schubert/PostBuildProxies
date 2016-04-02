using System;
using System.Collections.Generic;

namespace PostBuildProxies.Core.Definitions
{
    public class AssemblyDefinition
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public string FileName { get; set; }
        public string Directory { get; set; }
        public IList<TypeDefinition> TypeDefinitions { get; set; } = new List<TypeDefinition>();
    }
}
