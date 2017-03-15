using RocketWorks.Serialization;
using System;
using System.Collections.Generic;

namespace RocketWorks.CodeGeneration
{
    class ContextsBuilder : ClassBuilder
    {
        public ContextsBuilder(List<string> serializeableTypes)
        {
            BuildImports("Implementation.Components", "RocketWorks.Entities", "System.Collections.Generic");
            BuildHeader("", "Contexts", "", true);
            string lines = "";
            for (int i = 0; i < serializeableTypes.Count; i++)
            {
                string typeName = serializeableTypes[i];
                lines += string.Format("idToType.Add({0}, typeof({1})); typeToId.Add(typeof({1}), {0});", i, typeName);
            }
            BuildMethod("Populate", "partial void", lines);
            BuildEnding();
        }
    }
}
